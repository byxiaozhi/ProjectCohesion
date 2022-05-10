using System;
using System.Collections.Generic;
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
    public sealed partial class TabView : UserControl
    {

        public static readonly DependencyProperty TabItemsProperty = DependencyProperty.Register(nameof(TabItems), typeof(object), typeof(TabView), null);
        public object TabItems { get => GetValue(TabItemsProperty); set => SetValue(TabItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(TabView), null);
        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        public event SelectionChangedEventHandler SelectionChanged;

        public event EventHandler<object> TabCloseRequested;

        public TabView()
        {
            InitializeComponent();
        }

        private void TabView_TabCloseRequested(Microsoft.UI.Xaml.Controls.TabView sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        {
            TabCloseRequested?.Invoke(this, args.Item);
        }

        private void TabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
