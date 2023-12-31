﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using CinemaManagementProject.DTOs;
using CinemaManagementProject.Model.Service;
using CinemaManagementProject.Utils;
using System.Globalization;

namespace CinemaManagementProject.ViewModel.AdminVM.MovieManagementVM
{
    public partial class MovieManagementVM : BaseViewModel
    {
        public ICommand LoadAddMovieCM { get; set; }
        public async Task SaveMovieFunc(Window p)
        {
            if (filepath != null && IsValidData())
            {
                string movieImage = await CloudinaryService.Ins.UploadImage(filepath);

                if (movieImage is null)
                {
                    
                    CustomMessageBox.ShowOk(Properties.Settings.Default.isEnglish? "An error occurred while saving the image. Please try again" : "Lỗi phát sinh trong quá trình lưu ảnh. Vui lòng thử lại", Properties.Settings.Default.isEnglish ? "Notification":"Thông báo", "OK", Views.CustomMessageBoxImage.Warning);
                    return;
                }

                FilmDTO film = new FilmDTO
                {   // check ở đây
                    FilmName = filmName,
                    Country = filmCountry,
                    Author = filmDirector,
                    FilmInfor = filmDescribe,
                    Image = movieImage,
                    Genre = filmGenre,
                    ReleaseDate = DateTime.ParseExact($"01/01/{filmYear}", "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    DurationFilm = int.Parse(filmDuration),
                    FilmType = filmType,
                };

                (bool successAddMovie, string messageFromAddMovie, FilmDTO newMovie) = await FilmService.Ins.AddMovie(film);

                if (successAddMovie)
                {
                    isSaving = false;
                    CustomMessageBox.ShowOk(messageFromAddMovie, Properties.Settings.Default.isEnglish?"Notification":"Thông báo", "OK", Views.CustomMessageBoxImage.Success);
                    //LoadMovieListView(Operation.CREATE, newMovie);
                    ReloadListView();
                    p.Close();
                }
                else
                {
                    CustomMessageBox.ShowOk(messageFromAddMovie, Properties.Settings.Default.isEnglish ? "Error" : "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
                }
            }
            else
            {
                CustomMessageBox.ShowOk(Properties.Settings.Default.isEnglish ? "Please enter enough information!" : "Vui lòng nhập đủ thông tin!", Properties.Settings.Default.isEnglish ? "Warning" : "Cảnh báo", "OK", Views.CustomMessageBoxImage.Warning);
            }
        }
    }
}
