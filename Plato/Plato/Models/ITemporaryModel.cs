using Plato.ModelProvisioning;

namespace Plato.Models
{
    public interface ITemporaryModel : IModel
    {
        #region Events

        /// <summary>
        ///  Event fired when the temporary model has changed and accepted by rendering manager
        ///  such as a view model manager to update the view
        /// </summary>
        /// <remarks>
        ///  As temporary model is not mananged by model provider, so in the case that it inherits a
        ///  real model, we can't call the model change handler on the model manager as usual, instead
        ///  we can fire this event and it will be routed to the view model side by the view model manager
        /// </remarks>
        event ModelChangedEvent TemporaryModelChanged;

        #endregion
    }
}
