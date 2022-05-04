using Autofac;
using ProjectCohesion.Core.Models;
using ProjectCohesion.Core.ViewModels;
using ProjectCohesion.Win32.Utilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace ProjectCohesion.Win32.AppWindows
{
    public class BaseWindow : Window
    {
        enum DwmWindowAttribute : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029,
            DWMWA_SYSTEMBACKDROP_TYPE = 38,
            DWMWA_CAPTION_COLOR = 1030,
        }

        enum WindowPosFlags : uint
        {
            SWP_FRAMECHANGED = 0x20u,
            SWP_NOMOVE = 0x2u,
            SWP_NOSIZE = 0x1u,
        }

        public enum HitTestFlags
        {
            CLIENT = 1,
            CAPTION = 2,
            LEFT = 10,
            RIGHT,
            TOP,
            TOPLEFT,
            TOPRIGHT,
            BOTTOM,
            BOTTOMLEFT,
            BOTTOMRIGHT
        }

        enum SystemMetric
        {
            SM_CXFRAME = 32,
            SM_CXPADDEDBORDER = 92,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct NCCALCSIZE_PARAMS
        {
            public RECT rcNewWindow;
            public RECT rcOldWindow;
            public RECT rcClient;
            IntPtr lppos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        };

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref uint pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins pMarInset);

        [DllImport("dwmapi.dll")]
        static extern bool DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, WindowPosFlags uFlags);

        [DllImport("user32.dll")]
        static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bRevert);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        static extern bool TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        public BaseWindow()
        {
            ThemeListener.ThemeChanged += OnThemeChanged;
            Closed += (s, e) => ThemeListener.ThemeChanged -= OnThemeChanged;
            StateChanged += (s, e) => SetWindowStyle();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hWnd = new WindowInteropHelper(this).Handle;
            var hWndSource = HwndSource.FromHwnd(hWnd);
            if (Environment.OSVersion.Version.Build >= 22000)
            {
                // 在Win11下拓展标题栏到整个窗口，窗口背景设置透明后会显示标题栏颜色
                hWndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
                Background = new SolidColorBrush(Colors.Transparent);
                var nonClientArea = new Margins { cyTopHeight = -1 };
                DwmExtendFrameIntoClientArea(hWnd, ref nonClientArea);
            }
            else
            {
                SetWindowStyle();
                WindowChrome.SetWindowChrome(this, new WindowChrome()
                {
                    GlassFrameThickness = new Thickness(0, 0.1, 0.1, 0.1),
                    NonClientFrameEdges = NonClientFrameEdges.Top
                });
            }
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, WindowPosFlags.SWP_FRAMECHANGED | WindowPosFlags.SWP_NOMOVE | WindowPosFlags.SWP_NOSIZE);
            SetWindowEffect(hWnd);
            SetTheme(hWnd);
            hWndSource.AddHook(WndProc);
        }

        /// <summary>
        /// 命中测试
        /// </summary>
        protected virtual HitTestFlags HitTest(Point mousePosition)
        {
            var isTop = mousePosition.Y <= borderWidth / 2.0;
            var isBottom = mousePosition.Y >= ActualHeight - borderWidth / 2.0 - 1;
            var isLeft = mousePosition.X < 0;
            var isRight = mousePosition.X >= ActualWidth - borderWidth - 1;
            var hitTest = HitTestFlags.CLIENT;
            if (isTop)
            {
                if (isLeft) hitTest = HitTestFlags.TOPLEFT;
                else if (isRight) hitTest = HitTestFlags.TOPRIGHT;
                else hitTest = HitTestFlags.TOP;
            }
            else if (isBottom)
            {
                if (isLeft) hitTest = HitTestFlags.BOTTOMLEFT;
                else if (isRight) hitTest = HitTestFlags.BOTTOMRIGHT;
                else hitTest = HitTestFlags.BOTTOM;
            }
            else if (isLeft) hitTest = HitTestFlags.LEFT;
            else if (isRight) hitTest = HitTestFlags.RIGHT;
            else if (mousePosition.Y <= captionHeight) hitTest = HitTestFlags.CAPTION;
            return hitTest;
        }

        /// <summary>
        /// 打开系统菜单（但是还没有处理点击事件）
        /// </summary>
        protected void OpenSystemMenu()
        {
            IntPtr hMenu = GetSystemMenu(new WindowInteropHelper(this).Handle, false);
            GetCursorPos(out POINT point);
            TrackPopupMenu(hMenu, 0, point.x, point.y, 0, new WindowInteropHelper(this).Handle, new IntPtr());
        }

        readonly int borderWidth = GetSystemMetrics(SystemMetric.SM_CXFRAME) + GetSystemMetrics(SystemMetric.SM_CXPADDEDBORDER);
        readonly int captionHeight = 32;

        /// <summary>
        /// 处理窗口事件
        /// </summary>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            switch (msg)
            {
                case 0x0083: // NCCALCSIZE
                    handled = true;
                    var p = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NCCALCSIZE_PARAMS));
                    p.rcNewWindow.left += borderWidth - 1;
                    p.rcNewWindow.right -= borderWidth - 1;
                    p.rcNewWindow.bottom -= borderWidth - 1;
                    if (Environment.OSVersion.Version.Build < 22000)
                        p.rcNewWindow.top += 1;
                    Marshal.StructureToPtr(p, lParam, false);
                    return IntPtr.Zero;
                case 0x0084: // NCHITTEST
                    handled = true;
                    var mousePosition = PointFromScreen(new Point(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16));
                    var hitTest = HitTest(mousePosition);
                    if (hitTest == HitTestFlags.CAPTION && DwmDefWindowProc(hwnd, msg, wParam, lParam, out IntPtr dwmHitTest))
                        return dwmHitTest;
                    return (IntPtr)hitTest;
                case 0x00A5: // NCRBUTTONUP
                    handled = true;
                    if (wParam == (IntPtr)HitTestFlags.CAPTION)
                        OpenSystemMenu();
                    break;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 主题变化回调
        /// </summary>
        private void OnThemeChanged()
        {
            Dispatcher.Invoke(() => SetTheme(new WindowInteropHelper(this).Handle));
        }

        /// <summary>
        /// 设置窗口视觉样式
        /// </summary>
        private void SetWindowEffect(IntPtr hWnd)
        {
            if (Environment.OSVersion.Version.Build >= 22523)
            {
                uint micaValue = 0x02;
                DwmSetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref micaValue, Marshal.SizeOf(typeof(int)));
            }
            else if (Environment.OSVersion.Version.Build >= 22000)
            {
                uint trueValue = 0x01;
                DwmSetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
            }
        }

        /// <summary>
        /// 设置窗口样式
        /// </summary>
        private void SetWindowStyle()
        {
            // 还有更好的方式得到一个在Win10下无边框且可以最大化的窗口吗？
            if (Environment.OSVersion.Version.Build < 22000)
                if (WindowState == WindowState.Maximized)
                    WindowStyle = WindowStyle.SingleBorderWindow;
                else
                    WindowStyle = WindowStyle.None;
        }

        /// <summary>
        /// 设置窗口主题
        /// </summary>
        private void SetTheme(IntPtr hWnd)
        {
            var uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();
            uint darkModeValue = uiViewModel.Theme switch
            {
                Themes.Light => 0x01,
                Themes.Dark => 0x00,
                _ => (uint)(ThemeListener.IsDarkMode ? 0x01 : 0x00)
            };

            // 设置窗口主题，会影响窗口三个控制按钮颜色
            DwmSetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkModeValue, Marshal.SizeOf(typeof(int)));

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                // 在Win11下由于标题栏被拓展到整个窗口，所以只需要设置标题栏背景颜色
                uint color = (uint)(darkModeValue == 0x01 ? 0x00202020 : 0x00f3f3f3);
                DwmSetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_CAPTION_COLOR, ref color, Marshal.SizeOf(typeof(int)));
            }
            else
            {
                // 在Win10下需要设置窗口背景颜色
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(darkModeValue == 0x01 ? "#202020" : "#f3f3f3"));
            }
        }
    }
}
