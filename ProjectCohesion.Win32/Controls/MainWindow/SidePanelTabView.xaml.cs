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
    public partial class SidePanelTabView : UserControl
    {
        public static readonly DependencyProperty MenuItemsProperty = DependencyProperty.Register(nameof(MenuItems), typeof(object), typeof(SidePanelTabView), new PropertyMetadata(PropertyChanged));
        public object MenuItems { get => GetValue(MenuItemsProperty); set => SetValue(MenuItemsProperty, value); }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(SidePanelTabView), new PropertyMetadata(PropertyChanged));
        public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

        public event RoutedEventHandler ItemInvoked;

        public event RoutedEventHandler DoubleClick;

        private HashSet<ToggleButton> toggleButtons = new();

        public SidePanelTabView()
        {
            InitializeComponent();
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sidePanelTabView = d as SidePanelTabView;
            foreach (var toggleButton in sidePanelTabView.toggleButtons)
            {
                toggleButton.IsChecked = toggleButton.Tag == sidePanelTabView.SelectedItem;
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedItem = (sender as ToggleButton).Tag;
            (sender as ToggleButton).IsChecked = false;
            ItemInvoked?.Invoke(this, e);
        }

        private void ToggleButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DoubleClick?.Invoke(this, e);
        }

        private void ToggleButton_Loaded(object sender, RoutedEventArgs e)
        {
            toggleButtons.Add(sender as ToggleButton);
        }

        private void ToggleButton_Unloaded(object sender, RoutedEventArgs e)
        {
            toggleButtons.Remove(sender as ToggleButton);
        }
    }
}
