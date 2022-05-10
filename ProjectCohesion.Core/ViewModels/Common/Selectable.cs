using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.ViewModels.Common
{
    /// <summary>
    /// 可选视图模型
    /// </summary>
    public class Selectable<T> : ViewModel
    {
        private int selectedIndex = -1;


        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected => selectedIndex >= 0 && selectedIndex < Items.Count;

        /// <summary>
        /// 当前选中项
        /// </summary>
        public T Selected
        {
            get => IsSelected ? Items[SelectedIndex] : default;
            set => SelectedIndex = Items.IndexOf(value);
        }

        /// <summary>
        /// 当前选中索引
        /// </summary>
        public int SelectedIndex { get => IsSelected ? selectedIndex : -1; set => selectedIndex = value; }

        /// <summary>
        /// 可选项
        /// </summary>
        public ObservableCollection<T> Items { get; } = new();
    }
}
