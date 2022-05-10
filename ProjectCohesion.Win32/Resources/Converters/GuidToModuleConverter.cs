using Autofac;
using ProjectCohesion.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ProjectCohesion.Win32.Resources.Converters
{
    public class GuidToModuleConverter : IValueConverter
    {
        ModuleManager ModuleManager => Core.Autofac.Container.Resolve<ModuleManager>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ModuleManager.GetModule(value as Guid? ?? default);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
