using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Plato.Annotations;
using Plato.ModelProvisioning;
using Plato.Models;

namespace Plato.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Constructors

        protected BaseViewModel(IModel model)
        {
            Model = model;
        }

        #endregion

        #region Properties

        public IModel Model
        {
            get; private set;
        }

        /// <summary>
        ///  A reference to the view
        /// </summary>
        public virtual UIElement View
        {
            get; set;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        ///  Notifies the listener of the change of the specified property
        /// </summary>
        /// <param name="propertyName">The property that has changed</param>
        /// <remarks>
        ///  In this canvas MVVM programming pattern, it's not often that the view model 
        ///  property has a setter for, as such a setter's main purposes, either
        ///   - the modelling or actioning entities to change (instead they normally work directly on models)
        ///   - or for the view to change (as this is not quite the case for a canvas view object)
        ///  Thereofre this method is likely to be invoked in responds to model change
        ///  and the auto completion feature the attribute provides is not so useful
        /// </remarks>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///  Method that handles the event fired when the model has changed
        /// </summary>
        /// <param name="sender">The entity that fired the change envent</param>
        /// <param name="args">The arguments that contain the model and the user data etc</param>
        /// <remarks>
        ///  This may not be bound to an event from the associated model; instead it could be some
        ///  handler or entity that has changed the model that calls this method
        /// </remarks>
        public virtual void OnModelChanged(object sender, ModelChangedEventArgs args)
        {
        }

        #endregion
    }
}
