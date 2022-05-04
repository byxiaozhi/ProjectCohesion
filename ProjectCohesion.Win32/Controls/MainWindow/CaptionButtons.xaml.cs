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
    public partial class CaptionButtons : UserControl
    {
        public CaptionButtons()
        {
            InitializeComponent();
        }

        public Window Window => Window.GetWindow(this);

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Window.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            var maximized = Window.WindowState == WindowState.Maximized;
            Window.WindowState = maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }
    }
}
