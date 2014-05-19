using Plato.Models;

namespace Plato.StateManagement
{
    /// <summary>
    ///  Interface that entites should implement to claim the abilito of providing  
    ///  mapping form models to their corresponding state describing object
    /// </summary>
    public interface IModelStateManager
    {
        #region Methods

        /// <summary>
        ///  Retrieves the object that describes current state of the model
        /// </summary>
        /// <param name="model">The model to get the state of</param>
        /// <returns>The model state object</returns>
        IModelState GetModelState(IModel model);

        /// <summary>
        ///  Retrieves the state or builds one if doesn't exist
        /// </summary>
        /// <param name="model">The model to get the state of</param>
        /// <returns>The model state object</returns>
        IModelState GetOrBuildModelState(IModel model);

        /// <summary>
        ///  Assigns the specified state object to the mode if GetModelState() shows it lacks and requires one
        /// </summary>
        /// <param name="model">The model to assign a state to</param>
        /// <param name="state">the model state to assign to the model</param>
        void SetModelState(IModel model, IModelState state);

        /// <summary>
        ///  Remove the state object from the specified model
        /// </summary>
        /// <param name="model">The model to remove the state object from</param>
        void RemoveModelState(IModel model);

        /// <summary>
        ///  Mark the model's state has changed and not yet updated to presentation. This method should always be
        ///  manually called when the model's state has been changed either by assignment of a new state object or 
        ///  change to the state property obtained from the model; and called by the state change responder to 
        ///  dismiss the change flag
        /// </summary>
        /// <param name="model">The model whose whse state changed property needs to be set</param>
        /// <param name="changed">True to mark that the state has chnaged and not updated</param>
        void SetModelStateChanged(IModel model, bool changed);

        /// <summary>
        ///  Returns if the model's state has changed and not yet updated to presentation. This method is usually
        ///  called by the model responder to determine if the state has changed as part of or entirety of the 
        ///  latest model change
        /// </summary>
        /// <param name="model">The model whose whse state changed property needs to be retrieved</param>
        /// <returns>True if the state has chnaged and not updated</returns>
        bool GetModelStateChanged(IModel model);

        #endregion
    }
}
