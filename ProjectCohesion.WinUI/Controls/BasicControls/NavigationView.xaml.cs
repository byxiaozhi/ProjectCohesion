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

        public event EventHandler ItemInvoked;

        public NavigationView()
        {
            InitializeComponent();
        }

        private void navView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            SelectedItem = sender.SelectedItem;
            ItemInvoked?.Invoke(this, null);
        }
    }
}
