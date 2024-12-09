using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.Json;

namespace TheFilmVault.Models
{
    public class APIs : BindableObject, INotifyPropertyChanged
    {
        public static ObservableCollection<Movie> movies = new ObservableCollection<Movie>();

        public async static Task<bool> getMovieData(string url, int return_count = 20)
        {
            try
            {
                string json = await returnJSON(url);

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    var options = root.GetProperty("results");

                    int count = 0;
                    foreach (JsonElement movie in options.EnumerateArray())
                    {
                        if (count >= return_count) break;
                            
                        long id = movie.GetProperty("id").GetInt64();
                        string? title = movie.GetProperty("title").GetString();
                        string? b_path = movie.GetProperty("backdrop_path").GetString();
                        string? p_path = movie.GetProperty("poster_path").GetString();
                        string? desc = movie.GetProperty("overview").GetString();
                        string? rating = movie.GetProperty("vote_average").GetDouble().ToString();

                        movies.Add(new Movie {
                            movieId = id, 
                            movieTitle = title, 
                            backdropPath = b_path, 
                            posterPath = p_path, 
                            movieDesc = desc, 
                            movieRating = rating 
                        });
                            
                        count++;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                Debug.WriteLine("Error in API fetch.");
                return false;
            }
        }

        public static ObservableCollection<Genre> genres = new ObservableCollection<Genre>();

        public async static Task<bool> getGenreList()
        {
            try
            {
                string json = await returnJSON("https://thefilmvault.pythonanywhere.com/genre_list");

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;
                    var options = root.GetProperty("genres");

                    foreach (JsonElement element in options.EnumerateArray())
                    {
                        int id = element.GetProperty("id").GetInt32();
                        string? genre_name = element.GetProperty("name").GetString();

                        genres.Add(new Genre { 
                            genreId = id, 
                            genreName = genre_name 
                        });
                    }
                }
                return true; 
            }
            catch (Exception)
            {
                Debug.WriteLine("Error in JSON parse.");
                return false;
            }
        }

        private static async Task<string> returnJSON(string url)
        {
            try
            {
                HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                Debug.WriteLine("Error in API fetch.");
                return "error";
            }
        }
    }
}
