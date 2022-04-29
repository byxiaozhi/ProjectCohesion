using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Models.UIModels
{
    /// <summary>
    /// 主页顶部功能区分类集合
    /// </summary>
    public class ModuleSet
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模块描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 模块列表，内容为模块关键字
        /// </summary>
        public ObservableCollection<string> Modules { get; } = new();
    }
}
