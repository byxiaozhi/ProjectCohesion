using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Models.UIModels
{
    /// <summary>
    /// 主页顶部菜单项
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 模块集合列表
        /// </summary>
        public ObservableCollection<ModuleSet> ModuleSets { get; } = new();
    }
}
