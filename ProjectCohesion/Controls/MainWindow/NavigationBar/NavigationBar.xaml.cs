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
using System.Reactive.Linq;
using ProjectCohesion.Win32.Controls;

namespace ProjectCohesion.Controls.MainWindow
{
    public partial class NavigationBar : ReactiveControl
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

        private void navigationView_ItemInvoked(object sender, object args)
        {
            IsOpen = uiViewModel.TopMenu.Selected != null && uiViewModel.TopMenuCollapsed && (prevSelected != uiViewModel.TopMenu.Selected || !IsOpen);
            prevSelected = uiViewModel.TopMenu.Selected;
        }

        private void navigationView_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            uiViewModel.TopMenuCollapsed = !uiViewModel.TopMenuCollapsed;
            if (uiViewModel.TopMenuCollapsed)
                uiViewModel.TopMenu.Selected = null;
            IsOpen = false;
        }

        Window Window => Window.GetWindow(this);

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var observable1 = Observable.FromEventPattern(h => Window.Deactivated += h, h => Window.Deactivated -= h);
            var observable2 = eventCenter.GetObservable().Where(x => x.Name == "WM_MOUSEACTIVATE" || x.Name == "WM_LBUTTONDOWN" || x.Name == "WM_RBUTTONDOWN");
            ShouldDispose(Observable.Merge<object>(observable1, observable2).Where(x => IsOpen).Subscribe(x => MouseEvent()));
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        private void MouseEvent()
        {
            var screenPoint = User32.GetCursorPos();
            var point = TranslatePoint(PointFromScreen(new Point(screenPoint.x, screenPoint.y)), this);
            var rect = new Rect(8, 0, ActualWidth - 16, ActualHeight + moduleGroupsWrapper.ActualHeight - 24);
            if (!rect.Contains(point))
                ClosePopup(null, null);
        }

        private void ClosePopup(object sender, EventArgs e)
        {
            IsOpen = false;
            uiViewModel.TopMenu.Selected = null;
            prevSelected = null;
        }

    }
}
