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
        // Private methods ----------------------------------------------------

        private PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression ?? throw new ArgumentException("Expression is not member expression!");
            PropertyInfo propInfo = memberExpression.Member as PropertyInfo ?? throw new ArgumentException("Expression doesn't point to property!");
            return propInfo;
        }

        private void InternalSet<T>(ref T field, Expression<Func<T>> property, T value, bool force = false)
        {
            if (!Equals(field, value) || force)
            {
                field = value;
                OnPropertyChanged(property);
            }
        }

        // Protected methods --------------------------------------------------

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> property)
        {
            var propInfo = GetPropertyInfo(property);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propInfo.Name));
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

        protected void Set<T>(ref T field, T value, Expression<Func<T>> property, Action changeHandler = null, bool force = false)
        {
            if (!Equals(field, value) || force)
            {
                field = value;
                OnPropertyChanged(property);
                changeHandler?.Invoke();
            }
        }

        // Public properties --------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
