using System.ComponentModel;

namespace TheFilmVault.Models
{
    public class Movie : BindableObject, INotifyPropertyChanged
    {
        private string? backdrop_path;
        private string? poster_path;
        private string? rating;

        public long movieId { get; set; }

        public string? movieTitle { get; set; }

        public string? backdropPath
        {
            get { return backdrop_path; }
            set
            {
                backdrop_path = "https://image.tmdb.org/t/p/w1280" + value;
            }
        }

        public string? posterPath
        {
            get { return poster_path; }
            set
            {
                poster_path = "https://image.tmdb.org/t/p/w500" + value;
            }
        }

        public string? movieDesc { get; set; }

        public string? movieRating
        {
            get { return rating; }
            set
            {
                rating = value + "/10";
            }
        }
    }

    public class Genre : BindableObject, INotifyPropertyChanged
    {
        public int genreId { get; set; }
        public string? genreName { get; set; }
    }

    public class ThemeOptions : BindableObject, INotifyPropertyChanged
    {
        public static string path = Path.Combine(FileSystem.AppDataDirectory, "session.txt");
        public ThemeOptions(string? selection = null)
        {
            if (selection == null || selection == "dark")
            {
                backing = Color.Parse("#151515");
                accent = Color.Parse("#FFFFFF");
            }
            else if (selection == "light")
            {
                backing = Color.Parse("#FFFFFF");
                accent = Color.Parse("#151515");
            }
        }

        public static Color backing { get; set; }
        public static Color accent { get; set; }
    }
}
