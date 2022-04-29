using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ProjectCohesion.Core.Models.UIModels
{
    /// <summary>
    /// UI组件
    /// </summary>
    public class UIModule
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public ImageSource Icon { get; set; }

        public UIElement UIElement { get; set; }
        
    }
}
