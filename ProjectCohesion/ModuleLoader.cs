using ProjectCohesion.Win32.Interfaces;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectCohesion
{
    internal class ModuleLoader : IModuleLoader
    {
        readonly List<IModuleLoader> moduleLoaders = new();

        public ModuleLoader()
        {
            // 添加模块
            moduleLoaders.Add(new Modules.Demo.ModuleLoader());
        }

        public void Destroy(IContainer container)
        {
            // 销毁各个模块
            moduleLoaders.ForEach(module => module.Destroy(container));
        }

        public void Initialize(IContainer container)
        {
            // 初始化各个模块
            moduleLoaders.ForEach(module => module.Initialize(container));
        }
    }
}
