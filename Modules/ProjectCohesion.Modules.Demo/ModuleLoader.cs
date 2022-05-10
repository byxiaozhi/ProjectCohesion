using ProjectCohesion.Win32.Interfaces;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using ProjectCohesion.Core.Services;
using System.Windows;
using ProjectCohesion.Win32.Modules;

namespace ProjectCohesion.Modules.Demo
{
    public class ModuleLoader : IModuleLoader
    {
        public void Initialize(IContainer container)
        {
            var moduleManager = container.Resolve<ModuleManager>();
            var uiManager = container.Resolve<UIManager>();
            Guid guid;

            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Menu.Project.Create() });
            uiManager.AddTopMenuItem("主页", "项目", guid);
            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Menu.Project.Open() });
            uiManager.AddTopMenuItem("主页", "项目", guid);
            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Menu.Project.Save() });
            uiManager.AddTopMenuItem("主页", "项目", guid);
            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Menu.Project.SaveAs() });
            uiManager.AddTopMenuItem("主页", "项目", guid);
            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Menu.UnitSetting() });
            uiManager.AddTopMenuItem("主页", "单位", guid);
            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Menu.Chart.Vertical() });
            uiManager.AddTopMenuItem("通用分析", "图表", guid);
            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Menu.Chart.Inclination() });
            uiManager.AddTopMenuItem("通用分析", "图表", guid);

            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Side.Left.Body() });
            uiManager.AddLeftTabsItem("井身", guid);
            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Side.Left.Trajectory() });
            uiManager.AddLeftTabsItem("井轨迹", guid);
            guid = moduleManager.RegisterModule(new UIModule() { Element = () => new Controls.Side.Right.Legend() });
            uiManager.AddRightTabsItem("图例", guid);

            moduleManager.RegisterModule(
                new Guid("7488D2C9-F8F2-456E-8FBB-7D65C9AE53B8"),
                new UIModule() { Name = "井轨迹", Element = () => new Pages.Inclination() });
            moduleManager.RegisterModule(
                new Guid("22D1F1F5-AA66-42AE-A5E0-401F7A9C5C46"),
                new UIModule() { Name = "垂直井段", Element = () => new Pages.Vertical() });
        }

        public void Destroy(IContainer container)
        {

        }
    }
}
