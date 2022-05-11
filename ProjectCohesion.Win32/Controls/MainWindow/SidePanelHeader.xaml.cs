using Microsoft.Toolkit.Wpf.UI.XamlHost;
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
    public partial class SidePanelHeader : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(SidePanelHeader), null);
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public event Windows.UI.Xaml.RoutedEventHandler CloseClick;

        WinUI.Controls.SidePanelHeader sidePanelHeader;

        private readonly PropertyBridge propertyBridge = new();

        public SidePanelHeader()
        {
            InitializeComponent();
        }

        private void AppXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var windowsXamlHost = sender as WindowsXamlHost;
            sidePanelHeader = windowsXamlHost.GetUwpInternalObject() as WinUI.Controls.SidePanelHeader;
            if (sidePanelHeader != null)
            {
                sidePanelHeader.CloseClick += CloseClick;
                propertyBridge.OneWayBinding(sidePanelHeader, WinUI.Controls.SidePanelHeader.TitleProperty, this, TitleProperty);
            }
        }
    }
}
