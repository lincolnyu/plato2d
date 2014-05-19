using Windows.UI.Xaml;

namespace Plato.ViewModels
{
    /// <summary>
    ///  Entity that's able to build views for view models
    /// </summary>
    public interface IViewBuilder
    {
        #region Methods

        /// <summary>
        ///  Builds a view that presents the specified view model mostly based on its type
        /// </summary>
        /// <param name="viewModel">The view model the view to present</param>
        /// <returns>The view</returns>
        /// <remarks>
        ///  NOTE: It is up to the builder implementor and its responsibility to attach 
        ///  the view to the view model by typically setting its DataContext to the view model
        /// </remarks>
        UIElement Build(BaseViewModel viewModel);

        #endregion
    }
}
