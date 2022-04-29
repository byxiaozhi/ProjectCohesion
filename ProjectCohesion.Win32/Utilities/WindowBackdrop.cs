using Autofac;
using ProjectCohesion.Core.Models.UIModels;
using ProjectCohesion.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace ProjectCohesion.Win32.Utilities
{
    public static class WindowBackdrop
    {
        [Flags]
        public enum DwmWindowAttribute : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029,
            DWMWA_SYSTEMBACKDROP_TYPE = 38,
            DWMWA_CAPTION_COLOR = 35,
            DWMWA_TEXT_COLOR = 36
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref uint pvAttribute, int cbAttribute);

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        };

        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        private static HashSet<Window> windows = new();

        static WindowBackdrop()
        {
            ThemeListener.ThemeChanged += OnThemeChanged;
        }

        /// <summary>
        /// 设置窗口背景样式
        /// </summary>
        public static void SetBackdrop(Window window)
        {
            if (!windows.Contains(window))
            {
                windows.Add(window);
                window.Closed += (o, e) => windows.Remove(window);
                ExtendFrame(window);
                if (window.IsLoaded)
                {
                    SetTheme(window);
                    SetWindowEffect(window);
                }
                else
                {
                    void onLoaded(object o, RoutedEventArgs e)
                    {
                        window.Loaded -= onLoaded;
                        SetTheme(window);
                        SetWindowEffect(window);
                    }
                    window.Loaded += onLoaded;
                }
            }
        }

        /// <summary>
        /// 主题变化回调
        /// </summary>
        private static void OnThemeChanged()
        {
            foreach (var window in windows)
            {
                window.Dispatcher.Invoke(() => SetTheme(window));
            }
        }

        /// <summary>
        /// 设置窗口视觉样式
        /// </summary>
        private static void SetWindowEffect(Window window)
        {
            //暂时不使用Mica样式
            //var handle = new WindowInteropHelper(window).Handle;
            //if (Environment.OSVersion.Version.Build >= 22523)
            //{
            //    uint micaValue = 0x02;
            //    DwmSetWindowAttribute(handle, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref micaValue, Marshal.SizeOf(typeof(int)));
            //    window.Background = Brushes.Transparent;
            //}
            //else
            //{
            //    uint trueValue = 0x01;
            //    DwmSetWindowAttribute(handle, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
            //    window.Background = Brushes.Transparent;
            //}
            window.Background = Brushes.Transparent;
        }

        /// <summary>
        /// 拓展边框到整个窗口
        /// </summary>
        private static void ExtendFrame(Window window)
        {
            var windowChrome = new WindowChrome
            {
                GlassFrameThickness = new Thickness(-1),
                // NonClientFrameEdges = NonClientFrameEdges.Left | NonClientFrameEdges.Right | NonClientFrameEdges.Bottom, // 避免窗口控制按钮出现右边距
                NonClientFrameEdges = NonClientFrameEdges.Right | NonClientFrameEdges.Bottom, // 避免窗口控制按钮出现右边距
                ResizeBorderThickness = new Thickness(4)
            };
            WindowChrome.SetWindowChrome(window, windowChrome);
        }

        /// <summary>
        /// 设置窗口主题
        /// </summary>
        private static void SetTheme(Window window)
        {
            // 设置窗口主题，会影响窗口三个控制按钮颜色
            var uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();
            uint darkModeValue = uiViewModel.Theme switch
            {
                Themes.Light => 0x01,
                Themes.Dark => 0x00,
                _ => (uint)(ThemeListener.IsDarkMode ? 0x01 : 0x00)
            };
            var handle = new WindowInteropHelper(window).Handle;
            DwmSetWindowAttribute(handle, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkModeValue, Marshal.SizeOf(typeof(int)));

            // 设置窗口背景颜色
            uint color = (uint)(ThemeListener.IsDarkMode ? 0x00202020 : 0x00f3f3f3);
            DwmSetWindowAttribute(handle, DwmWindowAttribute.DWMWA_CAPTION_COLOR, ref color, Marshal.SizeOf(typeof(int)));
        }
    }
}
