using PInvoke;
using System;
using System.Collections.Generic;
using System.Reflection;
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
            contentControl = new();
            Inherit(DataContextProperty);
            Inherit(ContentControl.ContentProperty);
            var fieldInfo = typeof(Window).GetField("IWindowServiceProperty", BindingFlags.Static | BindingFlags.NonPublic);
            var window = Window.GetWindow(this);
            contentControl.SetBinding(fieldInfo.GetValue(window) as DependencyProperty, new Binding("IWindowService") { Source = window });

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

        private void Inherit(DependencyProperty property)
        {
            contentControl.SetBinding(property, new Binding(property.Name) { Source = this });
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
