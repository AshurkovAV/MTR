﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Examples.MVVM.Basic
{
    /// <summary>
    /// Interaction logic for View.xaml
    /// </summary>
    public partial class View : Page
    {
        public View()
        {
            InitializeComponent();

            DataContext = new ViewModel();
        }
    }
}
