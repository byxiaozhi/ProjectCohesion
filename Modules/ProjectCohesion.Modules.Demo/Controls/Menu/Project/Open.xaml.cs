using ProjectCohesion.Win32.AppWindows;
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

namespace ProjectCohesion.Modules.Demo.Controls.Menu.Project
{
    public partial class Open : UserControl
    {
        public Open()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContentDialog();
            dialog.Owner = Window.GetWindow(this);
            dialog.Title = "标题";
            dialog.Content = "对话框测试";
            dialog.PrimaryButtonText = "确定";
            dialog.PrimaryButtonStyle = "Accent";
            dialog.CloseButtonText = "取消";
            dialog.ShowDialog();
        }
    }
}
