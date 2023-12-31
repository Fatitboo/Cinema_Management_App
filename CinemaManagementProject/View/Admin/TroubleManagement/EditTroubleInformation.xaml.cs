﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CinemaManagementProject.View.Admin.TroubleManagement
{
    /// <summary>
    /// Interaction logic for EditTroubleInformation.xaml
    /// </summary>
    public partial class EditTroubleInformation : Window
    {
        public EditTroubleInformation()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cbb = sender as ComboBox;

            if (cbb.SelectedIndex == 2)
            {
                _Finishday.Visibility = Visibility.Collapsed;
                _startday.Visibility = Visibility.Collapsed;
                _cost.Visibility = Visibility.Collapsed;
            }
            else if (cbb.SelectedIndex == 0)
            {
                _startday.IsEnabled = true;
                _cost.IsEnabled = false;
                _Finishday.IsEnabled = false;
                costval.Text = "0";
                _Finishday.Visibility = Visibility.Collapsed;
                _startday.Visibility = Visibility.Visible;
                _cost.Visibility = Visibility.Collapsed;
            }
            else if (cbb.SelectedIndex == 1)
        {
                _startday.IsEnabled = false;
                start.SelectedDate = System.DateTime.Today;
                _Finishday.IsEnabled = true;
                _cost.IsEnabled = true;
                Finishdatepicker.Visibility = Visibility.Visible;
                _Finishday.Visibility = Visibility.Visible;
                _startday.Visibility = Visibility.Visible;
                _cost.Visibility = Visibility.Visible;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.Text.Length == 0)
                tb.Text = "0";
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
