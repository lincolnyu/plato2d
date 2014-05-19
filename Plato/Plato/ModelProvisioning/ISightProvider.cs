using Plato.Models;

namespace Plato.ModelProvisioning
{
    public interface ISightProvider
    {
        #region Events

        event SightChangedEvent SightChanged;

        #endregion

        #region Methods

        bool ModelShouldBeOnScreen(IModel model);

        #endregion
    }
}
