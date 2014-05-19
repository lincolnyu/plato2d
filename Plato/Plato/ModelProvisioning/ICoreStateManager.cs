using Plato.Models;

namespace Plato.ModelProvisioning
{
    public interface ICoreStateManager
    {
        #region Methods

        bool IsViewRetained(IModel model);

        void SetIsViewRetained(IModel model, bool isRetained);

        #endregion
    }
}
