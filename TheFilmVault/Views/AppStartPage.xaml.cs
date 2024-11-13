using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class AppStartPage : ContentPage
{
	public AppStartPage()
	{
		InitializeComponent();
	}

    private readonly HttpClient client = new HttpClient();

    public async void searchOptions(string input)
    {
        ObservableCollection<Movie> cv = new ObservableCollection<Movie>();

        try
        {
            var KEY = await SecureStorage.GetAsync("API_KEY");

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", KEY);
            HttpResponseMessage response = await client.GetAsync($"https://api.themoviedb.org/3/search/movie?query={input}&include_adult=false&language=en-US&page=1");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                var root = doc.RootElement;
                var options = root.GetProperty("results");

                int count = 0;
                foreach (JsonElement movie in options.EnumerateArray())
                {
                    if (count >= 10) break;
                    long id = movie.GetProperty("id").GetInt64();
                    string? title = movie.GetProperty("title").GetString();
                    string? path = movie.GetProperty("backdrop_path").GetString();
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

        moviesOptions.ItemsSource = cv;
    }

    private void goMovies(object sender, EventArgs e)
    {
		App.Current.MainPage = new MovieExplore();
    }

    private void goWatchlist(object sender, EventArgs e)
    {
        App.Current.MainPage = new Watchlist();
    }

    private void goAccount(object sender, EventArgs e)
    {
        App.Current.MainPage = new AccountPage();
    }

    private async void populateResults(object sender, TextChangedEventArgs e)
    {
        if (searchEntry.Text == null || searchEntry.Text.Length == 0)
        {
            searchGrid.IsVisible = false;
            focusButton.IsVisible = false;
            moviesOptions.IsVisible = false;
        }
        else
        {
            searchOptions(searchEntry.Text.Trim());
            searchGrid.IsVisible = true;
            focusButton.IsVisible = true;
            moviesOptions.IsVisible = true;
        }
    }

    private void removeSearchOptions(object sender, EventArgs e)
    {
        searchGrid.IsVisible = false;
        focusButton.IsVisible = false;
        moviesOptions.IsVisible = false;
    }
}