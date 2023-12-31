﻿using CinemaManagementProject.DTOs;
using CinemaManagementProject.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CinemaManagementProject.Model;
using CinemaManagementProject.View.Login;
using CinemaManagementProject.Views;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using CinemaManagementProject.View.Admin;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Markup;
using System.Security.RightsManagement;

namespace CinemaManagementProject.ViewModel.AdminVM
{
    public class AdminVM : BaseViewModel
    {
        public static StaffDTO currentStaff;
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnPropertyChanged(); }
        }
        private string _staffNameIcon { get; set; }
        public string StaffNameIcon
        {
            get
            {
                return _staffNameIcon;
            }
            set
            {
                _staffNameIcon = value;
                OnPropertyChanged();
            }
        }
        private string _staffName { get; set; }
        public string StaffName
        {
            get
            {
                return _staffName;
            }
            set
            {
                _staffName = value;
                OnPropertyChanged();
            }
        }
        private string _staffEmail { get; set; }
        public string StaffEmail
        {
            get
            {
                return _staffEmail;
            }
            set
            {
                _staffEmail = value;
                OnPropertyChanged();
            }
        }
        private Brush _mainColor { get; set; }
        public Brush MainColor 
        {
            get { return _mainColor; }
            set { _mainColor = value; OnPropertyChanged(); }
        }
        private ImageSource _avatarSource { get; set; }
        public ImageSource AvatarSource
        {
            get { return _avatarSource; }
            set { _avatarSource = value; OnPropertyChanged(); }
        }
        private bool isEN = Properties.Settings.Default.isEnglish;
        public ICommand FirstLoadCM { get; set; }
        public ICommand ReviewCommand { get; set; }
        public ICommand VoucherCommand { get; set; }
        public ICommand ShowTimeViewCommand { get; set; }
        public ICommand CustomerViewCommand { get; set; }
        public ICommand HistoryViewCommand { get; set; }
        public ICommand StaffViewCommand { get; set; }
        public ICommand FilmViewCommand { get; set; }
        public ICommand FoodCommand { get; set; }
        public ICommand TroubleCommand { get; set; }
        public ICommand StatisticalViewCommand { get; set; }
        public ICommand SettingCommand { get; set; }
        public ICommand HelpScreenCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public ICommand OpenAvatarPopupCommand { get; set; }
        public ICommand CloseAvatarPopupCommand { get; set; }
        public ICommand SwitchToSettingTab { get; set; }
        private void Review(object onj) => CurrentView = new ReviewManagementVM.ReviewManagementVM();
        private void Food(object obj) => CurrentView = new FoodManagementVM.FoodManagementVM();
        private void Voucher(object obj) => CurrentView = new VoucherManagementVM.VoucherViewModel();
        private void ShowTime(object obj) => CurrentView = new ShowtimeManagementVM.ShowtimeMangementViewModel();
        private void Customer(object obj) => CurrentView = new CustomerManagementVM.CustomerManagementViewModel();
        private void History(object obj) => CurrentView = new Import_ExportManagementVM.Import_ExportManagementViewModel();
        private void Staff(object obj) => CurrentView = new StaffManagementVM.StaffManagementViewModel();
        private void Film(object obj) => CurrentView = new MovieManagementVM.MovieManagementVM();
        public void Statistical(object obj) => CurrentView = new StatisticalManagementVM.StatisticalManagementVM();
        private void Trouble(object obj) => CurrentView = new TroubleManagementVM.TroubleManagementViewModel();
        private void Setting(object obj) => CurrentView = new SettingVM.SettingVM();

        private void HelpScreen(object obj) => CurrentView = new HelpScreenVM.HelpScreenVM();
        public AdminVM()
        {
            VoucherCommand = new RelayCommand(Voucher);
            ReviewCommand = new RelayCommand(Review);
            ShowTimeViewCommand = new RelayCommand(ShowTime);
            CustomerViewCommand = new RelayCommand(Customer);
            HistoryViewCommand = new RelayCommand(History);
            StaffViewCommand = new RelayCommand(Staff);
            FilmViewCommand = new RelayCommand(Film);
            FoodCommand = new RelayCommand(Food);
            _currentView = new VoucherManagementVM.VoucherViewModel();
            StatisticalViewCommand = new RelayCommand(Statistical);
            TroubleCommand = new RelayCommand(Trouble);
            SettingCommand = new RelayCommand(Setting);
            HelpScreenCommand = new RelayCommand(HelpScreen);
            LogOutCommand = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                isEN = Properties.Settings.Default.isEnglish;
                if (CustomMessageBox.ShowOkCancel(isEN? "Do you really want to log out?" : "Bạn thật sự muốn đăng xuất không?",isEN? "Warning" : "Cảnh báo",isEN? "Log out" : "Đăng xuất",isEN? "No" : "Không", Views.CustomMessageBoxImage.Warning) == CustomMessageBoxResult.OK)
                {
                    if (p != null)
                    {
                        LoginWindow loginwd = new LoginWindow();
                        loginwd.Show();
                        RenewData();
                        p.Close();
                    }
                }
            });
            OpenAvatarPopupCommand = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                p.Visibility = Visibility.Visible;
            });
            CloseAvatarPopupCommand = new RelayCommand<Grid>((p) => { return true; }, (p) =>
            {
                p.Visibility = Visibility.Hidden;
            });
            SwitchToSettingTab = new RelayCommand<RadioButton>((p) => { return true; }, (p) =>
            {
                p.IsChecked = true;
                CurrentView = new SettingVM.SettingVM();
            });
            FirstLoadCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                MainColor = (SolidColorBrush)new BrushConverter().ConvertFrom(Properties.Settings.Default.MainAppColor);
                if (currentStaff.Avatar != null)
                    AvatarSource = LoadAvatarImage(currentStaff.Avatar);
                else
                    AvatarSource = null;
                StaffName = currentStaff.StaffName;
                StaffEmail = currentStaff.Email;
                if (currentStaff != null)
                {
                    FormatStaffDisplayNameToIcon();
                    SetInfomationToView();
                }
                else
                {
                    StaffNameIcon = "Ad";
                    StaffName = "Admin";
                    StaffEmail = "admin@gmail.com";
                }
                CurrentView = new StatisticalManagementVM.StatisticalManagementVM();
            });
        }
        public BitmapImage LoadAvatarImage(byte[]data)
        {
            MemoryStream strm = new MemoryStream();
            strm.Write(data, 0, data.Length);
            strm.Position = 0;
            System.Drawing.Image img = System.Drawing.Image.FromStream(strm);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
        }
        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
            return returnImage;
        }
        public void FormatStaffDisplayNameToIcon()
        {
            string staffName = currentStaff.StaffName;
            string[] trimNames = staffName.Split(' ');
            StaffNameIcon = trimNames[trimNames.Length - 1][0].ToString() + trimNames[0][0].ToString();
        }
        public void SetInfomationToView()
        {
            StaffName = currentStaff.StaffName;
            StaffEmail = currentStaff.Email;
        }
        public void RenewData()
        {
            currentStaff = null;
            StaffEmail = "";
            StaffName = "";
            AvatarSource = null;
        }
    }
}
