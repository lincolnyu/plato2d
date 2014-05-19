using Plato.Models;
using Trollveggen;

namespace Plato.StateManagement
{
    /// <summary>
    ///  IModelStateManager implementation for models where state and model are one
    /// </summary>
    public class OnModelStateManager : IModelStateManager
    {
        /// <summary>
        ///  Retrieves the object that describes current state of the model
        /// </summary>
        /// <param name="model">The model to get the state of</param>
        /// <returns>The model state object</returns>
        public IModelState GetModelState(IModel model)
        {
            var modelState = model as IModelState;
            if (modelState != null)
            {
                return modelState;
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
            var modelState = model as IModelState;
            if (modelState != null)
            {
                return modelState;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            return nextWorker.GetOrBuildModelState(model);
        }

        /// <summary>
        ///  Assigns the specified state object to the mode if GetModelState() shows it lacks and requires one
        /// </summary>
        /// <param name="model">The model to assign a state to</param>
        /// <param name="state">the model state to assign to the model</param>
        public void SetModelState(IModel model, IModelState state)
        {
            var modelState = model as IModelState;
            if (modelState != null)
            {
                modelState.Merge(state);    // attempt to merge if this method is really called on a model that has embedded state
                return;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            nextWorker.SetModelState(model, state);
        }

        /// <summary>
        ///  Remove the state object from the specified model
        /// </summary>
        /// <param name="model">The model to remove the state object from</param>
        public void RemoveModelState(IModel model)
        {
            if (model is IModelState)
            {
                return;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            nextWorker.RemoveModelState(model);
        }

        /// <summary>
        ///  Mark the model's state has changed and not yet updated to presentation. This method should always be
        ///  manually called when the model's state has been changed either by assignment of a new state object or 
        ///  change to the state property obtained from the model; and called by the state change responder to 
        ///  dismiss the change flag
        /// </summary>
        /// <param name="model">The model whose whse state changed property needs to be set</param>
        /// <param name="changed">True to mark that the state has chnaged and not updated</param>
        public void SetModelStateChanged(IModel model, bool changed)
        {
            var stateOnModel = model as IStateOnModel;
            if (stateOnModel != null)
            {
                stateOnModel.StateChanged = changed;
                return;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            nextWorker.SetModelStateChanged(model, changed);
        }

        /// <summary>
        ///  Returns if the model's state has changed and not yet updated to presentation. This method is usually
        ///  called by the model responder to determine if the state has changed as part of or entirety of the 
        ///  latest model change
        /// </summary>
        /// <param name="model">The model whose whse state changed property needs to be retrieved</param>
        /// <returns>True if the state has chnaged and not updated</returns>
        public bool GetModelStateChanged(IModel model)
        {
            var stateOnModel = model as IStateOnModel;
            if (stateOnModel != null)
            {
                return stateOnModel.StateChanged;
            }

            // We assume a fall-back state manager is always available
            var nextWorker = Factory.Resolve<IModelStateManager>(this);
            return nextWorker.GetModelStateChanged(model);
        }
    }
}
