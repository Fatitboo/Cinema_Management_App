﻿using CinemaManagementProject.DTOs;
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

namespace CinemaManagementProject.View.Admin.VoucherManagement
{
    /// <summary>
    /// Interaction logic for VoucherManagementPage.xaml
    /// </summary>
    public partial class VoucherManagementPage : Page
    {
        public VoucherManagementPage()
        {
            InitializeComponent();
        }
        private bool Filter(object item)
        {
            if (String.IsNullOrEmpty(SearchBox.Text))
                return true;
            else
                return ((item as VoucherReleaseDTO).VoucherReleaseCode.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                       || ((item as VoucherReleaseDTO).VoucherReleaseName.IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void Search_SearchTextChange(object sender, EventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(VoucherListView.ItemsSource);
            if (view != null)
            {
                view.Filter = Filter;
                result.Content = VoucherListView.Items.Count;
                CollectionViewSource.GetDefaultView(VoucherListView.ItemsSource).Refresh();
            }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
