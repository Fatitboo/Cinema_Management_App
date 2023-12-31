﻿using CinemaManagementProject.DTOs;
using CinemaManagementProject.Model.Service;
using CinemaManagementProject.View.Admin.ShowtimeManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CinemaManagementProject.ViewModel.AdminVM.ShowtimeManagementVM
{
    public partial class ShowtimeMangementViewModel : BaseViewModel
    {
        private List<SeatSettingDTO> _ListSeat;
        public List<SeatSettingDTO> ListSeat
        {
            get { return _ListSeat; }
            set { _ListSeat = value; OnPropertyChanged(); }
        }
        private ObservableCollection<SeatSettingDTO> _ListSeat1;
        public ObservableCollection<SeatSettingDTO> ListSeat1
        {
            get { return _ListSeat1; }
            set { _ListSeat1 = value; OnPropertyChanged(); }
        }
        private ObservableCollection<SeatSettingDTO> _ListSeat2;
        public ObservableCollection<SeatSettingDTO> ListSeat2
        {
            get { return _ListSeat2; }
            set { _ListSeat2 = value; OnPropertyChanged(); }
        }

        private Infor_EditShowtimeWindow _EditShowtimeWindow;
        public Infor_EditShowtimeWindow EditShowtimeWindow
        {
            get { return _EditShowtimeWindow; }
            set { _EditShowtimeWindow = value; }
        }

        private ObservableCollection<ShowtimeDTO> _ListShowtimeofMovie;
        public ObservableCollection<ShowtimeDTO> ListShowtimeofMovie
        {
            get { return _ListShowtimeofMovie; }
            set { _ListShowtimeofMovie = value; OnPropertyChanged(); }
        }

        private int _IsBought;
        public int IsBought
        {
            get { return _IsBought; }
            set { _IsBought = value; OnPropertyChanged(); }
        }
        private int _IsFree;
        public int IsFree
        {
            get { return _IsFree; }
            set { _IsFree = value; OnPropertyChanged(); }
        }
        private int _isBooked;
        public int isBooked
        {
            get { return _isBooked; }
            set { _isBooked = value; OnPropertyChanged(); }
        }
        private int _isReady;
        public int isReady
        {
            get { return _isReady; }
            set { _isReady = value; OnPropertyChanged(); }
        }
        public ICommand LoadInfor_EditShowtime { get; set; }
        public ICommand CloseEditCM { get; set; }
        public ICommand LoadSeatCM { get; set; }
        public ICommand EditPriceCM { get; set; }


        private ShowtimeDTO _selectedShowtime; //the showtime being selected
        public ShowtimeDTO SelectedShowtime
        {
            get { return _selectedShowtime; }
            set
            {
                _selectedShowtime = value;
                OnPropertyChanged();
            }
        }


        public void Infor_EditFunc()
        {
            if (SelectedItem != null)
            {
                Infor_EditShowtimeWindow p = new Infor_EditShowtimeWindow();
                LoadDataEditWindow(p);
                EditShowtimeWindow = p;
                oldSelectedItem = SelectedItem;
                //ShadowMask.Visibility = System.Windows.Visibility.Visible;
                ListSeat1 = new ObservableCollection<SeatSettingDTO>();
                ListSeat2 = new ObservableCollection<SeatSettingDTO>();
                IsFree = IsBought = 0;
                p.ShowDialog();
            }
        }


        public void LoadDataEditWindow(Infor_EditShowtimeWindow p)
        {
            p._movieName.Text = SelectedItem.FilmName;
            p._ShowtimeDate.Text = SelectedDate.ToString("dd-MM-yyyy");
            if (SelectedRoomId != -1)
                p._ShowtimeRoom.Text = SelectedRoomId.ToString();
           
           ListShowtimeofMovie = new ObservableCollection<ShowtimeDTO>(SelectedItem.ShowTimes);
  
            moviePrice = 0;
        }
        public async Task GenerateSeat()
        {
            try
            {
                ListSeat = new List<SeatSettingDTO>(await SeatService.Ins.GetSeatsByShowtime(SelectedShowtime.Id));
            }
            catch (System.Data.Entity.Core.EntityException e)
            {
                Console.WriteLine(e);
                CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi","OK", Views.CustomMessageBoxImage.Error);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
                throw;
            }

            ListSeat1 = new ObservableCollection<SeatSettingDTO>();
            ListSeat2 = new ObservableCollection<SeatSettingDTO>();
            IsBought = 0;
            IsFree = 0;
            
            //foreach (var item in ListSeat)
            //{
            //    if (item.SeatPosition.Length == 2 && item.SeatPosition[1] < '3')
            //    {
            //        ListSeat2.Add(item);
            //    }
            //    else
            //    {
            //        ListSeat1.Add(item);
            //    }
            //    if (item.SeatStatus == true)
            //        IsBought++;
            //}
            //IsFree = ListSeat.Count - IsBought;
            foreach (var item in ListSeat)
            {
                if (item.SeatPosition.Length == 2 && item.SeatPosition[1] < '4')
                {
                    ListSeat1.Add(item);
                }
                else
                {
                    ListSeat2.Add(item);
                }
            }


        }
    }
}
