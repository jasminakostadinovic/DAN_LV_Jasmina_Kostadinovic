﻿using PanPizza.ViewModel;
using System.Windows;

namespace PanPizza
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {       
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(this);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            ToppingsGrid.Items.Refresh();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            ToppingsGrid.Items.Refresh();
        }    
    }
}
