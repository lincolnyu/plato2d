using Plato.Models;

namespace Plato.ViewModels
{
    public interface IViewModelBuilder
    {
        #region Methods

        BaseViewModel Build(IModel model);

        #endregion
    }
}
