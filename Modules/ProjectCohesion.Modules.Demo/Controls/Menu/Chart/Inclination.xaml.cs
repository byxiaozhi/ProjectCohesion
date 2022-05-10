using Autofac;
using ProjectCohesion.Core.Services;
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

namespace ProjectCohesion.Modules.Demo.Controls.Menu.Chart
{
    public partial class Inclination : UserControl
    {
        public Inclination()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tabsManager = Core.Autofac.Container.Resolve<ContentTabsManager>();
            tabsManager.ActivateTab(new Guid("7488D2C9-F8F2-456E-8FBB-7D65C9AE53B8"));
        }
    }
}
