using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using KaVE.Utils.Reflection;

namespace KaVE.VsFeedbackGenerator.SessionManager
{
    public abstract class ViewModelBase<T> : INotifyPropertyChanged
    {
        private readonly IDictionary<object, string> _propertyNameDictionary =
            new Dictionary<object, string>();

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            if (!_propertyNameDictionary.ContainsKey(expression))
            {
                var propertyName = TypeExtensions<T>.GetPropertyName(expression);
                _propertyNameDictionary.Add(expression, propertyName);
            }

            OnPropertyChanged(_propertyNameDictionary[expression]);
        }
    }
}