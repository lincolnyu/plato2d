using Plato.ModelProvisioning;
using Plato.Models;
using Trollveggen;

namespace Plato.StateManagement
{
    public abstract class AttachedStateManager : IModelStateManager, IVmModelStateManager
    {
        #region Methods

        #region IModelStateManager members

        public IModelState GetModelState(IModel model)
        {
            var stateModel = model as IStateModel;
            if (stateModel != null)
            {
                return stateModel.State;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            return nextWorker.GetModelState(model);
        }

           /// <summary>
        ///  Retrieves the state or builds one if doesn't exist
        /// </summary>
        /// <param name="model">The model to get the state of</param>
        /// <returns>The model state object</returns>
        public IModelState GetOrBuildModelState(IModel model)
        {
            var stateModel = model as IStateModel;
            if (stateModel != null)
            {
                if (stateModel.State != null)
                {
                    return stateModel.State;
                }
                return stateModel.State = BuildModelState();
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            return nextWorker.GetOrBuildModelState(model);
        }

        public void SetModelState(IModel model, IModelState state)
        {
            var stateModel = model as IStateModel;
            if (stateModel != null)
            {
                stateModel.State = state;
                return;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            nextWorker.SetModelState(model, state);
        }

        public void RemoveModelState(IModel model)
        {
            var stateModel = model as IStateModel;
            if (stateModel != null)
            {
                stateModel.State = null;
                return;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            nextWorker.RemoveModelState(model);
        }

        public void SetModelStateChanged(IModel model, bool changed)
        {
            var stateModel = model as IStateModel;
            if (stateModel != null)
            {
                stateModel.StateChanged = changed;
            }
            else
            {
                // We assume a fall-back state manager is always available
                var nextWorker = Factory.Resolve<IModelStateManager>(this);
                nextWorker.SetModelStateChanged(model, changed);
            }

            if (changed)
            {
                var modelManager = Factory.Resolve<IModelManager>();
                var args = new ModelChangedEventArgs(model, this);  // the args reference the object itself stating that this is a state change
                modelManager.OnModelChanged(this, args);
            }
        }

        public bool GetModelStateChanged(IModel model)
        {
            var stateModel = model as IStateModel;
            if (stateModel != null)
            {
                return stateModel.StateChanged;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            return nextWorker.GetModelStateChanged(model);
        }

        #endregion

        #region IVmModelStateManager members

        #region ICoreStateManager members

        public bool IsViewRetained(IModel model)
        {
            var modelState = GetModelState(model);
            var coreState = modelState as ICoreState;
            return coreState != null && coreState.IsViewRetained;
        }

        public void SetIsViewRetained(IModel model, bool isRetained)
        {
            var modelState = GetOrBuildModelState(model);
            var coreState = modelState as ICoreState;
            if (coreState != null)
            {
                coreState.IsViewRetained = isRetained;
            }
        }

        #endregion

        protected abstract IModelState BuildModelState();

        public bool IsViewModelRetained(IModel model)
        {
            var modelState = GetModelState(model);
            var vmModelState = modelState as IVmModelState;
            return vmModelState != null && vmModelState.IsViewModelRetained;
        }

        public void SetIsViewModelRetained(IModel model, bool isRetained)
        {
            var modelState = GetOrBuildModelState(model);
            var vmModelState = modelState as IVmModelState;
            if (vmModelState != null)
            {
                vmModelState.IsViewModelRetained = isRetained;
            }
        }

        #endregion

        #endregion
    }
}
