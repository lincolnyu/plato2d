using Plato.Models;

namespace Plato.StateManagement
{
    /// <summary>
    ///  Model that has its state attached
    /// </summary>
    public interface IStateModel : IModel
    {
        #region Properties

        /// <summary>
        ///  The state of the model
        /// </summary>
        IModelState State { get; set; }

        /// <summary>
        ///  The state has changed
        /// </summary>
        bool StateChanged { get; set; }

        #endregion
    }
}
