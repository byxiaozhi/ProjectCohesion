using Microsoft.Toolkit.Wpf.UI.XamlHost;
using ProjectCohesion.Core.Modules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    public partial class NavigationView : UserControl
    {

        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register(nameof(MenuItems), typeof(object), typeof(NavigationView), new PropertyMetadata(PropertyChanged));
        public object MenuItems { get => GetValue(MenuItemsProperty); set => SetValue(MenuItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(NavigationView), new PropertyMetadata(PropertyChanged));
        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        public event EventHandler ItemInvoked;

        public event EventHandler DoubleClick;

        public NavigationView()
        {
            InitializeComponent();
        }

        WinUI.Controls.NavigationView navigationView;

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var windowsXamlHost = sender as WindowsXamlHost;
            navigationView = windowsXamlHost.GetUwpInternalObject() as WinUI.Controls.NavigationView;
            if (navigationView != null)
            {
                navigationView.ItemInvoked += NavigationView_ItemInvoked;
                navigationView.DoubleClick += DoubleClick;
                navigationView.Loaded += NavigationView_Loaded;
            }
        }

        private void NavigationView_ItemInvoked(object sender, EventArgs e)
        {
            SelectedItem = navigationView.SelectedItem;
            ItemInvoked?.Invoke(this, EventArgs.Empty);
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var navigationView = ((NavigationView)d).navigationView;
            if (navigationView == null) return;
            if (e.Property.Name == nameof(MenuItems) && navigationView.MenuItems != e.NewValue)
                navigationView.MenuItems = e.NewValue;
            else if (e.Property.Name == nameof(SelectedItem) && navigationView.SelectedItem != e.NewValue)
                navigationView.SelectedItem = e.NewValue;
        }

        private async void NavigationView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(500);
            navigationView.SelectedItem = null;
            navigationView.SelectedItem = SelectedItem;
        }
    }
}
