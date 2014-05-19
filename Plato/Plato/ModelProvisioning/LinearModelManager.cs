using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Plato.Helpers;
using Plato.Models;
using Trollveggen;

namespace Plato.ModelProvisioning
{
    /// <summary>
    ///  A model manager that keeps all models in memory and organises them in display order
    /// </summary>
    public class LinearModelManager : IModelProvider, ILinearModelManager
    {
        #region Fields

        /// <summary> 
        ///  Backing field for SightProvider
        /// </summary>
        private ISightProvider _sightProvider;

        /// <summary>
        ///  Backing field for CoreStateManager
        /// </summary>
        private ICoreStateManager _coreStateManager;

        /// <summary>
        ///  ModelShouldBeOnScreen() is being called
        /// </summary>
        private bool _inModelShouldBeOnScreen;

        #endregion

        #region Constructors

        /// <summary>
        ///  Instantiates and initialises a ModelLinearManager
        /// </summary>
        public LinearModelManager()
        {
            AllModels = new ObservableCollection<IModel>();

            AllModels.CollectionChanged += AllModelsOnCollectionChanged;

            IndicesModelsOnScreen = new List<int>();
            ModelsOnScreen = new ObservableCollection<IModel>();
        }

        /// <summary>
        ///  Destructor
        /// </summary>
        ~LinearModelManager()
        {
            if (SightProvider != null)
            {
                SightProvider.SightChanged -= SightProviderOnSightChanged;
            }
        }

        #endregion

        #region Properties

        #region IModelProvider members

        /// <summary>
        ///  All models that are currently on screen in display order
        /// </summary>
        public ObservableCollection<IModel> ModelsOnScreen
        {
            get; private set;
        }

        #endregion

        public int ModelCount
        {
            get
            {
                return AllModels.Count;
            }
        }

        public ISightProvider SightProvider
        {
            get
            {
                return _sightProvider;
            }
            set
            {
                if (_sightProvider != value)
                {
                    if (_sightProvider != null)
                    {
                        _sightProvider.SightChanged -= SightProviderOnSightChanged;
                    }
                    _sightProvider = value;
                    if (_sightProvider != null)
                    {
                        _sightProvider.SightChanged += SightProviderOnSightChanged;
                        UpdateAll();
                    }
                }
            }
        }

        
        /// <summary>
        ///  indices of models on screen in AllModels list
        /// </summary>
        private List<int> IndicesModelsOnScreen
        {
            get; set;
        }

        /// <summary>
        ///  All models the manager manages in display order for internal access
        /// </summary>
        public ObservableCollection<IModel> AllModels
        {
            get; private set;
        }

        
        /// <summary>
        ///  The core state manager
        /// </summary>
        /// <remarks>
        ///  This in this implemtation as a static member is evaluated only once (so cannot change)
        /// </remarks>
        private ICoreStateManager CoreStateManager
        {
            get
            {
                return _coreStateManager ?? (_coreStateManager = Factory.Resolve<ICoreStateManager>());
            }
        }

        #endregion

        #region Events

        #region IModelProvider members

        public event ModelChangedEvent ModelChanged;

        #endregion

        #endregion

        #region Methods

        #region IModelManager members

        /// <summary>
        ///  Handles model changes
        /// </summary>
        /// <param name="sender">The model that has changed and needs update</param>
        /// <param name="args">The user defined data that's attached to this change</param>
        public void OnModelChanged(object sender, ModelChangedEventArgs args)
        {
            if (_inModelShouldBeOnScreen)
            {
                // simply sends the message
                ModelChanged(sender, args);
                return;
            }

            var model = args.ChangedModel;
            // check screen first
            for (var onscrIndex = 0; onscrIndex < ModelsOnScreen.Count; onscrIndex++)
            {
                var m = ModelsOnScreen[onscrIndex];
                if (model == m)
                {
                    var shouldBeOnScreen = ModelShouldBeOnScreen(model);
                    if (shouldBeOnScreen)
                    {
                        ModelChanged(sender, args);      // only notify changes to models on screen
                    }
                    else
                    {
                        FlipModelOffScreenScreenIndex(onscrIndex);
                    }
                    return;
                }
            }

            // check offscreen list
            for (var index = 0; index < AllModels.Count; index++)
            {
                var m = AllModels[index];
                if (model == m)
                {
                    var shouldBeOnScreen = ModelShouldBeOnScreen(model);
                    if (shouldBeOnScreen)
                    {
                        FlipModelToScreen(index);
                        if (ModelChanged != null)
                        {
                            ModelChanged(sender, args);     // only notify changes to models on screen
                        }
                    }
                }
            }
        }

        #endregion

