using Plato.Models;

namespace Plato.ModelProvisioning
{
    /// <summary>
    ///  The event arguments that give the details of a model changed event
    /// </summary>
    public class ModelChangedEventArgs
    {
        #region Constructors

        /// <summary>
        ///  Instantiates the model change event arguments object
        /// </summary>
        /// <param name="changedModel">The model that has changed</param>
        /// <param name="userData">The user data added to the event arguments</param>
        public ModelChangedEventArgs(IModel changedModel, object userData)
        {
            ChangedModel = changedModel;
            UserData = userData;
        }

        #endregion

        #region Properties

        /// <summary>
        ///  The model that has changed for which this event is fired
        /// </summary>
        public IModel ChangedModel { get; private set; }

        /// <summary>
        ///  The user data attached that gives additional information about the change
        /// </summary>
        public object UserData { get; private set; }

        #endregion
    }
}
