using Microsoft.Toolkit.Wpf.UI.XamlHost;
using ProjectCohesion.Core.Services;
using ProjectCohesion.Win32.Utilities;
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
    public partial class TabView : ReactiveControl
    {
        public static readonly DependencyProperty TabItemsProperty = RegisterProperty(nameof(TabItems), typeof(TabView));
        public object TabItems { get => GetValue(TabItemsProperty); set => SetValue(TabItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = RegisterProperty(nameof(SelectedItem), typeof(TabView));
        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        public event Windows.UI.Xaml.Controls.SelectionChangedEventHandler SelectionChanged;

        public event EventHandler<object> TabCloseRequested;

        public TabView()
        {
            InitializeComponent();
        }

        private WinUI.Controls.TabView tabView;

        private readonly PropertyBridge propertyBridge = new();

        private void AppXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var windowsXamlHost = sender as WindowsXamlHost;
            tabView = windowsXamlHost.GetUwpInternalObject() as WinUI.Controls.TabView;
            if (tabView != null)
            {
                propertyBridge.TwoWayBinding(tabView, WinUI.Controls.TabView.TabItemsProperty, this, TabItemsProperty);
                propertyBridge.TwoWayBinding(tabView, WinUI.Controls.TabView.SelectedItemProperty, this, SelectedItemProperty);
                tabView.SelectionChanged += SelectionChanged;
                tabView.TabCloseRequested += TabCloseRequested;
            }
        }

    }
}
