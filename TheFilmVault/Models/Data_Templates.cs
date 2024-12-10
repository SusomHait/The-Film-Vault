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

    public class ExtraDetails : BindableObject, INotifyPropertyChanged
    {
        public string? tagline { get; set; }
        public string? release_date { get; set; }
        public string? runtime { get; set; }
        public string? videoPath { get; set; }

    }

    public class Genre : BindableObject, INotifyPropertyChanged
    {
        public int genreId { get; set; }
        public string? genreName { get; set; }
    }
}
