using Plato.Models;
using Plato.StateManagement;
using Trollveggen;

namespace Plato.ViewModels
{
    public class VmModelToViewModel : IModelToViewModel
    {
        #region Methods

        #region IModelToViewModel members

        public virtual BaseViewModel GetViewModel(IModel model)
        {
            var vmModel = model as IVmModel;
            if (vmModel != null)
            {
                return vmModel.ViewModel;                
            }

            var nextWorker = Factory.Resolve<IModelToViewModel>(this);
            return nextWorker.GetViewModel(model);
        }

        public virtual BaseViewModel GetOrCreateViewModel(IModel model)
        {
            var vmModel = model as IVmModel;
            if (vmModel != null)
            {
                var viewModel = GetViewModel(vmModel);
                if (viewModel != null)
                {
                    return viewModel;
                }

                var viewModelBuilder = Factory.Resolve<IViewModelBuilder>();
                viewModel = viewModelBuilder.Build(vmModel);

                vmModel.ViewModel = viewModel;

                return viewModel;
            }

            var nextWorker = Factory.Resolve<IModelToViewModel>(this);
            return nextWorker.GetOrCreateViewModel(model);
        }

        public virtual void RemoveViewModel(IModel model)
        {
            var stateManager = Factory.Resolve<IVmModelStateManager>();
            if (stateManager.IsViewModelRetained(model))
            {
                return; // don't remove teh view model
            }

            var vmModel = model as IVmModel;
            if (vmModel != null)
            {
                vmModel.ViewModel = null;
                return;
            }

            var nextWorker = Factory.Resolve<IModelToViewModel>(this);
            nextWorker.RemoveViewModel(model);
        }

        #endregion

        #endregion
    }
}
