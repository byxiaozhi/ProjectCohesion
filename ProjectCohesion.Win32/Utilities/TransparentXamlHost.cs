using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Interop;

namespace ProjectCohesion.Win32.Utilities
{
    public class TransparentXamlHost : WindowsXamlHost
    {
        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, ref PInvoke.RECT lprcUpdate, IntPtr hrgnUpdate, int flags);

        readonly Timer timer = new();

        public TransparentXamlHost()
        {
            timer.Elapsed += Restore;
            timer.Interval = 15;
            timer.AutoReset = false;
        }

        private void Restore(object sender, ElapsedEventArgs e)
        {
            if (Handle != IntPtr.Zero)
            {
                var ex = (PInvoke.User32.SetWindowLongFlags)PInvoke.User32.GetWindowLong(Handle, PInvoke.User32.WindowLongIndexFlags.GWL_EXSTYLE);
                if (ex.HasFlag(PInvoke.User32.SetWindowLongFlags.WS_EX_TRANSPARENT))
                    PInvoke.User32.SetWindowLong(Handle, PInvoke.User32.WindowLongIndexFlags.GWL_EXSTYLE, ex & ~PInvoke.User32.SetWindowLongFlags.WS_EX_TRANSPARENT);
            }
        }

        private void Redraw(IntPtr hwnd)
        {
            var ex = (PInvoke.User32.SetWindowLongFlags)PInvoke.User32.GetWindowLong(hwnd, PInvoke.User32.WindowLongIndexFlags.GWL_EXSTYLE);
            if (!ex.HasFlag(PInvoke.User32.SetWindowLongFlags.WS_EX_TRANSPARENT))
                PInvoke.User32.SetWindowLong(Handle, PInvoke.User32.WindowLongIndexFlags.GWL_EXSTYLE, ex | PInvoke.User32.SetWindowLongFlags.WS_EX_TRANSPARENT);
            timer.Stop();
            timer.Start();
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // 当位置或大小发生变化时重绘
            if (msg == 70) Redraw(hwnd);
            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }
    }
}
