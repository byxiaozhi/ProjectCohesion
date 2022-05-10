using ProjectCohesion.Core.Models;
using ProjectCohesion.Core.Modules;
using ProjectCohesion.Core.Services;
using ProjectCohesion.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.ViewModels
{
    /// <summary>
    /// 界面视图模型，存放界面布局、主题样式、语言设定、显示单位等界面相关数据
    /// </summary>
    public class UIViewModel : ViewModel, IDisposable
    {
        private readonly ModuleManager moduleManager;

        public UIViewModel(ModuleManager moduleManager)
        {
            this.moduleManager = moduleManager;

            // 监听模块更新
            moduleManager.Registered += ModuleManager_Registered;
            moduleManager.Removed += ModuleManager_Removed;
        }

        public void Dispose()
        {
            // 移除监听模块更新
            moduleManager.Registered -= ModuleManager_Registered;
            moduleManager.Removed -= ModuleManager_Removed;
        }

        /// <summary>
        /// 当有模块卸载时更新UI
        /// </summary>
        private void ModuleManager_Removed(object sender, Models.EventArgs.ModuleEventArgs e)
        {
            if (e.Module is MenuModule menuModule && menuModule.Type == "MainTopMenu")
                TopMenu.Items.Remove(menuModule);
            else if (e.Module is GroupModule groupModule)
                if (groupModule.Type == "MainLeftTab")
                    LeftTabs.Items.Remove(groupModule);
                else if (groupModule.Type == "MainRightTab")
                    RightTabs.Items.Remove(groupModule);
        }

        /// <summary>
        /// 当有新模块注册时更新UI
        /// </summary>
        private void ModuleManager_Registered(object sender, Models.EventArgs.ModuleEventArgs e)
        {
            if (e.Module is MenuModule menuModule && menuModule.Type == "MainTopMenu")
            {
                TopMenu.Items.Add(menuModule);
                if (TopMenu.Selected == null)
                    TopMenu.Selected = menuModule;
            }
            else if (e.Module is GroupModule groupModule)
                if (groupModule.Type == "MainLeftTab")
                {
                    LeftTabs.Items.Add(groupModule);
                    if (LeftTabs.Selected == null)
                        LeftTabs.Selected = groupModule;
                }
                else if (groupModule.Type == "MainRightTab")
                {
                    RightTabs.Items.Add(groupModule);
                    if (RightTabs.Selected == null)
                        RightTabs.Selected = groupModule;
                }
        }

        /// <summary>
        /// 当前主题
        /// </summary>
        public Themes Theme { get; set; }

        /// <summary>
        /// 主页顶部菜单，
        /// 按照Order递增排序，
        /// 仅储存菜单元素，点击等事件需要组件自行处理
        /// </summary>
        public Selectable<MenuModule> TopMenu { get; } = new();

        /// <summary>
        /// 主页左侧标签页，
        /// 一般储存基础参数界面，
        /// 按照Order递增排序，
        /// 仅当Location为空或Location与当前内容标签页的Key相同时显示
        /// </summary>
        public Selectable<GroupModule> LeftTabs { get; } = new();

        /// <summary>
        /// 主页右侧标签页，
        /// 一般储存专有参数界面，
        /// 按照Order递增排序，
        /// 仅当Location为空或Location与当前内容标签页的Key相同时显示
        /// </summary>
        public Selectable<GroupModule> RightTabs { get; } = new();

        /// <summary>
        /// 主页内容标签页
        /// </summary>
        public Selectable<Guid?> ContentTabs { get; } = new();

    }
}
