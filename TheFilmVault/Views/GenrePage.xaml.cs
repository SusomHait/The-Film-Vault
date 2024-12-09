using System.Windows.Input;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class GenrePage : ContentPage
{
    public ICommand goMoviePage { get; }
    public Themes pageTheme { get; set; }

    private int current_page = 1;
    private int page_genre;

    public GenrePage(Genre caller)
	{
        InitializeComponent();
        initPage(caller);

        goMoviePage = new Command<Movie>(openMoviePage);

        pageTheme = new Themes();
        BindingContext = this;
    }

    private void initPage(Genre caller)
    {
        page_genre = caller.genreId;
        dynamicTitle.Text = $"{caller.genreName.Trim()}";

        APIs.movies.Clear();
        APIs.getMovieData($"https://thefilmvault.pythonanywhere.com/genre_search?genre={page_genre}&adult={Preferences.Default.Get("show_adult", "false")}&page={current_page}");

        genreGrid.ItemsSource = APIs.movies;
    }

    private async void loadMore(object sender, EventArgs e)
    {
        current_page++;
        string url = $"https://thefilmvault.pythonanywhere.com/genre_search?genre={page_genre}&adult={Preferences.Default.Get("show_adult", "false")}&page={current_page}";
        if (current_page <= 500) await APIs.getMovieData(url);
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
}