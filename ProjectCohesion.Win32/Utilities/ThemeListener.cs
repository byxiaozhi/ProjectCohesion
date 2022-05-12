using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.ViewManagement;

namespace ProjectCohesion.Win32.Utilities
{
    /// <summary>
    /// 监听系统主题变化
    /// </summary>
    public static class ThemeListener
    {
        public delegate void ThemeChangedHandler();

        private static readonly UISettings uiSettings = new();
        public static bool IsDarkMode { get; private set; }

        public static ThemeChangedHandler ThemeChanged;

        /// <summary>
        /// 从注册表获取当前是否浅色主题
        /// </summary>
        private static bool GetUseLightTheme()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var registryValueObject = key?.GetValue("AppsUseLightTheme");
            return registryValueObject == null || (int)registryValueObject > 0;
        }

        static ThemeListener()
        {
            uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
            UiSettings_ColorValuesChanged(uiSettings, null);
        }

        /// <summary>
        /// 主题变化回调
        /// </summary>
        private static void UiSettings_ColorValuesChanged(UISettings sender, object args)
        {
            var isDarkMode = !GetUseLightTheme();
            if (isDarkMode != IsDarkMode)
            {
                IsDarkMode = isDarkMode;
                ThemeChanged?.Invoke();
            }
        }
    }
}
