﻿using CinemaManagementProject.ViewModel.StaffVM.OrderFoodManagementVM;
using CinemaManagementProject.ViewModel.StaffVM.TicketVM;
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
using System.Windows.Shapes;

namespace CinemaManagementProject.View.Staff.TicketWindow
{
    /// <summary>
    /// Interaction logic for TicketWindow.xaml
    /// </summary>
    public partial class TicketWindow : Window
    {
        public TicketWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void EditWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
