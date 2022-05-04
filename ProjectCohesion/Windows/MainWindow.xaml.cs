using ProjectCohesion.Win32.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Autofac;
using ProjectCohesion.Core.ViewModels;

namespace ProjectCohesion
{
    public partial class MainWindow : Win32.AppWindows.BaseWindow
    {
        readonly Properties.Settings settings = Properties.Settings.Default;

        public MainWindow()
        {
            DataContext = Core.Autofac.Container.Resolve<AppViewModel>();
            InitializeComponent();
            RestoreWindowPlacement();
        }

        public void RestoreWindowPlacement()
        {
            if (!string.IsNullOrEmpty(settings.MainWindowPlacement))
            {
                WindowPlacement.RestoreWindowPlacement(this, JsonConvert.DeserializeObject<PInvoke.User32.WINDOWPLACEMENT>(settings.MainWindowPlacement));
            }
        }

        public void SaveWindowPlacement()
        {
            settings.MainWindowPlacement = JsonConvert.SerializeObject(WindowPlacement.GetWindowPlacement(this));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveWindowPlacement();
            settings.Save();
        }

        protected override HitTestFlags HitTest(Point mousePosition) { 
            var hit = base.HitTest(mousePosition);
            if (titleBar.CaptionButtonVisibility && hit != HitTestFlags.RIGHT && mousePosition.Y < titleBar.ActualHeight && mousePosition.X > titleBar.ActualWidth - 46 * 3)
                return HitTestFlags.CLIENT;
            return hit;
        }
    }
}
