using Autofac;
using ProjectCohesion.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core
{
    public static class Autofac
    {
        public static IContainer Container { get; }
        static Autofac()
        {
            var builder = new ContainerBuilder();
            RegisterViewModel(builder);
            Container = builder.Build();
        }

        /// <summary>
        /// 注册视图模型
        /// </summary>
        static public void RegisterViewModel(ContainerBuilder builder)
        {
            builder.RegisterType<AppViewModel>().SingleInstance();
            builder.RegisterType<UIViewModel>().SingleInstance();
            builder.RegisterType<ProjectViewModel>().SingleInstance();
        }
    }
}
