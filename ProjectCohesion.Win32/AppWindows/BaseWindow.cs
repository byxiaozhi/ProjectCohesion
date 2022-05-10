using Autofac;
using ProjectCohesion.Core.Models;
using ProjectCohesion.Core.ViewModels;
using ProjectCohesion.Win32.Utilities;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using PInvoke;
using System.Windows.Controls;
using ProjectCohesion.Win32.Controls;
using System.Windows.Data;

namespace ProjectCohesion.Win32.AppWindows
{
    public class BaseWindow : Window
    {
        enum DwmWindowAttribute : uint
        {
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_MICA_EFFECT = 1029,
            DWMWA_SYSTEMBACKDROP_TYPE = 38,
            DWMWA_CAPTION_COLOR = 35,
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

        [StructLayout(LayoutKind.Sequential)]
        struct NCCALCSIZE_PARAMS
        {
            public RECT rcNewWindow;
            public RECT rcOldWindow;
            public RECT rcClient;
            IntPtr lppos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        };

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, DwmWindowAttribute dwAttribute, ref uint pvAttribute, int cbAttribute);

        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        static extern bool DwmDefWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, out IntPtr plResult);

        [DllImport("user32.dll")]
        static extern IntPtr TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        // 内容属性
        public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(BaseWindow), null);
        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        // 背景属性，仅支持纯色
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(SolidColorBrush), typeof(BaseWindow), null);
        public new SolidColorBrush Background
        {
            get => (SolidColorBrush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        IntPtr Handle => new WindowInteropHelper(this).Handle;
        double WindowScale => User32.GetDpiForWindow(Handle) / 96.0;

        public BaseWindow()
        {
            InitializeComponent();
            ThemeListener.ThemeChanged += OnThemeChanged;
            Closed += (s, e) => ThemeListener.ThemeChanged -= OnThemeChanged;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hWndSource = HwndSource.FromHwnd(Handle);
            if (Environment.OSVersion.Version.Build >= 22000)
            {
                // 在Win11下拓展标题栏到整个窗口，窗口背景设置透明后会显示标题栏颜色
                hWndSource.CompositionTarget.BackgroundColor = Colors.Transparent;
                base.Background = new SolidColorBrush(Colors.Transparent);
                var nonClientArea = new MARGINS { cyTopHeight = -1 };
                DwmExtendFrameIntoClientArea(Handle, ref nonClientArea);
            }
            else
            {
                // 在Win10下将边框调整为1个像素
                var width = 1 / WindowScale;
                WindowChrome.SetWindowChrome(this, new WindowChrome()
                {
                    GlassFrameThickness = new Thickness(0, width, width, width),
                    NonClientFrameEdges = NonClientFrameEdges.Top
                });
            }

            // 移除WS_CLIPCHILDREN，否则无法渲染透明的Xaml控件
            var style = (User32.SetWindowLongFlags)User32.GetWindowLong(Handle, User32.WindowLongIndexFlags.GWL_STYLE);
            User32.SetWindowLong(Handle, User32.WindowLongIndexFlags.GWL_STYLE, style & ~User32.SetWindowLongFlags.WS_CLIPCHILDREN);
            User32.SetWindowPos(Handle, IntPtr.Zero, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_FRAMECHANGED | User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);

            // 添加钩子
            hWndSource.AddHook(WndProc);

            // 更新窗口
            UpdateWindowEffect();
            UpdateTheme();
            UpdateWindowStyle();
            UpdateRootLayout();
        }

        /// <summary>
        /// 命中测试
        /// </summary>
        protected virtual HitTestFlags HitTest(Point mousePosition)
        {
            if (rootElement == null) return HitTestFlags.CLIENT;

            var isTop = mousePosition.Y <= borderWidth / WindowScale;
            var isBottom = mousePosition.Y >= rootElement.ActualHeight;
            var isLeft = mousePosition.X < 0;
            var isRight = mousePosition.X > rootElement.ActualWidth;
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

            if (captionButtons != null)
            {
                // 检测是否命中控制按钮
                var point = captionButtons.TranslatePoint(new Point(0d, 0d), rootElement);
                if (mousePosition.X > point.X &&
                    mousePosition.Y > point.Y &&
                    mousePosition.X < point.X + captionButtons.ActualWidth &&
                    mousePosition.Y < captionButtons.ActualHeight)
                    return HitTestFlags.CLIENT;
            }

            return hitTest;
        }

        /// <summary>
        /// 打开系统菜单
        /// </summary>
        protected void OpenSystemMenu(IntPtr hWnd)
        {
            var hMenu = User32.GetSystemMenu(hWnd, false);
            User32.GetCursorPos(out var point);
            var retvalue = TrackPopupMenu(hMenu, 0x0100, point.x, point.y, 0, hWnd, new IntPtr());
            User32.PostMessage(hWnd, User32.WindowMessage.WM_SYSCOMMAND, retvalue, IntPtr.Zero);
        }

        readonly int borderWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXFRAME) + User32.GetSystemMetrics(User32.SystemMetric.SM_CXPADDEDBORDER);
        readonly int captionHeight = 32;

        /// <summary>
        /// 处理窗口事件
        /// </summary>
        private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch ((User32.WindowMessage)msg)
            {
                case User32.WindowMessage.WM_NCCALCSIZE:
                    handled = true;
                    if (ResizeMode == ResizeMode.CanResize ||
                        ResizeMode == ResizeMode.CanResizeWithGrip ||
                        Environment.OSVersion.Version.Build >= 22000)
                    {
                        var p = (NCCALCSIZE_PARAMS)Marshal.PtrToStructure(lParam, typeof(NCCALCSIZE_PARAMS));
                        p.rcNewWindow.left += borderWidth - 1;
                        p.rcNewWindow.right -= borderWidth - 1;
                        p.rcNewWindow.bottom -= borderWidth - 1;
                        if (Environment.OSVersion.Version.Build < 22000)
                            p.rcNewWindow.top += 1;
                        Marshal.StructureToPtr(p, lParam, true);
                    }
                    return IntPtr.Zero;
                case User32.WindowMessage.WM_NCHITTEST:
                    handled = true;
                    if (DwmDefWindowProc(hWnd, msg, wParam, lParam, out IntPtr dwmHitTest))
                        return dwmHitTest;
                    var mousePosition = PointFromScreen(new Point(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16));
                    var hitTest = HitTest(mousePosition);
                    return (IntPtr)hitTest;
                case User32.WindowMessage.WM_NCRBUTTONUP:
                    handled = true;
                    if (wParam == (IntPtr)HitTestFlags.CAPTION)
                        OpenSystemMenu(hWnd);
                    break;
                case User32.WindowMessage.WM_GETMINMAXINFO:
                    if (Environment.OSVersion.Version.Build < 22000)
                        UpdateMinMaxInfo(hWnd, lParam);
                    break;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// 主题变化回调
        /// </summary>
        private void OnThemeChanged()
        {
            Dispatcher.Invoke(UpdateTheme);
        }

        /// <summary>
        /// 设置窗口最大化信息
        /// </summary>
        private void UpdateMinMaxInfo(IntPtr hWnd, IntPtr lParam)
        {
            var monitor = User32.MonitorFromWindow(hWnd, User32.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                User32.MONITORINFO monitorInfo = new();
                monitorInfo.cbSize = Marshal.SizeOf(typeof(User32.MONITORINFO));
                User32.GetMonitorInfo(monitor, ref monitorInfo);
                var rcWorkArea = monitorInfo.rcWork;
                var rcMonitorArea = monitorInfo.rcMonitor;
                var minmaxinfo = (User32.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(User32.MINMAXINFO));
                minmaxinfo.ptMaxPosition.x = rcWorkArea.left - rcMonitorArea.left - borderWidth;
                minmaxinfo.ptMaxPosition.y = rcWorkArea.top - rcMonitorArea.top - borderWidth;
                minmaxinfo.ptMaxSize.x = rcWorkArea.right - rcWorkArea.left + borderWidth * 2;
                minmaxinfo.ptMaxSize.y = rcWorkArea.bottom - rcWorkArea.top + borderWidth * 2;
                Marshal.StructureToPtr(minmaxinfo, lParam, true);
            }
        }

        /// <summary>
        /// 设置窗口视觉样式
        /// </summary>
        private void UpdateWindowEffect()
        {
            if (Handle == null) return;

            if (Background != null)
            {
                uint falseValue = 0x00;
                DwmSetWindowAttribute(Handle, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref falseValue, Marshal.SizeOf(typeof(int)));
                DwmSetWindowAttribute(Handle, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref falseValue, Marshal.SizeOf(typeof(int)));
            }
            else if (Environment.OSVersion.Version.Build >= 22523)
            {
                uint micaValue = 0x02;
                DwmSetWindowAttribute(Handle, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref micaValue, Marshal.SizeOf(typeof(int)));
            }
            else if (Environment.OSVersion.Version.Build >= 22000)
            {
                uint trueValue = 0x01;
                DwmSetWindowAttribute(Handle, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref trueValue, Marshal.SizeOf(typeof(int)));
            }
        }

        /// <summary>
        /// 设置窗口样式
        /// </summary>
        private void UpdateWindowStyle()
        {
            if (Environment.OSVersion.Version.Build < 22000)
                if (ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip)
                    WindowStyle = WindowStyle.None;
                else
                    WindowStyle = WindowStyle.SingleBorderWindow;
        }

        /// <summary>
        /// 设置窗口布局
        /// </summary>
        private void UpdateRootLayout()
        {
            if (rootElement == null) return;

            // 最大化时整个布局向下移动
            if (WindowState == WindowState.Maximized && rootElement != null)
                rootElement.Margin = new Thickness(0, borderWidth / WindowScale, 0, 0);
            else
                rootElement.Margin = new Thickness(0);
        }

        /// <summary>
        /// 设置窗口主题
        /// </summary>
        private void UpdateTheme()
        {
            if (Handle == null) return;

            // 获取设置的主题
            var uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();
            uint darkModeValue = uiViewModel.Theme switch
            {
                Themes.Light => 0,
                Themes.Dark => 1,
                _ => (uint)(ThemeListener.IsDarkMode ? 1 : 0)
            };

            // 设置窗口主题，会影响窗口三个控制按钮颜色
            DwmSetWindowAttribute(Handle, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkModeValue, Marshal.SizeOf(typeof(int)));

            // 设置背景色
            var fallbackColor = (Color)ColorConverter.ConvertFromString(darkModeValue == 1 ? "#202020" : "#f3f3f3");
            if (Environment.OSVersion.Version.Build >= 22000)
            {
                // 在Win11下由于标题栏被拓展到整个窗口，所以只需要设置标题栏背景颜色
                // 由于标题栏颜色不支持半透明，所以需要混色
                if (Background != null)
                {
                    var alpha1 = Background.Color.A / 255f;
                    var alpha2 = 1 - alpha1;
                    uint color = 0;
                    color |= (uint)(Background.Color.R * alpha1 + fallbackColor.R * alpha2) << 0;
                    color |= (uint)(Background.Color.G * alpha1 + fallbackColor.G * alpha2) << 8;
                    color |= (uint)(Background.Color.B * alpha1 + fallbackColor.B * alpha2) << 16;
                    DwmSetWindowAttribute(Handle, DwmWindowAttribute.DWMWA_CAPTION_COLOR, ref color, Marshal.SizeOf(typeof(int)));
                }
            }
            else
            {
                // 在Win10下需要设置窗口背景颜色
                base.Background = new SolidColorBrush(fallbackColor);
                rootElement.Background = Background;
            }
        }

        /// <summary>
        // 构建基础布局
        /// </summary>
        private void InitializeComponent()
        {
            if (rootElement != null) return;
            base.Content = rootElement = new Grid();
            contentElement = new ContentControl();
            rootElement.Children.Add(contentElement);
            if (Environment.OSVersion.Version.Build < 22000)
            {
                // Win10下添加自定义窗口控制按钮
                rootElement.Children.Add(
                    captionButtons = new CaptionButtons()
                    {
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Right,
                    });
            }
        }

        /// <summary>
        /// 依赖属性变化
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == BackgroundProperty)
            {
                UpdateWindowEffect();
                UpdateTheme();
            }
            else if (e.Property == ResizeModeProperty)
            {
                UpdateWindowStyle();
            }
            else if (e.Property == WindowStateProperty)
            {
                UpdateRootLayout();
            }
            else if (e.Property == ContentProperty)
            {
                contentElement.Content = Content;
            }
        }

        private Grid rootElement;
        private ContentControl contentElement;
        private CaptionButtons captionButtons;
    }
}
