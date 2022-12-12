﻿using CinemaManagementProject.DTOs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CinemaManagementProject.Model.Service
{
    public class MovieService
    {
        private MovieService() { }

        private static MovieService _ins;
        public static MovieService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new MovieService();
                }
                return _ins;
            }
            private set => _ins = value;
        }

        public async Task<List<MovieDTO>> GetAllMovie()
        {
            List<MovieDTO> movies = null;

            try
            {
                using (var context = new CinemaManagementProjectEntities())
                {
                    movies = await (from movie in context.Films
                                    where !movie.IsDeleted
                                    select new MovieDTO
                                    {
                                        Id = movie.Id,
                                        FilmName = movie.FilmName,
                                        Duration = movie.Duration,
                                        Country = movie.Country,
                                        FilmInfo = movie.FilmInfo,
                                        ReleaseDate = movie.ReleaseDate,
                                        FilmType = movie.FilmType,
                                        Author = movie.Author,
                                        //Image = movie.Image,
                                        //Genre = (from genre in movie.Genres
                                        //          select new GenreDTO { DisplayName = genre.DisplayName, Id = genre.Id }
                                        //        ).ToList(),
                                    }
                     ).ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return movies;
        }
        //public async Task<List<MovieDTO>> GetShowingMovieByDay(DateTime date)
        //{
        //    List<MovieDTO> movieList = new List<MovieDTO>();
        //    try
        //    {
        //        using (var context = new CinemaManagementProjectEntities())
        //        {
        //            var MovieIdList = await (from showSet in context.ShowtimeSettings
        //                                     where DbFunctions.TruncateTime(showSet.ShowDate) == date.Date
        //                                     select showSet into S
        //                                     from show in S.Showtimes
        //                                     select new
        //                                     {
        //                                         MovieId = show.MovieId,
        //                                         ShowTime = show,
        //                                     }).GroupBy(m => m.MovieId).ToListAsync();
        //            for (int i = 0; i < MovieIdList.Count(); i++)
        //            {
        //                int id = MovieIdList[i].Key;

        //                List<ShowtimeDTO> showtimeDTOsList = new List<ShowtimeDTO>();
        //                MovieDTO mov = null;
        //                foreach (var m in MovieIdList[i])
        //                {
        //                    showtimeDTOsList.Add(new ShowtimeDTO
        //                    {
        //                        Id = m.ShowTime.Id,
        //                        MovieId = m.ShowTime.MovieId,
        //                        StartTime = m.ShowTime.StartTime,
        //                        RoomId = m.ShowTime.ShowtimeSetting.RoomId,
        //                        ShowDate = m.ShowTime.ShowtimeSetting.ShowDate,
        //                        TicketPrice = m.ShowTime.TicketPrice
        //                    });
        //                    if (mov is null)
        //                    {
        //                        Movie movie = m.ShowTime.Movie;

        //                        if (movie is null)
        //                        {
        //                            movie = await context.Movies.FindAsync(m.ShowTime.MovieId);
        //                        }
        //                        mov = new MovieDTO
        //                        {
        //                            Id = movie.Id,
        //                            DisplayName = movie.DisplayName,
        //                            RunningTime = movie.RunningTime,
        //                            Country = movie.Country,
        //                            Description = movie.Description,
        //                            ReleaseYear = movie.ReleaseYear,
        //                            MovieType = movie.MovieType,
        //                            Director = movie.Director,
        //                            Image = movie.Image,
        //                            Genres = (from genre in movie.Genres
        //                                      select new GenreDTO { DisplayName = genre.DisplayName, Id = genre.Id }
        //                                             ).ToList(),
        //                        };
        //                    }
        //                }
        //                movieList.Add(mov);
        //                movieList[i].Showtimes = showtimeDTOsList.OrderBy(s => s.StartTime).ToList();
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    return movieList;
        //}
    }
}
