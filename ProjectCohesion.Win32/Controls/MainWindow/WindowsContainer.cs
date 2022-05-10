using PInvoke;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Markup;

namespace ProjectCohesion.Win32.Controls
{
    [ContentProperty("Content")]
    public partial class WindowContainer : HwndHost
    {

        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(WindowContainer), null);
        public object Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }

        private HwndSource hwndSource;
        private ContentControl contentControl;

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            contentControl = new() { DataContext = DataContext };
            var binding = new Binding()
            {
                Source = this,
                Path = new PropertyPath(nameof(Content)),
                Mode = BindingMode.OneWay,
            };
            contentControl.SetBinding(ContentControl.ContentProperty, binding);

            hwndSource = new(new HwndSourceParameters()
            {
                ParentWindow = hwndParent.Handle,
                WindowStyle = (int)User32.WindowStyles.WS_CHILD,
                UsesPerPixelTransparency = true,
                Height = 0,
                Width = 0,
            });
            hwndSource.RootVisual = contentControl;

            return new HandleRef(this, hwndSource.Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            hwndSource?.Dispose();
        }

        protected override async void OnWindowPositionChanged(Rect rcBoundingBox)
        {
            await Task.Yield();
            base.OnWindowPositionChanged(rcBoundingBox);
        }

    }
}
