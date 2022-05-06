using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ProjectCohesion.Win32.Resources.Converters
{
    public class EqualsConverter : IValueConverter
    {

        public bool IsReverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = value != null && value.ToString() == parameter.ToString();
            return IsReverse ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
