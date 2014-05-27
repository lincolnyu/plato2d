using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;
using Plato.ViewModels;
using Trollveggen;

namespace Plato.Rendering
{
    public class CanvasViewManager
    {
        #region Fields

        /// <summary>
        ///  Backing field for ViewModelManager
        /// </summary>
        private IViewModelManager _viewModelManager;

        /// <summary>
        ///  Backing field for Canvas
        /// </summary>
        private Canvas _canvas;

        #endregion

        #region Properties

        public IViewModelManager ViewModelManager
        {
            get
            {
                return _viewModelManager;
            }
            set
            {
                if (_viewModelManager != value)
                {
                    if (_viewModelManager != null)
                    {
                        _viewModelManager.ViewModelCollectionChanged -= OnViewModelCollectionChanged;
                    }
                    _viewModelManager = value;
                    if (_viewModelManager != null)
                    {
                        _viewModelManager.ViewModelCollectionChanged += OnViewModelCollectionChanged;
                    }
                    if (Canvas != null)
                    {
                        UpdateCanvasOnCollectionReset();
                    }
                }
            }
        }

        /// <summary>
        ///  The canvas this manager caters for
        /// </summary>
        public Canvas Canvas
        {
            get
            {
                return _canvas;
            }
            set
            {
                if (value != Canvas)
                {
                    _canvas = value;
                    if (Canvas != null)
                    {
                        UpdateCanvasOnCollectionReset();
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void OnViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Canvas == null)
            {
                return;
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        var index = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            var view = GetOrCreateView((BaseViewModel)item);
                            Canvas.Children.Insert(index++, view);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        var index = e.OldStartingIndex;
                        var count = e.OldItems.Count;
                        for (var i = index; i < index + count; i++)
                        {
                            Canvas.Children.RemoveAt(i);
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        var index = e.NewStartingIndex;
                        for (var i = index; i < index + e.NewItems.Count; i++)
                        {
                            var m = GetOrCreateView(ViewModelManager.ViewModels[i]);
                            // This is to avoid adding duplicate items in the course of e.g. swapping
                            if (Canvas.Children.Contains(m))
                            {
                                var iremove = Canvas.Children.IndexOf(m);
                                Canvas.Children[iremove] = new Line();// should be temporary
                            }
                            Canvas.Children[i] = m;
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Move:
                    {
                        var canvasSplice = new LinkedList<UIElement>();
                        var sourceIndex = e.OldStartingIndex;
                        var targetIndex = e.NewStartingIndex;
                        var count = e.OldItems.Count;
                        for (var i = sourceIndex + count - 1; i >= sourceIndex; i--)
                        {
                            canvasSplice.AddFirst(Canvas.Children[i]);
                            Canvas.Children.RemoveAt(i);
                        }
                        for (var i = targetIndex; i < targetIndex + count; i++)
                        {
                            Canvas.Children.Insert(i, canvasSplice.First.Value);
                            canvasSplice.RemoveFirst();
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    UpdateCanvasOnCollectionReset();
                    break;
            }
        }


        /// <summary>
        ///  Repopulates the canvas children collection in response to a drastic change to the source collection
        ///  or a reassignment of canvas
        /// </summary>
        private void UpdateCanvasOnCollectionReset()
        {
            Canvas.Children.Clear();
            if (ViewModelManager == null)
            {
                return;
            }
            foreach (var vm in ViewModelManager.ViewModels)
            {
                var view = GetOrCreateView(vm);
                Canvas.Children.Add(view);
            }
        }

        /// <summary>
        ///  Returns a view for the specified view model from the dictionary (cached) or creates one if unavailable
        /// </summary>
        /// <param name="viewModel">The view model to return a view for</param>
        /// <returns>The view for the view model</returns>
        private UIElement GetOrCreateView(BaseViewModel viewModel)
        {
            if (viewModel.View != null)
            {
                return viewModel.View;
            }
            var viewBuilder = Factory.Resolve<IViewBuilder>();
            var view = viewBuilder.Build(viewModel);
            viewModel.View = view;
            return view;
        }

        #endregion
    }
}
