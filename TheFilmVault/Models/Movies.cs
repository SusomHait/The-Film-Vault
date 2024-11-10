using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TheFilmVault.Models
{
    public class Movies
    {
        public int id { get; set; }
        public string title { get; set; } = "";
        public string poster_path { get; set; } = "none";
        public decimal rating { get; set; }
        public string description { get; set; } = "";
    }
}
