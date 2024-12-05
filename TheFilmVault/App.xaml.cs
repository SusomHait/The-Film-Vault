using System.Collections.ObjectModel;
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

            App.Current.MainPage = new Views.AppStartPage();
        }

        private async void loadElements()
        {
            APIs.movies.Clear();
            await APIs.getMovieData("https://api.themoviedb.org/3/movie/now_playing?language=en-US&page=1", carousel_count);

            foreach (Movie element in APIs.movies)
            {
                catalog.Add(element);
            }

            APIs.genres.Clear();
            await APIs.getGenreList();
        }
    }
}
