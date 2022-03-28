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
    public class UIViewModel : ViewModelBase
    {
        public Models.UIModels.Themes Theme { get; set; }
    }
}
