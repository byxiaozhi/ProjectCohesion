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

namespace ProjectCohesion.Controls.MainWindow
{
    public partial class MainContent : UserControl
    {
        public MainContent()
        {
            InitializeComponent();
        }

        private void TabView_TabCloseRequested(object sender, object e)
        {
            var tabsManager = Core.Autofac.Container.Resolve<ContentTabsManager>();
            tabsManager.RemoveTab((Guid)e);
        }
    }
}
