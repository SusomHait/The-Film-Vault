using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TheFilmVault.Models
{
    public class Movie : BindableObject, INotifyPropertyChanged
    {
        private string? backdrop_path;
        private string? rating;

        public long movieId { get; set; }
        public string? movieTitle { get; set; }
        public string? backdropPath
        {
            get { return backdrop_path; }
            set
            {
                backdrop_path = "https://image.tmdb.org/t/p/original" + value;
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
}
