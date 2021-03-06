using Autofac;
using ProjectCohesion.Core.ViewModels;
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

namespace ProjectCohesion.Controls.MainWindow
{
    public partial class LeftPanel : UserControl
    {
        public LeftPanel()
        {
            InitializeComponent();
        }

        UIViewModel UIViewModel => Core.Autofac.Container.Resolve<UIViewModel>();

        private void SidePanelHeader_CloseClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UIViewModel.LeftTabs.Selected = null;
        }
    }
}
