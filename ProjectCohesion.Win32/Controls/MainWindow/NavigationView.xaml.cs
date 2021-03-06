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
using Windows.Foundation;
using Windows.UI.Xaml.Input;

namespace ProjectCohesion.Win32.Controls
{
    [ContentProperty("MenuItems")]
    public partial class NavigationView : ReactiveControl
    {

        public static readonly DependencyProperty MenuItemsProperty = RegisterProperty(nameof(MenuItems), typeof(NavigationView));
        public object MenuItems { get => GetValue(MenuItemsProperty); set => SetValue(MenuItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = RegisterProperty(nameof(SelectedItem), typeof(NavigationView));
        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        public event TypedEventHandler<WinUI.Controls.NavigationView, object> ItemInvoked;

        public event DoubleTappedEventHandler DoubleTapped;

        public NavigationView()
        {
            InitializeComponent();
        }

        private WinUI.Controls.NavigationView navigationView;

        private readonly PropertyBridge propertyBridge = new();

        private void AppXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var windowsXamlHost = sender as WindowsXamlHost;
            navigationView = windowsXamlHost.GetUwpInternalObject() as WinUI.Controls.NavigationView;
            if (navigationView != null)
            {
                navigationView.ItemInvoked += ItemInvoked;
                navigationView.DoubleTapped += DoubleTapped; ;
                navigationView.Loaded += NavigationView_Loaded;
                propertyBridge.OneWayBinding(navigationView, WinUI.Controls.NavigationView.MenuItemsProperty, this, MenuItemsProperty);
                if (Environment.OSVersion.Version.Build >= 22000)
                    navigationView.Background = null;
            }
        }

        private async void NavigationView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, () =>
            {
                propertyBridge.TwoWayBinding(navigationView, WinUI.Controls.NavigationView.SelectedItemProperty, this, SelectedItemProperty);
            });
        }
    }
}
