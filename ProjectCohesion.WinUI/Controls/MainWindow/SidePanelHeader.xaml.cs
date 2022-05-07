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
    public sealed partial class SidePanelHeader : UserControl
    {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(SidePanelHeader), null);
        public object Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }


        public event RoutedEventHandler CloseClick;

        public SidePanelHeader()
        {
            this.InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseClick?.Invoke(this, e);
        }
    }
}
