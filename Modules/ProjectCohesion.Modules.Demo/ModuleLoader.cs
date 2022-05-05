using ProjectCohesion.Win32.Interfaces;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using ProjectCohesion.Core.Services;
using System.Windows;

namespace ProjectCohesion.Modules.Demo
{
    public class ModuleLoader : IModuleLoader
    {
        public void Initialize(IContainer container)
        {
            var moduleManager = container.Resolve<ModuleManager>();
            var uiManager = container.Resolve<UIManager>();
            Guid guid;

            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Menu.Project.Create() });
            uiManager.AddTopMenuItem("主页", "项目", guid);
            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Menu.Project.Open() });
            uiManager.AddTopMenuItem("主页", "项目", guid);
            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Menu.Project.Save() });
            uiManager.AddTopMenuItem("主页", "项目", guid);
            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Menu.Project.SaveAs() });
            uiManager.AddTopMenuItem("主页", "项目", guid);


            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Menu.UnitSetting() });
            uiManager.AddTopMenuItem("主页", "单位", guid);

            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Menu.Chart.Vertical() });
            uiManager.AddTopMenuItem("通用分析", "图表", guid);

            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Menu.Chart.Inclination() });
            uiManager.AddTopMenuItem("通用分析", "图表", guid);

            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Side.Left.Body() });
            uiManager.AddLeftTabsItem("井身", guid);

            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Side.Left.Trajectory() });
            uiManager.AddLeftTabsItem("井轨迹", guid);

            guid = moduleManager.RegisterModule(new Core.Module<Func<UIElement>>() { Element = () => new Controls.Side.Right.Legend() });
            uiManager.AddRightTabsItem("图例", guid);

        }

        public void Destroy(IContainer container)
        {

        }
    }
}
