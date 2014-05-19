using Plato.Models;

namespace Plato.ViewModels
{
    public interface IModelToViewModel
    {
        #region Methods

        /// <summary>
        ///  Returns the view model that serves for the specified model
        /// </summary>
        /// <param name="model">The model to retrieve view model for</param>
        /// <returns>The view model that's associated with the model if any or null</returns>
        BaseViewModel GetViewModel(IModel model);

        /// <summary>
        ///  Returns a view model for the specified model from the dictionary (cached) or creates one if unavailable
        /// </summary>
        /// <param name="model">The model to return a view model for</param>
        /// <returns>The view model for the model</returns>
        BaseViewModel GetOrCreateViewModel(IModel model);

        /// <summary>
        ///  Detaches a view model from the model
        /// </summary>
        /// <param name="model">The model to detach the view model from</param>
        void RemoveViewModel(IModel model);

        #endregion
    }
}
