using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class GenrePage : ContentPage
{
	public GenrePage(Genre caller)
	{
		InitializeComponent();

		loadGenreOptions(caller.genreId);
        dynamicTitle.Text = $"{caller.genreName.Trim()}";
	}

	private readonly HttpClient client = new HttpClient();

    public async void loadGenreOptions(int genre_key)
    {
        ObservableCollection<Movie> cv = new ObservableCollection<Movie>();

        try
        {
            var KEY = await SecureStorage.GetAsync("API_KEY");
            string url = $"https://api.themoviedb.org/3/discover/movie?include_adult=false&language=en-US&page=1&sort_by=popularity.desc&with_genres={genre_key}";
            
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", KEY);
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;
                var nowPlaying = root.GetProperty("results");

                int count = 0;
                foreach (JsonElement movie in nowPlaying.EnumerateArray())
                {
                    if (count > 5) break;
                    long id = movie.GetProperty("id").GetInt64();
                    string? title = movie.GetProperty("title").GetString();
                    string? path = movie.GetProperty("poster_path").GetString();
                    string? desc = movie.GetProperty("overview").GetString();
                    string? rating = movie.GetProperty("vote_average").GetDouble().ToString();

                    cv.Add(new Movie { movieId = id, movieTitle = title, backdropPath = path, movieDesc = desc, movieRating = rating });
                    count++;
                }
            }
        }
        catch (Exception)
        {
            Debug.WriteLine("Error in Getting Data");
        }
        genreGrid.ItemsSource = cv;
    }
}