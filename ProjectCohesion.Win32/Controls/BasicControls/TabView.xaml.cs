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

namespace ProjectCohesion.Win32.Controls
{
    public partial class TabView : UserControl
    {
        public TabView()
        {
            InitializeComponent();
        }

        private void AppXamlHost_ChildChanged(object sender, EventArgs e)
        {

        }
    }
}
