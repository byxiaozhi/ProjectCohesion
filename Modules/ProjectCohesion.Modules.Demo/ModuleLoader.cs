using ProjectCohesion.Win32.Interfaces;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using ProjectCohesion.Core.Services;

namespace ProjectCohesion.Modules.Demo
{
    public class ModuleLoader : IModuleLoader
    {
        public void Initialize(IContainer container)
        {
            var moduleManager = container.Resolve<ModuleManager>();
            var uiManager = container.Resolve<UIManager>();
            Guid guid;

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Menu.Project.Open) });
            uiManager.AddTopMenuItem("主页", "项目", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Menu.Project.Create) });
            uiManager.AddTopMenuItem("主页", "项目", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Menu.Test.ButtonTest) });
            uiManager.AddTopMenuItem("主页", "测试", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Menu.Test.SelectTest) });
            uiManager.AddTopMenuItem("主页", "测试", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Menu.Test.CheckedBox) });
            uiManager.AddTopMenuItem("主页", "测试", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Menu.Chart.Vertical) });
            uiManager.AddTopMenuItem("通用分析", "图表", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Menu.Chart.Inclination) });
            uiManager.AddTopMenuItem("通用分析", "图表", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Side.Left.Body) });
            uiManager.AddLeftTabsItem("井身", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Side.Left.Trajectory) });
            uiManager.AddLeftTabsItem("井轨迹", guid);

            guid = Guid.NewGuid();
            moduleManager.RegisterModule(guid, new Core.Module<Type>() { Element = typeof(Controls.Side.Right.Legend) });
            uiManager.AddRightTabsItem("图例", guid);

        }

        public void Destroy(IContainer container)
        {

        }
    }
}
