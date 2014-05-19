using System.Collections.Generic;
using Plato.Models;

namespace Plato.StateManagement
{
    public abstract class ModelStateDictionary : IModelStateManager
    {
        #region Constructors

        public ModelStateDictionary()
        {
            ModelToStateModel = new Dictionary<IModel, IModelState>();
            ChangedModel = new HashSet<IModel>();
        }

        #endregion

        #region Methods

        #region Properties

        /// <summary>
        ///  The mapping from models to their corresponding view models
        /// </summary>
        private Dictionary<IModel, IModelState> ModelToStateModel
        {
            get;
            set;
        }

        private HashSet<IModel> ChangedModel
        {
            get; 
            set;
        }

        #endregion

        #region IModelStateManager

        public IModelState GetModelState(IModel model)
        {
            if (ModelToStateModel.ContainsKey(model))
            {
                return ModelToStateModel[model];
            }
            return null;
        }

        public abstract IModelState GetOrBuildModelState(IModel model);

        public void SetModelState(IModel model, IModelState state)
        {
            ModelToStateModel[model] = state;
        }

        public void RemoveModelState(IModel model)
        {
            ModelToStateModel.Remove(model);
        }

        public void SetModelStateChanged(IModel model, bool changed)
        {
            if (changed)
            {
                ChangedModel.Add(model);
            }
            else
            {
                if (ChangedModel.Contains(model))
                {
                    ChangedModel.Remove(model);
                }
            }
        }

        public bool GetModelStateChanged(IModel model)
        {
            return ChangedModel.Contains(model);
        }

        #endregion

        #endregion
    }
}
