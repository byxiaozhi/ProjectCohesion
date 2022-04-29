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

namespace ProjectCohesion
{
    public partial class MainWindow : Window
    {
        readonly Properties.Settings settings = Properties.Settings.Default;

        public MainWindow()
        {
            WindowBackdrop.SetBackdrop(this);
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
    }
}
