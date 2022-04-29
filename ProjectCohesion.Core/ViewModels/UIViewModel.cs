using ProjectCohesion.Core.Models.UIModels;
using ProjectCohesion.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.ViewModels
{
    /// <summary>
    /// 界面视图模型，存放界面布局、主题样式、语言设定、显示单位等界面相关数据
    /// </summary>
    public class UIViewModel : ViewModel
    {
        /// <summary>
        /// 当前主题
        /// </summary>
        public Themes Theme { get; set; }

        /// <summary>
        /// 主页顶部菜单
        /// </summary>
        public Selectable<Menu> TopMenu { get; } = new();

        /// <summary>
        /// 主页左侧标签页
        /// </summary>
        public Selectable<SideTab> LeftTabs { get; } = new();

        /// <summary>
        /// 主页右侧标签页
        /// </summary>
        public Selectable<SideTab> RightTabs { get; } = new();
    }
}
