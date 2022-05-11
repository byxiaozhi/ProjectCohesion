using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Collections.Generic;
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
    [ContentProperty("ItemsSource")]
    public partial class ComboBox : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(ComboBox), new PropertyMetadata(PropertyChanged));
        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), typeof(int), typeof(ComboBox), new PropertyMetadata(PropertyChanged));
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(ComboBox), new PropertyMetadata(PropertyChanged));
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        Windows.UI.Xaml.Controls.ComboBox comboBox;

        public event Windows.UI.Xaml.Controls.SelectionChangedEventHandler SelectionChanged;

        public ComboBox()
        {
            InitializeComponent();
        }

        private void AppXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var windowsXamlHost = sender as WindowsXamlHost;
            comboBox = windowsXamlHost.GetUwpInternalObject() as Windows.UI.Xaml.Controls.ComboBox;
            if (comboBox != null)
            {
                comboBox.ItemsSource = ItemsSource;
                comboBox.SelectedIndex = SelectedIndex;
                comboBox.SelectedItem = SelectedItem;
                comboBox.SelectionChanged += ComboBox_SelectionChanged;
            }
        }

        private void ComboBox_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            SelectedItem = comboBox.SelectedItem;
            SelectedIndex = comboBox.SelectedIndex;
            SelectionChanged?.Invoke(this, e);
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var comboBox = ((ComboBox)d).comboBox;
            if (comboBox == null) return;
            if(e.Property == ItemsSourceProperty && comboBox.ItemsSource != e.NewValue)
                comboBox.ItemsSource = e.NewValue;
            if(e.Property == SelectedIndexProperty && comboBox.SelectedIndex != (int)e.NewValue)
                comboBox.SelectedIndex = (int)e.NewValue;
            if(e.Property == SelectedItemProperty && comboBox.SelectedItem != e.NewValue)
                comboBox.SelectedItem = e.NewValue;
        }
    }
}
