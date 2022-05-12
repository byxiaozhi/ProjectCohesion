using ProjectCohesion.Core.Models;
using ProjectCohesion.Core.Reactive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.ViewModels
{
    /// <summary>
    /// 根视图模型，作为DataContext提供给各个页面
    /// </summary>
    public class AppViewModel : ReactiveObject
    {
        public UIViewModel UIViewModel { get; }
        public ProjectViewModel ProjectViewModel { get; }

        public AppViewModel(UIViewModel uiViewModel, ProjectViewModel projectViewModel)
        {
            UIViewModel = uiViewModel;
            ProjectViewModel = projectViewModel;
        }
    }
}
