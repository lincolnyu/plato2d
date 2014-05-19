using System.Collections.Generic;
using System.Collections.Specialized;
using Plato.Models;
using Plato.ViewModels;

namespace Plato.Rendering
{
    public interface IViewModelManager
    {
        #region Properties

        IReadOnlyList<BaseViewModel> ViewModels { get; }

        #endregion

        #region Events

        event NotifyCollectionChangedEventHandler ViewModelCollectionChanged;

        #endregion

        #region Methods

        void AddTemporaryModel(ITemporaryModel model);

        void RemoveTemporaryModel(ITemporaryModel model);

        #endregion
    }
}
