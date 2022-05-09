using PInvoke;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace ProjectCohesion.Win32.Utilities
{
    /// <summary>
    /// 窗口位置
    /// </summary>
    public static class WindowPlacement
    {
        /// <summary>
        /// 获取窗口位置
        /// </summary>
        public static User32.WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd)
        {
            var placement = User32.GetWindowPlacement(hwnd);
            var scale = User32.GetDpiForWindow(hwnd) / 96.0;
            // 通过Win32API获得的坐标没有经过缩放，所以需要除以显示器缩放
            placement.rcNormalPosition.top = (int)(placement.rcNormalPosition.top / scale);
            placement.rcNormalPosition.left = (int)(placement.rcNormalPosition.left / scale);
            placement.rcNormalPosition.right = (int)(placement.rcNormalPosition.right / scale);
            placement.rcNormalPosition.bottom = (int)(placement.rcNormalPosition.bottom / scale);
            placement.ptMaxPosition.x = (int)(placement.ptMaxPosition.x / scale);
            placement.ptMaxPosition.y = (int)(placement.ptMaxPosition.y / scale);
            return placement;
        }

        /// <summary>
        /// 获取窗口位置
        /// </summary>
        public static User32.WINDOWPLACEMENT GetWindowPlacement(Window window)
        {
            // 当窗口最大化时，无法通过Window获得正确的窗口大小，所以使用Win32API
            var hwnd = new WindowInteropHelper(window).Handle;
            return GetWindowPlacement(hwnd);
        }

        /// <summary>
        /// 恢复窗口位置
        /// </summary>
        public static void RestoreWindowPlacement(Window window, User32.WINDOWPLACEMENT placement)
        {            
            var position = placement.rcNormalPosition;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Left = position.left;
            window.Top = position.top;
            window.Width = (position.right - position.left);
            window.Height = (position.bottom - position.top);

            // 恢复窗口状态时，只处理最大化/正常两种情况，不考虑窗口启动时就最小化等情况
            window.WindowState = placement.showCmd == User32.WindowShowStyle.SW_SHOWMAXIMIZED ? WindowState.Maximized : WindowState.Normal;
        }
    }
}
