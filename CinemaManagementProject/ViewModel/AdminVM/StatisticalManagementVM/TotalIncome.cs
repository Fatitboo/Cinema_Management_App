﻿using CinemaManagementProject.DTOs;
using CinemaManagementProject.Model.Service;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CinemaManagementProject.ViewModel.AdminVM.StatisticalManagementVM
{
    public partial class StatisticalManagementVM : BaseViewModel
    {
        private SeriesCollection _Top5MovieData;
        public SeriesCollection Top5MovieData
        {
            get { return _Top5MovieData; }
            set { _Top5MovieData = value; OnPropertyChanged(); }
        }

        private SeriesCollection _Top5FoodData;
        public SeriesCollection Top5FoodData
        {
            get { return _Top5FoodData; }
            set { _Top5FoodData = value; OnPropertyChanged(); }
        }

        private List<FilmDTO> top5Movie;
        public List<FilmDTO> Top5Movie
        {
            get { return top5Movie; }
            set { top5Movie = value; OnPropertyChanged(); }
        }

        private List<ProductDTO> top5Product;
        public List<ProductDTO> Top5Product
        {
            get { return top5Product; }
            set { top5Product = value; OnPropertyChanged(); }
        }


        private ComboBoxItem _SelectedBestSellPeriod;
        public ComboBoxItem SelectedBestSellPeriod
        {
            get { return _SelectedBestSellPeriod; }
            set { _SelectedBestSellPeriod = value; OnPropertyChanged(); }
        }

        private string _selectedBestSellTime;
        public string SelectedBestSellTime
        {
            get { return _selectedBestSellTime; }
            set { _selectedBestSellTime = value; OnPropertyChanged(); }
        }

        private ComboBoxItem _SelectedBestSellPeriod2;
        public ComboBoxItem SelectedBestSellPeriod2
        {
            get { return _SelectedBestSellPeriod2; }
            set { _SelectedBestSellPeriod2 = value; OnPropertyChanged(); }
        }

        private string _selectedBestSellTime2;
        public string SelectedBestSellTime2
        {
            get { return _selectedBestSellTime2; }
            set { _selectedBestSellTime2 = value; OnPropertyChanged(); }
        }



        public async Task ChangeBestSellPeriod()
        {
            if (SelectedBestSellPeriod != null)
            {
                switch (SelectedBestSellPeriod.Content.ToString())
                {
                    case "Theo năm":
                        {
                            if (SelectedBestSellTime != null)
                            {
                                await LoadBestSellByYear();
                            }
                            return;
                        }
                    case "By Year":
                        {
                            if (SelectedBestSellTime != null)
                            {
                                await LoadBestSellByYear();
                            }
                            return;
                        }
                    case "Theo tháng":
                        {
                            if (SelectedBestSellTime != null)
                            {
                                await LoadBestSellByMonth();
                            }
                            return;
                        }
                    case "By Month":
                        {
                            if (SelectedBestSellTime != null)
                            {
                                await LoadBestSellByMonth();
                            }
                            return;
                        }
                }
            }
        }
        public async Task LoadBestSellByYear()
        {
            if (SelectedBestSellTime.Length != 4) return;
            try
            {
                Top5Movie = await Task.Run(() => StatisticsService.Ins.GetTop5BestMovieByYear(int.Parse(SelectedBestSellTime)));
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                Console.WriteLine(e);
                if (Properties.Settings.Default.isEnglish) CustomMessageBox.ShowOk("Unable to connect to database", "Error", "OK", Views.CustomMessageBoxImage.Error);
                else CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (Properties.Settings.Default.isEnglish) CustomMessageBox.ShowOk("System error", "Error", "OK", Views.CustomMessageBoxImage.Error);
                else CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
            }



            List<float> chartdata = new List<float>();
            chartdata.Add(0);
            for (int i = 0; i < Top5Movie.Count; i++)
            {
                chartdata.Add(Top5Movie[i].Revenue / 1000000);
            }

            Top5MovieData = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<float>(chartdata),
                    Title = Properties.Settings.Default.isEnglish ? "Revenue" : "Doanh thu"
                },
            };
        }
        public async Task LoadBestSellByMonth()
        {
            if (SelectedBestSellTime.Length == 4) return;
            try
            {
                if (SelectedBestSellTime == "January") SelectedBestSellTime = "Tháng 1";
                if (SelectedBestSellTime == "February") SelectedBestSellTime = "Tháng 2";
                if (SelectedBestSellTime == "March") SelectedBestSellTime = "Tháng 3";
                if (SelectedBestSellTime == "April") SelectedBestSellTime = "Tháng 4";
                if (SelectedBestSellTime == "May") SelectedBestSellTime = "Tháng 5";
                if (SelectedBestSellTime == "June") SelectedBestSellTime = "Tháng 6";
                if (SelectedBestSellTime == "July") SelectedBestSellTime = "Tháng 7";
                if (SelectedBestSellTime == "August") SelectedBestSellTime = "Tháng 8";
                if (SelectedBestSellTime == "September") SelectedBestSellTime = "Tháng 9";
                if (SelectedBestSellTime == "October") SelectedBestSellTime = "Tháng 10";
                if (SelectedBestSellTime == "November") SelectedBestSellTime = "Tháng 11";
                if (SelectedBestSellTime == "December") SelectedBestSellTime = "Tháng 12";
                Top5Movie = await Task.Run(() => StatisticsService.Ins.GetTop5BestMovieByMonth(int.Parse(SelectedBestSellTime.Remove(0, 6))));
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                Console.WriteLine(e);
                if (Properties.Settings.Default.isEnglish) CustomMessageBox.ShowOk("Unable to connect to database", "Error", "OK", Views.CustomMessageBoxImage.Error);
                else CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (Properties.Settings.Default.isEnglish) CustomMessageBox.ShowOk("System error", "Error", "OK", Views.CustomMessageBoxImage.Error);
                else CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
            }



            List<float> chartdata = new List<float>();
            chartdata.Add(0);
            for (int i = 0; i < Top5Movie.Count; i++)
            {
                chartdata.Add(Top5Movie[i].Revenue / 1000000);
            }

            Top5MovieData = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<float>(chartdata),
                    Title = Properties.Settings.Default.isEnglish ? "Revenue" : "Doanh thu"
                },

            };
        }



        public async Task ChangeBestSellPeriod2()
        {
            if (SelectedBestSellPeriod2 != null)
            {
                switch (SelectedBestSellPeriod2.Content.ToString())
                {
                    case "Theo năm":
                        {
                            if (SelectedBestSellTime2 != null)
                            {
                                await LoadBestSellByYear2();
                            }
                            return;
                        }
                    case "By Year":
                        {
                            if (SelectedBestSellTime2 != null)
                            {
                                await LoadBestSellByYear2();
                            }
                            return;
                        }
                    case "Theo tháng":
                        {
                            if (SelectedBestSellTime2 != null)
                            {
                                await LoadBestSellByMonth2();
                            }
                            return;
                        }
                    case "By Month":
                        {
                            if (SelectedBestSellTime2 != null)
                            {
                                await LoadBestSellByMonth2();
                            }
                            return;
                        }
                }
            }
        }
        public async Task LoadBestSellByYear2()
        {
            if (SelectedBestSellTime2.Length != 4) return;
            try
            {
                Top5Product = await Task.Run(() => StatisticsService.Ins.GetTop5BestProductByYear(int.Parse(SelectedBestSellTime2)));
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                Console.WriteLine(e);
                if (Properties.Settings.Default.isEnglish) CustomMessageBox.ShowOk("Unable to connect to database", "Error", "OK", Views.CustomMessageBoxImage.Error);
                else CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (Properties.Settings.Default.isEnglish) CustomMessageBox.ShowOk("System error", "Error", "OK", Views.CustomMessageBoxImage.Error);
                else CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
            }

            List<float> chartdata = new List<float>();
            chartdata.Add(0);
            for (int i = 0; i < Top5Product.Count; i++)
            {
                chartdata.Add(Top5Product[i].Revenue / 1000000);
            }

            Top5FoodData = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<float>(chartdata),
                    Title = Properties.Settings.Default.isEnglish ? "Revenue" : "Doanh thu"
                },

            };
        }
        public async Task LoadBestSellByMonth2()
        {
            if (SelectedBestSellTime2.Length == 4) return;
            try
            {
                if (SelectedBestSellTime2 == "January") SelectedBestSellTime2 = "Tháng 1";
                if (SelectedBestSellTime2 == "February") SelectedBestSellTime2 = "Tháng 2";
                if (SelectedBestSellTime2 == "March") SelectedBestSellTime2 = "Tháng 3";
                if (SelectedBestSellTime2 == "April") SelectedBestSellTime2 = "Tháng 4";
                if (SelectedBestSellTime2 == "May") SelectedBestSellTime2 = "Tháng 5";
                if (SelectedBestSellTime2 == "June") SelectedBestSellTime2 = "Tháng 6";
                if (SelectedBestSellTime2 == "July") SelectedBestSellTime2 = "Tháng 7";
                if (SelectedBestSellTime2 == "August") SelectedBestSellTime2 = "Tháng 8";
                if (SelectedBestSellTime2 == "September") SelectedBestSellTime2 = "Tháng 9";
                if (SelectedBestSellTime2 == "October") SelectedBestSellTime2 = "Tháng 10";
                if (SelectedBestSellTime2 == "November") SelectedBestSellTime2 = "Tháng 11";
                if (SelectedBestSellTime2 == "December") SelectedBestSellTime2 = "Tháng 12";
                Top5Product = await Task.Run(() => StatisticsService.Ins.GetTop5BestProductByMonth(int.Parse(SelectedBestSellTime2.Remove(0, 6))));

            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                Console.WriteLine(e);
                if (Properties.Settings.Default.isEnglish) CustomMessageBox.ShowOk("Unable to connect to database", "Error", "OK", Views.CustomMessageBoxImage.Error);
                else CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (Properties.Settings.Default.isEnglish) CustomMessageBox.ShowOk("System error", "Error", "OK", Views.CustomMessageBoxImage.Error);
                else CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
            }

            List<float> chartdata = new List<float>();
            chartdata.Add(0);
            for (int i = 0; i < Top5Product.Count; i++)
            {
                chartdata.Add(Top5Product[i].Revenue / 1000000);
            }

            Top5FoodData = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = new ChartValues<float>(chartdata),
                    Title = Properties.Settings.Default.isEnglish ? "Revenue" : "Doanh thu"
                },
            };
        }
    }
}
