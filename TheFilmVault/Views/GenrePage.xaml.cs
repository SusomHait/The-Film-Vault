using System.Windows.Input;
using System.Collections.ObjectModel;
using TheFilmVault.Models;
using System;
using System.Text.Json;

namespace TheFilmVault.Views;

public partial class GenrePage : ContentPage
{
    public ICommand goMoviePage { get; }
    public Themes pageTheme { get; set; }
    public ObservableCollection<Movie> genre_options = new ObservableCollection<Movie>();

    private int current_page = 1;
    private int page_genre;

    public GenrePage(Genre caller)
	{
        InitializeComponent();
        initPage(caller);

        goMoviePage = new Command<Movie>(openMoviePage);

        APIs.movies.Clear();
        moviesOptions.ItemsSource = APIs.movies;

        pageTheme = new Themes();
        BindingContext = this;
    }

    private void initPage(Genre caller)
    {
        page_genre = caller.genreId;
        dynamicTitle.Text = $"{caller.genreName.Trim()}";
        localDriver();

        //APIs.getMovieData($"https://thefilmvault.pythonanywhere.com/genre_search?genre={page_genre}&adult={Preferences.Default.Get("show_adult", "false")}&page={current_page}");

        genreGrid.ItemsSource = genre_options;
    }

    private async void loadMore(object sender, EventArgs e)
    {
        current_page++;
        //string url = $"https://thefilmvault.pythonanywhere.com/genre_search?genre={page_genre}&adult={Preferences.Default.Get("show_adult", "false")}&page={current_page}";
        if (current_page <= 500) localDriver();
        //await APIs.getMovieData(url);
    }

    private void localDriver()
    {
        HttpClient client = new HttpClient();
        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/genre_search?genre={page_genre}&adult={Preferences.Default.Get("show_adult", "false")}&page={current_page}").GetAwaiter().GetResult();
        string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            var root = doc.RootElement;
            var options = root.GetProperty("results");

            foreach (JsonElement movie in options.EnumerateArray())
            {

                long id = movie.GetProperty("id").GetInt64();
                string? title = movie.GetProperty("title").GetString();
                string? b_path = movie.GetProperty("backdrop_path").GetString();
                string? p_path = movie.GetProperty("poster_path").GetString();
                string? desc = movie.GetProperty("overview").GetString();
                string? rating = movie.GetProperty("vote_average").GetDouble().ToString();

                genre_options.Add(new Movie
                {
                    movieId = id,
                    movieTitle = title,
                    backdropPath = b_path,
                    posterPath = p_path,
                    movieDesc = desc,
                    movieRating = rating
                });
            }
        }
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

    private void goExplore(object sender, EventArgs e)
    {
        App.Current.MainPage = new MovieExplore();
    }
}