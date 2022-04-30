using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectCohesion
{
    public partial class App : Application
    {
        readonly ModuleLoader moduleLoader = new();
        public App()
        {
            InitializeComponent();
            moduleLoader.Initialize(Core.Autofac.Container);
        }
    }
}
