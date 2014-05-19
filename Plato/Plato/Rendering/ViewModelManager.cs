using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Plato.ModelProvisioning;
using Plato.Models;
using Plato.ViewModels;

namespace Plato.Rendering
{
    /// <summary>
    ///  A class that provides rendering management functionalities
    /// </summary>
    public class ViewModelManager : IViewModelManager
    {
        #region Fields

        /// <summary>
        ///  Maximum number of cached removed models
        /// </summary>
#if DEBUG
        private const int MaxCachedRemoved = 0;
#else
        private const int MaxCachedRemoved = 64;
#endif

        /// <summary>
        ///  The model manager this rendering manager works with
        /// </summary>
        private IModelProvider _modelProvider;

        #endregion

        #region Constructors

        /// <summary>
        ///  Instantiates and initialises a ViewModelManager
        /// </summary>
        public ViewModelManager(IModelToViewModel modelToViewModel)
        {
            InternalViewModels = new List<BaseViewModel>();
            CachedRemovedModels = new LinkedList<IModel>();
            ModelToViewModel = modelToViewModel;
            TemporaryModels = new List<int>();
        }

        #endregion

        #region Properties

        /// <summary>
        ///  A readonly reference to the collection of all currently active view models
        /// </summary>
        public IReadOnlyList<BaseViewModel> ViewModels
        {
            get
            {
                return InternalViewModels;
            }
        }

        /// <summary>
        ///  The model provider that feeds an ordered list of models to show to the view model manager
        /// </summary>
        public IModelProvider ModelProvider
        {
            get
            {
                return _modelProvider;
            }
            set
            {
                if (_modelProvider != value)
                {
                    if (_modelProvider != null)
                    {
                        _modelProvider.ModelsOnScreen.CollectionChanged -= ModelsOnScreenOnCollectionChanged;
                        _modelProvider.ModelChanged -= ModelProviderOnModelChanged;
                    }
                    _modelProvider = value;
                    if (_modelProvider != null)
                    {
                        _modelProvider.ModelsOnScreen.CollectionChanged += ModelsOnScreenOnCollectionChanged;
                        UpdateOnCollectionReset(_modelProvider.ModelsOnScreen);
                        _modelProvider.ModelChanged += ModelProviderOnModelChanged;
                    }
                }
            }
        }

        /// <summary>
        ///  Current on-screen view models, in this play order
        /// </summary>
        private List<BaseViewModel> InternalViewModels
        {
            get; set;
        }

        /// <summary>
        ///  Orderd list of recently removed and cached models
        /// </summary>
        private LinkedList<IModel> CachedRemovedModels
        {
            get; set;
        }

        /// <summary>
        ///  The entity that provides mapping from model to view model
        /// </summary>
        private IModelToViewModel ModelToViewModel { get; set; }

        /// <summary>
        ///  The list of the indcies of the temporary models in the internal list
        /// </summary>
        public List<int> TemporaryModels { get; private set; }

        #endregion

        #region Events

        #region IViewModelManager members

        /// <summary>
        ///  Notifies the view manager that the view model has changed and it should organise a view update accordingly
        /// </summary>
        public event NotifyCollectionChangedEventHandler ViewModelCollectionChanged;

        #endregion

        #endregion

        #region Methods

        #region IViewModelManager members

        /// <summary>
        ///  Adds a temporary model to the rendering
        /// </summary>
        /// <param name="model">The model to add</param>
        /// <remarks>
        ///  Note the model is always added to the top of the rendering queue
        ///  So it has no impact on the existing real model rendering
        /// </remarks>
        public void AddTemporaryModel(ITemporaryModel model)
        {
            var newViewModels = new List<BaseViewModel>();
            var viewModel = ModelToViewModel.GetOrCreateViewModel(model);
            newViewModels.Add(viewModel);
            // NOTE always add on top
            var index = InternalViewModels.Count;
            InternalViewModels.Add(viewModel);
            if (ViewModelCollectionChanged != null)
            {
                ViewModelCollectionChanged(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Add,
                    newViewModels, index));
            }
            model.TemporaryModelChanged += ModelOnTemporaryModelChanged;
        }

