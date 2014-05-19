namespace Plato.ModelProvisioning
{
    /// <summary>
    ///  Model change event that's passed through the model provider to the renderer
    /// </summary>
    /// <param name="sender">The sender of the event, NOT necessary the model itself</param>
    /// <param name="args">The model changed event arguments that describe the change</param>
    public delegate void ModelChangedEvent(object sender, ModelChangedEventArgs args);
}
