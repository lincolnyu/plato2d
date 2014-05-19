using Plato.ModelProvisioning;
using Plato.Models;

namespace Plato.StateManagement
{
    public interface IVmModelStateManager : ICoreStateManager
    {
        #region Methods

        bool IsViewModelRetained(IModel model);

        void SetIsViewModelRetained(IModel model, bool isRetained);

        #endregion
    }
}
