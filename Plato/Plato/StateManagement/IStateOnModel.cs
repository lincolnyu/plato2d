using Plato.Models;

namespace Plato.StateManagement
{
    /// <summary>
    ///  
    /// </summary>
    public interface IStateOnModel : IModel, IModelState
    {
        #region Methods

        /// <summary>
        ///  Indicating if the state has changed
        /// </summary>
        bool StateChanged
        {
            get; set;
        }

        #endregion
    }
}
