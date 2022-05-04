﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectCohesion.Controls.MainWindow
{

    public partial class TitleBar : UserControl
    {
        public bool CaptionButtonVisibility => Environment.OSVersion.Version.Build < 22000;

        public TitleBar()
        {
            InitializeComponent();
        }
    }
}
