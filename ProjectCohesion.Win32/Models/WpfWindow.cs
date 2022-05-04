using ProjectCohesion.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace ProjectCohesion.Win32.Models
{
    public class WpfWindow : IWpfWindow, IDisposable
    {
        public event EventHandler Activated;
        public event EventHandler Deactivated;

        private readonly Window window;

        public bool IsActive { get; private set; }

        public WpfWindow(Window window)
        {
            this.window = window;
            IsActive = window.IsActive;
            window.Activated += Window_Activated;
            window.Deactivated += Window_Deactivated;
        }

        public void Dispose()
        {
            window.Activated -= Window_Activated;
            window.Deactivated -= Window_Deactivated;
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            IsActive = true;
            Activated?.Invoke(this, e);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            IsActive = false;
            Deactivated?.Invoke(this, e);
        }
    }
}
