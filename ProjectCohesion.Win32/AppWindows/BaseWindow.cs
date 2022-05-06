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
        static extern bool TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        // 内容属性
        public new static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(BaseWindow), new PropertyMetadata(PropertyChanged));
        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        // 背景属性，仅支持纯色
        public new static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(SolidColorBrush), typeof(BaseWindow), new PropertyMetadata(PropertyChanged));
        public new SolidColorBrush Background
        {
            get => (SolidColorBrush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        double WindowScale => User32.GetDpiForWindow(new WindowInteropHelper(this).Handle) / 96.0;

        public BaseWindow()
        {
            ThemeListener.ThemeChanged += OnThemeChanged;
            Closed += (s, e) => ThemeListener.ThemeChanged -= OnThemeChanged;
            StateChanged += (s, e) => UpdateWindowStyle();
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
                base.Background = new SolidColorBrush(Colors.Transparent);
                var nonClientArea = new MARGINS { cyTopHeight = -1 };
                DwmExtendFrameIntoClientArea(hWnd, ref nonClientArea);
            }
            else
            {
                // 在Win10下将删除标题栏并将边框调整为1个像素
                var width = 1 / WindowScale;
                WindowChrome.SetWindowChrome(this, new WindowChrome()
                {

                    GlassFrameThickness = new Thickness(0, width, width, width),
                    NonClientFrameEdges = NonClientFrameEdges.Top
                }); ;
                UpdateWindowStyle();
            }

            // 移除WS_CLIPCHILDREN，否则无法渲染透明的Xaml控件
            var style = (User32.SetWindowLongFlags)User32.GetWindowLong(hWnd, User32.WindowLongIndexFlags.GWL_STYLE);
            User32.SetWindowLong(hWnd, User32.WindowLongIndexFlags.GWL_STYLE, style & ~User32.SetWindowLongFlags.WS_CLIPCHILDREN);

            User32.SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, User32.SetWindowPosFlags.SWP_FRAMECHANGED | User32.SetWindowPosFlags.SWP_NOMOVE | User32.SetWindowPosFlags.SWP_NOSIZE);
            UpdateWindowEffect(hWnd);
            UpdateTheme(hWnd);
            hWndSource.AddHook(WndProc);
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
        /// 打开系统菜单（但是还没有处理点击事件）
        /// </summary>
        protected void OpenSystemMenu()
        {
            IntPtr hMenu = User32.GetSystemMenu(new WindowInteropHelper(this).Handle, false);
            User32.GetCursorPos(out var point);
            TrackPopupMenu(hMenu, 0, point.x, point.y, 0, new WindowInteropHelper(this).Handle, new IntPtr());
        }

        readonly int borderWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXFRAME) + User32.GetSystemMetrics(User32.SystemMetric.SM_CXPADDEDBORDER);
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
                    if (DwmDefWindowProc(hwnd, msg, wParam, lParam, out IntPtr dwmHitTest))
                        return dwmHitTest;
                    var mousePosition = PointFromScreen(new Point(lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16));
                    var hitTest = HitTest(mousePosition);
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
            Dispatcher.Invoke(() => UpdateTheme(new WindowInteropHelper(this).Handle));
        }

        /// <summary>
        /// 设置窗口视觉样式
        /// </summary>
        private void UpdateWindowEffect(IntPtr hWnd)
        {
            if (Background != null)
            {
                uint falseValue = 0x00;
                DwmSetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_MICA_EFFECT, ref falseValue, Marshal.SizeOf(typeof(int)));
                DwmSetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_SYSTEMBACKDROP_TYPE, ref falseValue, Marshal.SizeOf(typeof(int)));
            }
            else if (Environment.OSVersion.Version.Build >= 22523)
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
        private void UpdateWindowStyle()
        {
            // 还有更好的方式得到一个在Win10下无边框且可以最大化的窗口吗？
            if (Environment.OSVersion.Version.Build < 22000)
                if (WindowState == WindowState.Maximized)
                    WindowStyle = WindowStyle.SingleBorderWindow;
                else
                    WindowStyle = WindowStyle.None;

            // 最大化时整个布局向下移动
            if (WindowState == WindowState.Maximized && rootElement != null)
                rootElement.Margin = new Thickness(0, borderWidth / WindowScale, 0, 0);
            else
                rootElement.Margin = new Thickness(0);
        }

        /// <summary>
        /// 设置窗口主题
        /// </summary>
        private void UpdateTheme(IntPtr hWnd)
        {
            // 获取设置的主题
            var uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();
            uint darkModeValue = uiViewModel.Theme switch
            {
                Themes.Light => 0,
                Themes.Dark => 1,
                _ => (uint)(ThemeListener.IsDarkMode ? 1 : 0)
            };

            // 设置窗口主题，会影响窗口三个控制按钮颜色
            DwmSetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref darkModeValue, Marshal.SizeOf(typeof(int)));

            if (Environment.OSVersion.Version.Build >= 22000)
            {
                // 在Win11下由于标题栏被拓展到整个窗口，所以只需要设置标题栏背景颜色
                if (Background != null)
                {
                    uint color = 0;
                    color |= (uint)Background.Color.R << 0;
                    color |= (uint)Background.Color.G << 8;
                    color |= (uint)Background.Color.B << 16;
                    DwmSetWindowAttribute(hWnd, DwmWindowAttribute.DWMWA_CAPTION_COLOR, ref color, Marshal.SizeOf(typeof(int)));
                }

            }
            else
            {
                // 在Win10下需要设置窗口背景颜色
                if (Background == null)
                    base.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(darkModeValue == 1 ? "#202020" : "#f3f3f3"));
                else
                    base.Background = Background;
            }
        }

        // 构建基础布局
        private void UpdateContent()
        {
            if (rootElement == null)
            {
                rootElement = new Grid();
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
            contentElement.Content = Content;
            base.Content = rootElement;
        }

        /// <summary>
        /// 依赖属性变化
        /// </summary>
        private static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BaseWindow)d).PropertyChanged(e);
        }

        /// <summary>
        /// 依赖属性变化
        /// </summary>
        private void PropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(Content))
            {
                UpdateContent();
            }
            else if (e.Property.Name == nameof(Background))
            {
                var hWnd = new WindowInteropHelper(this).Handle;
                UpdateWindowEffect(hWnd);
                UpdateTheme(hWnd);
            }
        }

        private Grid rootElement;
        private ContentControl contentElement;
        private CaptionButtons captionButtons;
    }
}
