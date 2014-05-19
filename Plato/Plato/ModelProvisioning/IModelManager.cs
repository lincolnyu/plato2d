using Plato.Models;

namespace Plato.ModelProvisioning
{
    /// <summary>
    ///  Model manager from model changers' perspective
    /// </summary>
    public interface IModelManager
    {
        #region Properties

        int ModelCount { get; }

        #endregion

        #region Methods

        /// <summary>
        ///  Notifies the model manager that the specified models have changed
        /// </summary>
        /// <param name="sender">The sender/source of the change event (normally the caller of the method but not necessarily)</param>
        /// <param name="args">The event arguments that detail the change including the model that has changed and a user defined data object</param>
        /// <remarks>
        ///  NOTE
        ///  A good design always has all model change initiators call this method.
        ///  Model manager and consequently rendering manager as a rule only uses this message to 
        ///  swap in and out view models and doesn't initiate detailed updates on corresponding view model,
        ///  which minimises the chance of cyclic update in case of a poor design that involves model 
        ///  originated updates.
        /// </remarks>
        void OnModelChanged(object sender, ModelChangedEventArgs args);

        #endregion
    }
}
