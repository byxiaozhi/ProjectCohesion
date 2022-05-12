using ProjectCohesion.Win32.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectCohesion.Win32.Controls
{
    public partial class SidePanelTabView : ReactiveControl
    {
        public static readonly DependencyProperty MenuItemsProperty = RegisterProperty(nameof(MenuItems), typeof(SidePanelTabView));
        public object MenuItems { get => GetValue(MenuItemsProperty); set => SetValue(MenuItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = RegisterProperty(nameof(SelectedItem), typeof(SidePanelTabView));
        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        public event RoutedEventHandler SelectionChanged;

        public event RoutedEventHandler DoubleClick;

        public SidePanelTabView()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
