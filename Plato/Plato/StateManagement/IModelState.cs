namespace Plato.StateManagement
{
    public interface IModelState
    {
        #region Methods

        /// <summary>
        ///  Makes a clone of this state object
        /// </summary>
        /// <returns></returns>
        IModelState Clone();

        /// <summary>
        ///  merge the state data with others
        /// </summary>
        /// <param name="other">Data to merge from</param>
        void Merge(IModelState other);

        #endregion
    }
}
