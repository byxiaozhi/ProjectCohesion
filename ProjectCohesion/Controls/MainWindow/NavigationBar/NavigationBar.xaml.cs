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
using ProjectCohesion.Core.Services;
using PInvoke;

namespace ProjectCohesion.Controls.MainWindow
{
    public partial class NavigationBar : UserControl
    {
        readonly UIViewModel uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();
        readonly EventCenter eventCenter = Core.Autofac.Container.Resolve<EventCenter>();

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(NavigationBar), null);
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        Core.Modules.MenuModule prevSelected;

        public NavigationBar()
        {
            InitializeComponent();
        }

        private void navigationView_ItemInvoked(object sender, EventArgs e)
        {
            IsOpen = uiViewModel.TopMenuCollapsed && (prevSelected != uiViewModel.TopMenu.Selected || !IsOpen);
            prevSelected = uiViewModel.TopMenu.Selected;
        }

        private void navigationView_DoubleClick(object sender, EventArgs e)
        {
            uiViewModel.TopMenuCollapsed = !uiViewModel.TopMenuCollapsed;
            IsOpen = false;
        }

        Window Window => Window.GetWindow(this);

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Deactivated += ClosePopup;
            eventCenter.AddEventListener("WM_MOUSEACTIVATE", MouseEvent);
            eventCenter.AddEventListener("WM_LBUTTONDOWN", MouseEvent);
            eventCenter.AddEventListener("WM_RBUTTONDOWN", MouseEvent);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Window.Deactivated -= ClosePopup;
            eventCenter.RemoveEventListener("WM_MOUSEACTIVATE", MouseEvent);
            eventCenter.RemoveEventListener("WM_LBUTTONDOWN", MouseEvent);
            eventCenter.RemoveEventListener("WM_RBUTTONDOWN", MouseEvent);
        }

        private void MouseEvent(object sender, EventArgs e)
        {
            if (!IsOpen) return;
            var screenPoint = User32.GetCursorPos();
            var point = TranslatePoint(PointFromScreen(new Point(screenPoint.x, screenPoint.y)), this);
            if (point.X < 8 ||
                point.X > ActualWidth - 16 ||
                point.Y < 0 ||
                point.Y > ActualHeight + moduleGroupsWrapper.ActualHeight - 24)
                ClosePopup(sender, e);
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            if (!IsOpen) return;
            IsOpen = false;
            uiViewModel.TopMenu.Selected = null;
        }
    }
}
