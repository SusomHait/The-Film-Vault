using System.Text.Json;
using System.Windows.Input;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class MovieView : ContentPage
{
    public ICommand goMoviePage { get; }
    public Themes pageTheme { get; set; }
	public Movie current { get; set; }
    public ExtraDetails? extra { get; set; }

	public MovieView(Movie caller)
	{
		InitializeComponent();
        current = caller;
        getDetails(caller.movieId);

        APIs.movies.Clear();
        moviesOptions.ItemsSource = APIs.movies;

        goMoviePage = new Command<Movie>(openMoviePage);

        pageTheme = new Themes();
        BindingContext = this;
	}

    // grab extra details
    private void getDetails(long ID)
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/details?id={ID}").GetAwaiter().GetResult();

        string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        string? tagline, runtime, releaseDate;
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            var root = doc.RootElement;

            tagline = root.GetProperty("tagline").GetString();
            runtime = root.GetProperty("runtime").GetInt32().ToString();
            releaseDate = root.GetProperty("release_date").GetString();
        }

        response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/video?id={ID}").GetAwaiter().GetResult();
        json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        string key = "none";
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            var root = doc.RootElement;
            var options = root.GetProperty("results");

            foreach (JsonElement video in options.EnumerateArray())
            {
                if (video.GetProperty("site").GetString() == "YouTube")
                {
                    key = video.GetProperty("key").GetString();
                    break;
                }
            }
        }

        if (key == "none")
        {
            vidPlayer.IsVisible = false;
            key = "";
        }
        else
        {
            backup.IsVisible = false;
            key = "https://www.youtube.com/embed/" + key;
        }

        extra = new ExtraDetails
        {
            tagline = tagline,
            runtime = runtime + " mins.",
            release_date = "Released: " + releaseDate,
            videoPath = key
        };
    }

    // Search Bar Functionality
    private async void searchOptions(string input)
    {
        APIs.movies.Clear();
        await APIs.getMovieData($"https://thefilmvault.pythonanywhere.com/search?query={input}&adult={Preferences.Default.Get("show_adult", "false")}");
    }

    private void startSearch(object sender, EventArgs e)
    {
        searchGrid.IsVisible = true;
        searchEntry.Text = null;
        moviesOptions.IsVisible = false;
    }

    private void populateResults(object sender, TextChangedEventArgs e)
    {
        if (searchEntry.Text == null || searchEntry.Text.Length == 0)
        {
            moviesOptions.IsVisible = false;
        }
        else
        {
            searchOptions(searchEntry.Text.Trim());
            moviesOptions.IsVisible = true;
        }
    }

    private void removeSearchOptions(object sender, EventArgs e)
    {
        searchGrid.IsVisible = false;
        moviesOptions.IsVisible = false;
    }

    // page navigation
    private void openMoviePage(Movie calling_movie)
    {
        App.Current.MainPage = new MovieView(calling_movie);
    }

    private void watchAdd(object sender, EventArgs e)
    {
        HttpClient client = new HttpClient();

        int id = Preferences.Default.Get("userID", -1);
        string title = Uri.EscapeDataString(current.movieTitle);
        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/add_entry?id={id}&movie_id={current.movieId}&title={title}&list_type=Watchlist&movie_length={extra.runtime}").GetAwaiter().GetResult();

        int return_code = (int)response.StatusCode;
        switch (return_code)
        {
            case 200:
                DisplayAlert("Success", "Entry Added.", "OK");
                break;
            case 201:
                DisplayAlert("Existing Entry", "This Movie is Already in your Lists.", "OK");
                break;
            default:
                DisplayAlert("Error", "An Error Occured, Please Try Again Later.", "OK");
                break;
        }
    }
}