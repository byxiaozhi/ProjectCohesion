using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.ViewModels
{
    /// <summary>
    /// 视图模型基类
    /// </summary>
    public abstract class ViewModel: INotifyPropertyChanged
    {
#pragma warning disable CS0067
        // PropertyChanged.Fody 会自动监听属性变化并发出事件
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
