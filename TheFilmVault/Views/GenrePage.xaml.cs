using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class GenrePage : ContentPage
{
    private readonly HttpClient client = new HttpClient();
    private int current_page = 1;
    private int page_genre;
    private ObservableCollection<Movie> cv = new ObservableCollection<Movie>();

    public GenrePage(Genre caller)
	{
        page_genre = caller.genreId;
        InitializeComponent();

		getOptions();
        dynamicTitle.Text = $"{caller.genreName.Trim()}";

        genreGrid.ItemsSource = cv;
	}

    private async void loadMore(object sender, EventArgs e)
    {
        current_page++;
        if (current_page <= 500) getOptions();
    }

    public async void getOptions()
    {
        try
        {
            var KEY = await SecureStorage.GetAsync("API_KEY");
            string url = $"https://api.themoviedb.org/3/discover/movie?include_adult=false&language=en-US&page={current_page}&sort_by=popularity.desc&with_genres={page_genre}";
            
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
                    long id = movie.GetProperty("id").GetInt64();
                    string? title = movie.GetProperty("title").GetString();
                    string? path = movie.GetProperty("poster_path").GetString();
                    string? desc = movie.GetProperty("overview").GetString();
                    string? rating = movie.GetProperty("vote_average").GetDouble().ToString();

                    cv.Add(new Movie { movieId = id, movieTitle = title, posterPath = path, movieDesc = desc, movieRating = rating });
                    count++;
                }
            }
        }
        catch (Exception)
        {
            Debug.WriteLine("Error in Getting Data");
        }
    }
}