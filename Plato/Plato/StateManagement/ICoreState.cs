namespace Plato.StateManagement
{
    public interface ICoreState : IModelState
    {
        #region Properties

        /// <summary>
        ///  Should the model provider retain the model on screen
        /// </summary>
        /// <remarks>
        ///  This normally happens when the object is involved in user interaction
        ///  like when it's being edited and not necessarily on the canvas
        /// </remarks>
        bool IsViewRetained
        {
            get; set;
        }

        #endregion
    }
}
