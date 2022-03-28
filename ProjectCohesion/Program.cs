using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectCohesion
{
    class Program
    {
        [STAThread()]
        public static void Main()
        {
            using (new XamlHost.App())
            {
                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }
    }
}
