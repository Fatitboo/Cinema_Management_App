﻿using CinemaManagementProject.DTOs;
using CinemaManagementProject.Model.Service;
using CinemaManagementProject.Utilities;
using CinemaManagementProject.Utils;
using CinemaManagementProject.View.Admin;
using CinemaManagementProject.View.Login;
using CinemaManagementProject.ViewModel.AdminVM;


using CinemaManagementProject.View.Staff;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using CinemaManagementProject.View;

namespace CinemaManagementProject.ViewModel.LoginVM
{
    public class LoginVM : BaseViewModel
    {
        public Window LoginWindow { get; set; }
        public static string Password { get; set; }
        public string _username { get; set; }
        public string Username
        {
            get
            {
                return _username;
            }
            set { _username = value; OnPropertyChanged(); }
        }
        public string _error { get; set; }
        public string Error
        {
            get
            {
                return _error;
            }
            set { _error = value; OnPropertyChanged(); }
        }
        private StaffDTO _currentStaff { get; set; }
        public StaffDTO CurrentStaff
        {
            get { return _currentStaff; }
            set
            {
                _currentStaff = value;
            }
        }
        private string _staffPosition { get; set; }
        public string StaffPosition
        {
            get { return _staffPosition; }
            set
            {
                _staffPosition = value;
            }
        }
        private string _currentStaffName { get; set; }
        public string CurrentStaffName
        {
            get { return _currentStaffName; }
            set
            {
                _currentStaffName = value;
            }
        }

        public bool isEN = Properties.Settings.Default.isEnglish;
        public static Frame MainFrame { get; set; }
        public ICommand LoadLoginPageCM { get; set; }
        public ICommand LoadForgotPageCM { get; set; }
        public ICommand LoginCM { get; set; }
        public ICommand PasswordChangedCM { get; set; }
        public ICommand SaveLoginWindowNameCM { get; set; }
        public ICommand ToWorkingWindowCM { get; set; }
        public ICommand LoginPageCM { get; set; } //  dùng để load frame login
        public LoginVM()
        {
            LoadLoginPageCM = new RelayCommand<Frame>((p) => { return true; }, (p) =>
            {
                MainFrame = p;
                if (Properties.Settings.Default.isRemidUserAndPass)
                {
                    Username = Properties.Settings.Default.userNameSetting;
                    Password = Properties.Settings.Default.userPassSetting;
                }
                else
                {
                    Username = "";
                    Password = "";
                }

                p.Content = new LoginPage();
            });
            LoadForgotPageCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                MainFrame.Content = new ForgotPassPage();
            });
            PasswordChangedCM = new RelayCommand<PasswordBox>((p) => { return true; }, (p) =>
            {
                Password = p.Password;
            });
            LoginCM = new RelayCommand<Label>((p) => { return true; }, async (p) =>
            {
                await CheckValidateAccount(Username, Password, p);
            });
            SaveLoginWindowNameCM = new RelayCommand<Window>((p) => { return true; }, (p) =>
            {
                if (Properties.Settings.Default.isEnglish == true)
                    LanguageManager.SetLanguageDictionary(LanguageManager.ELanguage.English);
                LoginWindow = p;
            });
            ToWorkingWindowCM = new RelayCommand<Button>((p) => { return true; }, (p) =>
            {
                if (CurrentStaff.Position == "Quản lý")
                {
                    LoginWindow.Hide();
                    AdminVM.AdminVM.currentStaff = CurrentStaff;
                    AdminWindow stWD = new AdminWindow();
                    stWD.Show();
                    LoginWindow.Close();
                }
                else
                    if (CurrentStaff.Position == "Nhân viên")
                {
                    LoginWindow.Hide();
                    StaffVM.StaffVM.currentStaff = CurrentStaff;
                    StaffWindow stWD = new StaffWindow();
                    stWD.Show();
                    LoginWindow.Close();
                }
            });
            LoginPageCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                MainFrame.Content = new LoginPage();
                if (Properties.Settings.Default.isRemidUserAndPass)
                {
                    Username = Properties.Settings.Default.userNameSetting;
                    Password = Properties.Settings.Default.userPassSetting;
                }
                else
                {
                    Username = "";
                    Password = "";
                }
            });
        }
        public async Task CheckValidateAccount(string userNameOrEmail, string passWord, Label lableError)
        {
            if (string.IsNullOrEmpty(userNameOrEmail) || string.IsNullOrEmpty(passWord))
            {
                lableError.Content = isEN ? "Please enter enough information" : "Vui lòng nhập đủ thông tin";
                return;
            }

            (bool loginSuccess, string message, StaffDTO staff) = await Task<(bool loginSuccess, string message, StaffDTO staff)>.Run(() => StaffService.Ins.Login(userNameOrEmail, passWord));
            CurrentStaff = staff;
            if (loginSuccess)
            {
                Password = "";
                StaffPosition = CurrentStaff.Position;
                CurrentStaffName = CurrentStaff.StaffName;
                MainFrame.Content = new PositionPage();
            }
            else
            {
                lableError.Content = message;
                return;
            }
        }
    }
}
