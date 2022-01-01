using Board.BusinessLogic.Types.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Base
{
    public class ModelEditorViewModel<TModel> : BaseViewModel
    {
        // Protected methods --------------------------------------------------

        protected void IterateProperties(TModel model, Action<PropertyInfo, PropertyInfo> action)
        {
            var thisType = GetType();
            var modelType = model.GetType();

            foreach (var property in thisType.GetProperties())
            {
                var boundAttribute = property.GetCustomAttribute<RepresentsModelPropertyAttribute>(true);
                if (boundAttribute != null)
                {
                    var modelProperty = modelType.GetProperty(boundAttribute.ModelProperty, BindingFlags.Public | BindingFlags.Instance);
                    if (modelProperty != null)
                        action(property, modelProperty);
                }
            }
        }

        /// <summary>
        /// Finds all properties decorated with <see cref="RepresentsModelPropertyAttribute"/>
        /// and updates them with values retrieved form specific properties of given model.
        /// </summary>
        protected virtual void UpdatePropertiesFromModel(TModel model)
        {
            IterateProperties(model, (PropertyInfo thisProperty, PropertyInfo modelProperty) =>
            {
                var data = modelProperty.GetValue(model, null);
                thisProperty.SetValue(this, data, null);
            });
        }

        /// <summary>
        /// Finds all properties decorated with <see cref="RepresentsModelPropertyAttribute"/>
        /// and updates specific model's properties with their values.
        /// </summary>
        /// <param name="model"></param>
        protected virtual void UpdatePropertiesToModel(TModel model)
        {
            IterateProperties(model, (PropertyInfo thisProperty, PropertyInfo modelProperty) =>
            {
                var data = thisProperty.GetValue(this, null);
                modelProperty.SetValue(model, data, null);
            });
        }
    }
}
