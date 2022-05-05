using Autofac;
using ProjectCohesion.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ProjectCohesion.Win32.Resources.Converters
{
    public class GuidToControlConverter : IValueConverter
    {
        ModuleManager ModuleManager => Core.Autofac.Container.Resolve<ModuleManager>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 生成一个控件对象
            return ((Core.Module<Func<UIElement>>)ModuleManager.GetModule((Guid)value)).Element();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
