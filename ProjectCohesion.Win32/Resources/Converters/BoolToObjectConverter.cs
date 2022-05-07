using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ProjectCohesion.Win32.Resources.Converters
{
    public class BoolToObjectConverter : Freezable, IValueConverter
    {
        public static readonly DependencyProperty TrueValueProperty = DependencyProperty.Register(nameof(TrueValue), typeof(object), typeof(BoolToObjectConverter), null);
        public object TrueValue
        {
            get => GetValue(TrueValueProperty);
            set => SetValue(TrueValueProperty, value);
        }

        public static readonly DependencyProperty FalseValueProperty = DependencyProperty.Register(nameof(FalseValue), typeof(object), typeof(BoolToObjectConverter), null);
        public object FalseValue
        {
            get => GetValue(FalseValueProperty);
            set => SetValue(FalseValueProperty, value);
        }

        private readonly ObjectToBoolConverter objectToBoolConverter = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = (bool)objectToBoolConverter.Convert(value, targetType, parameter, culture);
            return result ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BoolToObjectConverter();
        }
    }
}
