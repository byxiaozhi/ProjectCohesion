using ProjectCohesion.Win32.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectCohesion
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            WindowBackdrop.SetBackdrop(this);
            InitializeComponent();
        }

        private void btn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            btn.Text = "123";
            btn.IsEnabled = false;
        }
    }
}
