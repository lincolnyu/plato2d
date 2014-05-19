namespace Plato.StateManagement
{
    public interface IVmModelState : ICoreState
    {
        bool IsViewModelRetained
        {
            get; set;
        }
    }
}
