using Autofac;
using ProjectCohesion.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ProjectCohesion.WinUI.Resources.Converters
{
    public class GuidToModuleConverter: IValueConverter
    {
        ModuleManager ModuleManager => Core.Autofac.Container.Resolve<ModuleManager>();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ModuleManager.GetModule(value as Guid? ?? default);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
