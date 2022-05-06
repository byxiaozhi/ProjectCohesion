using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ProjectCohesion.Win32.Resources.Converters
{
    public class BoolToObjectConverter : IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

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
    }
}
