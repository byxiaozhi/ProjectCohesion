using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ProjectCohesion.Win32.Resources.Converters
{
    public class ObjectToBoolConverter : IValueConverter
    {

        public bool IsReverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;
            if (value == null)
                result = false;
            else if (value is bool flag)
                result = flag;
            else if (value is int num)
                result = num != 0;
            return IsReverse ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
