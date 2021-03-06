using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;
using Windows.UI.ViewManagement;

namespace ProjectCohesion.Win32.Resources.Brushs
{
    public class SystemColorBrushs
    {
        private static readonly UISettings uiSettings = new();

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        public static SolidColorBrush Accent => ToBrush(uiSettings.GetColorValue(UIColorType.Accent));
        public static SolidColorBrush Foreground => ToBrush(uiSettings.GetColorValue(UIColorType.Foreground));
        public static SolidColorBrush Background => ToBrush(uiSettings.GetColorValue(UIColorType.Background));

        static SystemColorBrushs()
        {
            uiSettings.ColorValuesChanged += (s, e) => Update();
        }

        private static void Update()
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Accent)));
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Foreground)));
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Background)));
        }

        private static SolidColorBrush ToBrush(Windows.UI.Color color)
        {
            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
        }

    }
}