        /// <summary>
        ///  Removes a temporary model from the rendering
        /// </summary>
        /// <param name="model">The model to remove</param>
        public void RemoveTemporaryModel(ITemporaryModel model)
        {
            for (var i = InternalViewModels.Count - 1; i >= 0; i--)
            {
                var internalVm = InternalViewModels[i];
                if (internalVm.Model == model)
                {
                    InternalViewModels.RemoveAt(i);
                    if (ViewModelCollectionChanged != null)
                    {
                        var oldItems = new List<BaseViewModel> {internalVm};
                        ViewModelCollectionChanged(this, new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove,
                            oldItems, i));
                        model.TemporaryModelChanged -= ModelOnTemporaryModelChanged;
                    }
                    break;
                }
            }
        }

        #endregion

        #region Event handlers

        /// <summary>
        ///  This makes corresponding changes to on screen view model collection as well as 
        ///  Views on canvas as per changes to view model collection
        /// </summary>
        /// <param name="sender">The sender of the model collection change event, should be the collection in the model manager</param>
        /// <param name="e">The event arguments that provide the change details</param>
        private void ModelsOnScreenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    var index = e.NewStartingIndex;
                    var newViewModels = new List<BaseViewModel>();
                    foreach (var newItem in e.NewItems)
                    {
                        var viewModel = ModelToViewModel.GetOrCreateViewModel((IModel)newItem);
                        newViewModels.Add(viewModel);
                        UncacheReAddedModel((IModel)newItem);
                    }
                    InternalViewModels.InsertRange(index, newViewModels);

                    if (ViewModelCollectionChanged != null)
                    {
                        ViewModelCollectionChanged(this, new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Add,
                            newViewModels, index));
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    var index = e.OldStartingIndex;
                    var count = e.OldItems.Count;

                    List<BaseViewModel> oldItems = null;
                    if (ViewModelCollectionChanged != null)
                    {
                        oldItems = InternalViewModels.GetRange(index, count);
                    }
                    
                    InternalViewModels.RemoveRange(index, count);
                    foreach (var oldItem in e.OldItems)
                    {
                        CacheRemovedModel((IModel)oldItem);
                    }

                    if (ViewModelCollectionChanged != null)
                    {
                        ViewModelCollectionChanged(this, new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Remove, oldItems, index));
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    // TODO check if it works as expected
                    var index = e.NewStartingIndex;
                    
                    List<BaseViewModel> oldItems = null;
                    if (ViewModelCollectionChanged != null)
                    {
                        oldItems = InternalViewModels.GetRange(index, e.OldItems.Count);
                    }

                    foreach (var oldItem in e.OldItems)
                    {
                        CacheRemovedModel((IModel)oldItem);
                    }
                    foreach (var newItem in e.NewItems)
                    {
                        var viewModel = ModelToViewModel.GetOrCreateViewModel((IModel)newItem);
                        InternalViewModels[index++] = viewModel;
                        UncacheReAddedModel((IModel)newItem);
                    }

                    if (ViewModelCollectionChanged != null)
                    {
                        index = e.NewStartingIndex;
                        var newItems = InternalViewModels.GetRange(index, e.NewItems.Count);
                        ViewModelCollectionChanged(this, new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Replace, newItems, oldItems,
                            e.NewStartingIndex));
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Move:
                {
                    // TODO not sure if it's correct
                    var sourceIndex = e.OldStartingIndex;
                    var targetIndex = e.NewStartingIndex;
                    var count = e.NewItems.Count;
                    var splice = InternalViewModels.GetRange(sourceIndex, count);
                    InternalViewModels.RemoveRange(sourceIndex, count);
                    InternalViewModels.InsertRange(targetIndex, splice);

                    if (ViewModelCollectionChanged != null)
                    {
                        ViewModelCollectionChanged(this, new NotifyCollectionChangedEventArgs(
                            NotifyCollectionChangedAction.Move, splice, targetIndex, sourceIndex));
                    }
                  
                    break;   
                }
                case NotifyCollectionChangedAction.Reset:
                    UpdateOnCollectionReset((IEnumerable)sender);
                    break;
            }
        }

        /// <summary>
        ///  Notifies the view model of the specified model to update itself
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="args">The arguments that describe the event</param>
        private void ModelProviderOnModelChanged(object sender , ModelChangedEventArgs args)
        {
            var changedModel = args.ChangedModel;
            var vm = ModelToViewModel.GetViewModel(changedModel);
            if (vm != null)
            {
                // if it's null it means it's not on screen, no need to update it
                vm.OnModelChanged(sender, args);
            }
        }

        /// <summary>
        ///  Fired when a temporary model has changed
        /// </summary>
        /// <param name="sender">The sender of the message</param>
        /// <param name="args">The arguments that describe the change including the model that has changed</param>
        private void ModelOnTemporaryModelChanged(object sender, ModelChangedEventArgs args)
        {
            ModelProviderOnModelChanged(sender, args);
        }

        /// <summary>
        ///  Repopulates the collection in response to a drastic change to the model collection
        /// </summary>
        /// <param name="modelsOnScreen">What the source model collection contains</param>
        private void UpdateOnCollectionReset(IEnumerable modelsOnScreen)
        {
            foreach (var vm in InternalViewModels)
            {
                var model = vm.Model;
                CacheRemovedModel(model);
            }

            InternalViewModels.Clear();

            foreach (var v in modelsOnScreen)
            {
                var model = (IModel) v;
                var viewModel = ModelToViewModel.GetOrCreateViewModel(model);
                InternalViewModels.Add(viewModel);
            }

            if (ViewModelCollectionChanged != null)
            {
                ViewModelCollectionChanged(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Reset));
            }
        }

        #endregion

        /// <summary>
        ///  Caches a model that's been removed from the screen
        /// </summary>
        /// <param name="model">The model to cache</param>
        private void CacheRemovedModel(IModel model)
        {
            var node = CachedRemovedModels.Find(model);
            if (node != null)
            {
                CachedRemovedModels.Remove(node);
            }
            CachedRemovedModels.AddLast(model);
            // TODO enhance the algorithm for real scenario
            while (CachedRemovedModels.Count > MaxCachedRemoved)
            {
                var first = CachedRemovedModels.First;
                var modelToRemove = first.Value;
                ModelToViewModel.RemoveViewModel(modelToRemove);
                CachedRemovedModels.RemoveFirst();
            }
        }

        /// <summary>
        ///  Called when removed model is added back
        /// </summary>
        /// <param name="model">The model to be added back and removed from cache</param>
        private void UncacheReAddedModel(IModel model)
        {
            CachedRemovedModels.Remove(model);
        }

        #endregion
    }
}
