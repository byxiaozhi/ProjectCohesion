using ProjectCohesion.Core.ViewModels;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;

namespace ProjectCohesion.Controls.MainWindow
{
    public partial class NavigationBar : UserControl
    {
        readonly UIViewModel uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();

        public NavigationBar()
        {
            InitializeComponent();
        }

        private void navigationView_ItemInvoked(object sender, EventArgs e)
        {

        }
    }
}
