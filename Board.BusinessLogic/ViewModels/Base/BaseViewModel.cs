﻿using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.Types.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Base
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null, Action changeHandler = null, bool force = false)
        {
            if (!Equals(field, value) || force)
            {
                field = value;
                OnPropertyChanged(propertyName);
                changeHandler?.Invoke();
            }
        }

        protected void PropertyGroupChanged(string propertyGroupName)
        {
            foreach (var property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var notificationGroup = property.GetCustomAttribute<PropertyNotificationGroupAttribute>();
                if (notificationGroup != null && notificationGroup.GroupName == propertyGroupName)
                    OnPropertyChanged(property.Name);
            }
        }

        // Protected methods --------------------------------------------------

        protected void IterateProperties<TModel>(TModel model, Action<MemberInfo, PropertyInfo> action, ModelSyncDirection direction)
        {
            var thisType = GetType();
            var modelType = model.GetType();

            List<MemberInfo> properties = thisType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Cast<MemberInfo>().ToList();
            List<MemberInfo> fields = thisType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Cast<MemberInfo>().ToList();

            foreach (var member in properties.Concat(fields))
            {
                var boundAttribute = member.GetCustomAttribute<SyncWithModelAttribute>(true);
                if (boundAttribute != null && (direction == ModelSyncDirection.BothWays || boundAttribute.Direction == ModelSyncDirection.BothWays || boundAttribute.Direction == direction))
                {
                    var modelProperty = modelType.GetProperty(boundAttribute.ModelProperty, BindingFlags.Public | BindingFlags.Instance);
                    if (modelProperty != null)
                        action(member, modelProperty);
                    else
                        throw new InvalidOperationException($"Property {boundAttribute.ModelProperty} not found on object of type {modelType.Name}!");
                }
            }
        }

        /// <summary>
        /// Finds all properties decorated with <see cref="RepresentsModelPropertyAttribute"/>
        /// and updates them with values retrieved form specific properties of given model.
        /// </summary>
        protected virtual void UpdateFromModel<TModel>(TModel model)
        {
            IterateProperties(model, (MemberInfo thisProperty, PropertyInfo modelProperty) =>
            {
                var data = modelProperty.GetValue(model, null);

                if (thisProperty is PropertyInfo propertyInfo)
                    propertyInfo.SetValue(this, data, null);
                else if (thisProperty is FieldInfo fieldInfo)
                    fieldInfo.SetValue(this, data);
            }, ModelSyncDirection.FromModel);
        }

        /// <summary>
        /// Finds all properties decorated with <see cref="RepresentsModelPropertyAttribute"/>
        /// and updates specific model's properties with their values.
        /// </summary>
        /// <param name="model"></param>
        protected virtual void UpdateToModel<TModel>(TModel model)
        {
            IterateProperties(model, (MemberInfo thisProperty, PropertyInfo modelProperty) =>
            {
                object data = null;

                if (thisProperty is PropertyInfo propertyInfo)
                    data = propertyInfo.GetValue(this, null);
                else if (thisProperty is FieldInfo fieldInfo)
                    data = fieldInfo.GetValue(this);

                modelProperty.SetValue(model, data, null);
            }, ModelSyncDirection.ToModel);
        }

        // Public properties --------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
