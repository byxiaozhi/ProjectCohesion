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

namespace ProjectCohesion.Modules.Demo.Controls.Menu
{
    public partial class UnitSetting : UserControl
    {
        public UnitSetting()
        {
            InitializeComponent();
            comboBox.ItemsSource = new List<string>()
            {
                "国际单位",
                "英制单位"
            };
        }
    }
}