        private void AllModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    var i = e.NewStartingIndex;
                    foreach (var newItem in e.NewItems)
                    {
                        OnInsertModel(i, (IModel)newItem);
                        i++;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    var oldStartingIndex = e.OldStartingIndex;
                    var count = e.OldItems.Count;
                    for (var i = oldStartingIndex + count - 1; i >= oldStartingIndex; i--)
                    {
                        OnRemoveModel(i);
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Move:
                {
                    var oldStartingIndex = e.OldStartingIndex;
                    var count = e.OldItems.Count;
                    for (var i = oldStartingIndex + count - 1; i >= oldStartingIndex; i--)
                    {
                        OnRemoveModel(i);
                    }
                    var j = e.NewStartingIndex;
                    foreach (var newItem in e.NewItems)
                    {
                        OnInsertModel(j, (IModel)newItem);
                        j++;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    var i = e.OldStartingIndex;
                    foreach (var newItem in e.NewItems)
                    {
                        OnReplaceModel(i, (IModel)newItem);
                        i++;
                    }
                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    OnResetModels();
                    break;
                }
            }
        }

        /// <summary>
        ///  Inserts a model at the specified index in the model collection
        /// </summary>
        /// <param name="index">The index of the model to insert to</param>
        /// <param name="model">The model to insert</param>
        private void OnInsertModel(int index, IModel model)
        {
            var onscreenIndex = IndicesModelsOnScreen.Search(index.CompareTo);
            if (onscreenIndex < 0)
            {
                onscreenIndex = -onscreenIndex - 1;
            }
            for (var i = onscreenIndex; i < IndicesModelsOnScreen.Count; i++)
            {
                IndicesModelsOnScreen[i]++;
            }

            if (ModelShouldBeOnScreen(model))
            {
                ModelsOnScreen.Insert(onscreenIndex, model);
                IndicesModelsOnScreen.Insert(onscreenIndex, index);
            }
        }

        /// <summary>
        ///  Removes a model at the specified index in the model collection
        /// </summary>
        /// <param name="index">The index of the model to remove</param>
        private void OnRemoveModel(int index)
        {
            var onscreenIndex = IndicesModelsOnScreen.Search(index.CompareTo);
            var foundOnScreen = onscreenIndex >= 0;
            if (!foundOnScreen)
            {
                onscreenIndex = -onscreenIndex - 1;
            }
            if (foundOnScreen)
            {
                ModelsOnScreen.RemoveAt(onscreenIndex);
                IndicesModelsOnScreen.RemoveAt(onscreenIndex);
            }
            for (var i = onscreenIndex; i < IndicesModelsOnScreen.Count; i++)
            {
                IndicesModelsOnScreen[i]--;
            }
        }

        private void OnReplaceModel(int index, IModel newModel)
        {
            var onscreenIndex = IndicesModelsOnScreen.Search(index.CompareTo);
            var newModelShouldBeOnScreen = ModelShouldBeOnScreen(newModel);
            if (onscreenIndex >= 0)
            {
                // model was on screen
                if (newModelShouldBeOnScreen)
                {
                    ModelsOnScreen[onscreenIndex] = newModel;
                }
                else
                {
                    ModelsOnScreen.RemoveAt(onscreenIndex);
                }
            }
            else
            {
                // model was not on screen
                onscreenIndex = -onscreenIndex - 1;
                if (newModelShouldBeOnScreen)
                {
                    ModelsOnScreen.Insert(onscreenIndex, newModel);
                    IndicesModelsOnScreen.Insert(onscreenIndex, index);
                }
            }
        }

        private void OnResetModels()
        {
            IndicesModelsOnScreen.Clear();
            ModelsOnScreen.Clear();
            for (var i = 0; i < AllModels.Count; i++)
            {
                var model = AllModels[i];
                OnInsertModel(i, model);
            }
        }

        /// <summary>
        ///  Returns if the model should currently be on screen
        /// </summary>
        /// <param name="model">The model to check</param>
        /// <returns>True if the model should be on screen</returns>
        protected virtual bool ModelShouldBeOnScreen(IModel model)
        {
            if (CoreStateManager != null && CoreStateManager.IsViewRetained(model))
            {
                return true;
            }
            _inModelShouldBeOnScreen = true;
            var shouldBeOnScreen = SightProvider != null && SightProvider.ModelShouldBeOnScreen(model);
            if (shouldBeOnScreen)
            {
                _inModelShouldBeOnScreen = false;
                return true;
            }
            _inModelShouldBeOnScreen = false;
            return false;
        }

        /// <summary>
        ///  Flips a model onto the screen if it's off with its model collection index
        /// </summary>
        /// <param name="index">The model's model collection index</param>
        private void FlipModelToScreen(int index)
        {
            var onscreenIndex = IndicesModelsOnScreen.Search(index.CompareTo);
            if (onscreenIndex >= 0)
            {
                // already on screen
                return;
            }
            var model = AllModels[index];
            onscreenIndex = -onscreenIndex - 1;
            ModelsOnScreen.Insert(onscreenIndex, model);
            IndicesModelsOnScreen.Insert(onscreenIndex, index);
        }

        /// <summary>
        ///  Flips a model off the screen if it's on with its model collection index
        /// </summary>
        /// <param name="index">The model's model collection index</param>
        private void FlipModelOffScreen(int index)
        {
            var onscreenIndex = IndicesModelsOnScreen.Search(index.CompareTo);
            if (onscreenIndex < 0)
            {
                return;
            }
            FlipModelOffScreenScreenIndex(onscreenIndex);
        }

        /// <summary>
        ///  Flips a model off the screen with its current on-screen index
        /// </summary>
        /// <param name="onscreenIndex">The model's on-screen index</param>
        private void FlipModelOffScreenScreenIndex(int onscreenIndex)
        {
            ModelsOnScreen.RemoveAt(onscreenIndex);
            IndicesModelsOnScreen.RemoveAt(onscreenIndex);
        }

        /// <summary>
        ///  Updates all models as per their supposed on-screen states
        /// </summary>
        public void UpdateAll()
        {
            for (var i = 0; i < AllModels.Count; i++)
            {
                var model = AllModels[i];
                if (ModelShouldBeOnScreen(model))
                {
                    FlipModelToScreen(i);
                    if (ModelChanged != null)
                    {
                        // Update the model due to sight change
                        // provides the sight provider as the change event argument
                        var args = new ModelChangedEventArgs(model, SightProvider);
                        ModelChanged(this, args);
                    }
                }
                else
                {
                    FlipModelOffScreen(i);
                }
            }
        }

        private void SightProviderOnSightChanged(object sender)
        {
            UpdateAll();
        }
        
        #endregion
    }
}
