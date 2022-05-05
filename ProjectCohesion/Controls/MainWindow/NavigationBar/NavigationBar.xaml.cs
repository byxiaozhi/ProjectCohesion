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
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProjectCohesion.Controls.MainWindow
{
    public partial class NavigationBar : UserControl
    {
        readonly UIViewModel uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();

        Core.Modules.MenuModule prevSelected;

        public NavigationBar()
        {
            InitializeComponent();
        }

        private void navigationView_ItemInvoked(object sender, EventArgs e)
        {
            // 打开Popup
            popup.StaysOpen = true;
            popup.IsOpen = moduleGroupsWrapper.Visibility == Visibility.Collapsed && (prevSelected != uiViewModel.TopMenu.Selected || !popup.IsOpen);
            prevSelected = uiViewModel.TopMenu.Selected;
        }

        private void Popup_MouseMove(object sender, MouseEventArgs e)
        {
            // 当HitTest使用
            var point = e.GetPosition(navigationView);
            popup.StaysOpen = point.Y < navigationView.ActualHeight;
        }

        private void navigationView_DoubleClick(object sender, EventArgs e)
        {
            if (moduleGroupsWrapper.Visibility == Visibility.Visible)
            {
                moduleGroupsWrapper.Visibility = Visibility.Collapsed;
            }
            else
            {
                moduleGroupsWrapper.Visibility = Visibility.Visible;
                popup.IsOpen = false;
            }
        }

        Window Window => Window.GetWindow(this);

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window.LocationChanged += ClosePopup;
            Window.Deactivated += ClosePopup;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.LocationChanged -= ClosePopup;
            Window.Deactivated -= ClosePopup;
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            // 点击窗口时关闭Popup
            popup.IsOpen = false;
        }
    }
}
