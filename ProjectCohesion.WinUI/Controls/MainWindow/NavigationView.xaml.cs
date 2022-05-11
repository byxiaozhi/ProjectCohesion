using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace ProjectCohesion.WinUI.Controls
{
    public sealed partial class NavigationView : UserControl
    {
        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register(nameof(MenuItems), typeof(object), typeof(NavigationView), null);
        public object MenuItems { get => GetValue(MenuItemsProperty); set => SetValue(MenuItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(NavigationView), null);
        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        public event TypedEventHandler<NavigationView, object> ItemInvoked;

        public new event DoubleTappedEventHandler DoubleTapped;

        public NavigationView()
        {
            InitializeComponent();
        }

        private void NavigationViewItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SelectedItem = (sender as Microsoft.UI.Xaml.Controls.NavigationViewItem).Tag;
            ItemInvoked?.Invoke(this, SelectedItem);
        }
    }
}
