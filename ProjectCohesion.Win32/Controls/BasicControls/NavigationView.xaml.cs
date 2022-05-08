using Microsoft.Toolkit.Wpf.UI.XamlHost;
using ProjectCohesion.Core.Modules;
using ProjectCohesion.Win32.Utilities;
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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectCohesion.Win32.Controls
{
    [ContentProperty("MenuItems")]
    public partial class NavigationView : UserControl
    {

        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register(nameof(MenuItems), typeof(object), typeof(NavigationView), null);
        public object MenuItems { get => GetValue(MenuItemsProperty); set => SetValue(MenuItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(NavigationView), null);
        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        public event EventHandler ItemInvoked;

        public event RoutedEventHandler DoubleClick;

        public NavigationView()
        {
            InitializeComponent();
        }

        WinUI.Controls.NavigationView navigationView;

        private PropertyBridge propertyBridge = new();

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var windowsXamlHost = sender as WindowsXamlHost;
            navigationView = windowsXamlHost.GetUwpInternalObject() as WinUI.Controls.NavigationView;
            if (navigationView != null)
            {
                navigationView.ItemInvoked += NavigationView_ItemInvoked;
                navigationView.DoubleClick += NavigationView_DoubleClick; ;
                navigationView.Loaded += NavigationView_Loaded;
                propertyBridge.OneWayBinding(navigationView, WinUI.Controls.NavigationView.MenuItemsProperty, this, MenuItemsProperty);
                if (Environment.OSVersion.Version.Build >= 22000)
                    navigationView.Background = null;
            }
        }

        private void NavigationView_DoubleClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DoubleClick?.Invoke(this, new RoutedEventArgs());
        }

        private void NavigationView_ItemInvoked(object sender, EventArgs e)
        {
            SelectedItem = navigationView.SelectedItem;
            ItemInvoked?.Invoke(this, EventArgs.Empty);
        }

        private async void NavigationView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(500);
            propertyBridge.TwoWayBinding(navigationView, WinUI.Controls.NavigationView.SelectedItemProperty, this, SelectedItemProperty);
        }
    }
}
