using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core
{
    public class Module<T> where T : new()
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模块关键字，用于搜索模块
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 模块描述，描述模块用途等
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 模块位置，决定模块放置位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 模块顺序，在模块位置相同时通过此属性决定模块顺序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 模块元素
        /// </summary>
        public T Element { get; set; }
    }
}
