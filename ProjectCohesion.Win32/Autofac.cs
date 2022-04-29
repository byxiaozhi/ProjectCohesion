using Autofac;
using ProjectCohesion.Win32.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Win32
{
    public static class Autofac
    {
        public static IContainer Container { get; }
        static Autofac()
        {
            var builder = new ContainerBuilder();
            RegisterServices(builder);
            Container = builder.Build();
        }

        /// <summary>
        /// 注册服务
        /// </summary>
        static public void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<ModuleManager>().SingleInstance();
            builder.RegisterType<UIManager>().SingleInstance();
        }
    }
}
