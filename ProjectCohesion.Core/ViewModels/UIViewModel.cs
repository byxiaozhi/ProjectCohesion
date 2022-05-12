using ProjectCohesion.Core.Models;
using ProjectCohesion.Core.Models.EventArgs;
using ProjectCohesion.Core.Modules;
using ProjectCohesion.Core.Reactive;
using ProjectCohesion.Core.Services;
using ProjectCohesion.Core.Utilities;
using ProjectCohesion.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.ViewModels
{
    /// <summary>
    /// 界面视图模型，存放界面布局、主题样式、语言设定、显示单位等界面相关数据
    /// </summary>
    public class UIViewModel : ReactiveObject
    {
        private readonly ModuleManager moduleManager;

        public UIViewModel(ModuleManager moduleManager)
        {
            this.moduleManager = moduleManager;
            SubscribeModuleRegistered();
            SubscribeModuleRemoved();
        }

        /// <summary>
        /// 当有新模块注册时更新UI
        /// </summary>
        private void SubscribeModuleRegistered()
        {
            var observable = moduleManager.Registered.Select(x => x.Module);
            ShouldDispose(observable.OfType<MenuModule>().Where(x => x.Type == "MainTopMenu").Subscribe(TopMenu.Items.Add));
            ShouldDispose(observable.OfType<GroupModule>().Where(x => x.Type == "MainLeftTab").Subscribe(LeftTabs.Items.Add));
            ShouldDispose(observable.OfType<GroupModule>().Where(x => x.Type == "MainRightTab").Subscribe(RightTabs.Items.Add));
        }

        /// <summary>
        /// 当有模块卸载时更新UI
        /// </summary>
        private void SubscribeModuleRemoved()
        {
            var observable = moduleManager.Removed.Select(x => x.Module);
            ShouldDispose(observable.OfType<MenuModule>().Where(x => x.Type == "MainTopMenu").Subscribe(x => TopMenu.Items.Remove(x)));
            ShouldDispose(observable.OfType<GroupModule>().Where(x => x.Type == "MainLeftTab").Subscribe(x => LeftTabs.Items.Remove(x)));
            ShouldDispose(observable.OfType<GroupModule>().Where(x => x.Type == "MainRightTab").Subscribe(x => RightTabs.Items.Remove(x)));
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
        public Selectable<MenuModule> TopMenu { get; } = new() { SelectedIndex = 0 };

        /// <summary>
        /// 主页顶部菜单是否折叠
        /// </summary>
        public bool TopMenuCollapsed { get; set; }

        /// <summary>
        /// 主页左侧标签页，
        /// 一般储存基础参数界面，
        /// 按照Order递增排序，
        /// 仅当Location为空或Location与当前内容标签页的Key相同时显示
        /// </summary>
        public Selectable<GroupModule> LeftTabs { get; } = new() { SelectedIndex = 0 };

        /// <summary>
        /// 主页右侧标签页，
        /// 一般储存专有参数界面，
        /// 按照Order递增排序，
        /// 仅当Location为空或Location与当前内容标签页的Key相同时显示
        /// </summary>
        public Selectable<GroupModule> RightTabs { get; } = new() { SelectedIndex = 0 };

        /// <summary>
        /// 主页内容标签页
        /// </summary>
        public Selectable<Guid?> ContentTabs { get; } = new() { SelectedIndex = -1 };

    }
}
