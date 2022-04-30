using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ProjectCohesion.Win32.Interfaces
{
    public interface IModuleLoader
    {
        /// <summary>
        /// 模块初始化
        /// </summary>
        void Initialize(Autofac.IContainer container);

        /// <summary>
        /// 模块卸载
        /// </summary>
        void Destroy(Autofac.IContainer container);
    }
}
