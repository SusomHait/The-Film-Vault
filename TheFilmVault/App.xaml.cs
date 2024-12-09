﻿using System.Collections.ObjectModel;
using TheFilmVault.Models;

namespace TheFilmVault
{
    public partial class App : Application
    {
        public static ObservableCollection<Movie> catalog = new ObservableCollection<Movie>();
        public static readonly int carousel_count = 8;

        public App()
        {
            InitializeComponent();
            loadElements();

            // init theme if it doesn't exist
            if (!Preferences.Default.ContainsKey("theme"))
            {
                Preferences.Default.Set("theme", "dark");
            }
            if (!Preferences.Default.ContainsKey("show_adult"))
            {
                Preferences.Default.Set("show_adult", "false");
            }

            App.Current.MainPage = new Views.AppStartPage();
        }

        private async void loadElements()
        {
            APIs.movies.Clear();
            await APIs.getMovieData("https://thefilmvault.pythonanywhere.com/load_carousel", carousel_count);

            foreach (Movie element in APIs.movies)
            {
                catalog.Add(element);
            }

            APIs.genres.Clear();
            await APIs.getGenreList();
        }
    }
}
