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
    public partial class Vertical : UserControl
    {
        public Vertical()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var tabsManager = Core.Autofac.Container.Resolve<ContentTabsManager>();
            tabsManager.ActivateTab(new Guid("22D1F1F5-AA66-42AE-A5E0-401F7A9C5C46"));
        }
    }
}
