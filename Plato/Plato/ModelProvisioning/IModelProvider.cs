using System.Collections.ObjectModel;
using Plato.Models;

namespace Plato.ModelProvisioning
{
    /// <summary>
    ///  Model manager from rendering manager's perspective
    /// </summary>
    public interface IModelProvider
    {
        #region Properties

        /// <summary>
        ///  All the models that should be on screen or maintain user interation
        /// </summary>
        ObservableCollection<IModel> ModelsOnScreen { get; }
        
        #endregion

        #region Events

        /// <summary>
        ///  Notifies the model consumer (rendering unit(s)) of the change to a model
        /// </summary>
        event ModelChangedEvent ModelChanged;

        #endregion
    }
}
