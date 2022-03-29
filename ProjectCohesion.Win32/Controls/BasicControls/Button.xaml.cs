using Microsoft.Toolkit.Wpf.UI.XamlHost;
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

namespace ProjectCohesion.Win32.Controls.BasicControls
{
    public partial class Button : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Button), new PropertyMetadata(TextChanged));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(Button), new PropertyMetadata(CommandChanged));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static new readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(Button), new PropertyMetadata(true, IsEnableChanged));
        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public event Windows.UI.Xaml.RoutedEventHandler Click;

        WinUI.Controls.BasicControls.Button button;

        public Button()
        {
            InitializeComponent();
        }

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var windowsXamlHost = sender as WindowsXamlHost;
            button = windowsXamlHost.GetUwpInternalObject() as WinUI.Controls.BasicControls.Button;
            if (button != null)
            {
                button.Text = Text;
                button.Command = Command;
                button.IsEnabled = IsEnabled;
                button.Click += (s, e) => Click?.Invoke(this, e);
            }
        }

        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = ((Button)d).button;
            if (button != null)
            {
                button.Text = (string)e.NewValue;
            }
        }

        private static void CommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = ((Button)d).button;
            if (button != null)
            {
                button.Command = (ICommand)e.NewValue;
            }
        }

        private static void IsEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = ((Button)d).button;
            if (button != null)
            {
                button.IsEnabled = (bool)e.NewValue;
            }
        }
    }
}
