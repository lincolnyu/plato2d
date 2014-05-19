using System.Collections.ObjectModel;
using Plato.Models;

namespace Plato.ModelProvisioning
{
    public interface ILinearModelManager : IModelManager
    {
        #region Properties

        ObservableCollection<IModel> AllModels { get; }

        #endregion
    }
}
