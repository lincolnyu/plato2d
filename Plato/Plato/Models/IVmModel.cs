using Plato.ViewModels;

namespace Plato.Models
{
    /// <summary>
    ///  Model that keeps a reference to its view model
    /// </summary>
    public interface IVmModel : IModel
    {
        #region Properties

        /// <summary>
        ///  Reference to the view model
        /// </summary>
        BaseViewModel ViewModel { get; set; }
        
        #endregion
    }
}
