using Microsoft.Toolkit.Wpf.UI.XamlHost;
using PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace ProjectCohesion.Win32.Controls
{
    public class AppXamlHost : WindowsXamlHost
    {
        public static IEnumerable<Windows.UI.Xaml.UIElement> Elements => elementDictionary.Values;

        private static Dictionary<AppXamlHost, Windows.UI.Xaml.UIElement> elementDictionary = new();

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            return base.BuildWindowCore(hwndParent);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            base.DestroyWindowCore(hwnd);
        }

        protected override async void OnWindowPositionChanged(Rect rcBoundingBox)
        {
            await Task.Yield();
            base.OnWindowPositionChanged(rcBoundingBox);
        }

        protected override void OnChildChanged()
        {
            base.OnChildChanged();
            if (GetUwpInternalObject() is Windows.UI.Xaml.UIElement ele)
                elementDictionary.Add(this, ele);
            else
                elementDictionary.Remove(this);
        }

    }
}
