using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Collections.Generic;
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
    [ContentProperty("Text")]
    public partial class Button : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Button), new PropertyMetadata(PropertyChanged));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(Button), new PropertyMetadata(PropertyChanged));
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static new readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(Button), new PropertyMetadata(true, PropertyChanged));
        public new bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }

        public static new readonly DependencyProperty WidthProperty = DependencyProperty.Register(nameof(Width), typeof(double), typeof(Button), new PropertyMetadata(PropertyChanged));
        public new double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public static new readonly DependencyProperty HeightProperty = DependencyProperty.Register(nameof(Height), typeof(double), typeof(Button), new PropertyMetadata(PropertyChanged));
        public new double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register(nameof(ButtonStyle), typeof(string), typeof(Button), new PropertyMetadata(PropertyChanged));
        public string ButtonStyle
        {
            get => (string)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        public event Windows.UI.Xaml.RoutedEventHandler Click;

        public Windows.UI.Xaml.Controls.Button ChildButton { get; private set; }
        WinUI.Controls.Button child;

        public Button()
        {
            InitializeComponent();
        }

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var windowsXamlHost = sender as WindowsXamlHost;
            child = (windowsXamlHost.GetUwpInternalObject() as WinUI.Controls.Button);
            ChildButton = child?.ChildButton;
            if (ChildButton != null)
            {
                ChildButton.Content = Text;
                ChildButton.Command = Command;
                ChildButton.IsEnabled = IsEnabled;
                if (Width > 0) ChildButton.Width = Width;
                if (Height > 0) ChildButton.Height = Height;
                if (ButtonStyle != null && child.Resources.ContainsKey(ButtonStyle)) ChildButton.Style = (Windows.UI.Xaml.Style)child.Resources[ButtonStyle];
                ChildButton.Click += (s, e) => Click?.Invoke(this, e);
            }
        }

        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = ((Button)d).ChildButton;
            var child = ((Button)d).child;
            if (button != null)
            {
                if (e.Property.Name == nameof(Text) && (string)button.Content != (string)e.NewValue)
                    button.Content = e.NewValue;
                if (e.Property.Name == nameof(Command) && button.Command != (ICommand)e.NewValue)
                    button.Command = (ICommand)e.NewValue;
                if (e.Property.Name == nameof(IsEnabled) && button.IsEnabled != (bool)e.NewValue)
                    button.IsEnabled = (bool)e.NewValue;
                if (e.Property.Name == nameof(Width) && button.Width != (double)e.NewValue && (double)e.NewValue > 0)
                    button.Width = (double)e.NewValue;
                if (e.Property.Name == nameof(Height) && button.Height != (double)e.NewValue && (double)e.NewValue > 0)
                    button.Height = (double)e.NewValue;
                if (e.Property.Name == nameof(ButtonStyle) && e.NewValue != null && child.Resources.ContainsKey((string)e.NewValue))
                    button.Style = (Windows.UI.Xaml.Style)child.Resources[(string)e.NewValue];
            }
        }
    }
}
