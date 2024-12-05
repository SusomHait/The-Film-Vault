using System.Windows.Input;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class AppStartPage : ContentPage
{
    public ICommand goMovieView { get; }
    public Themes pageTheme { get; set; }


    public AppStartPage()
	{
		InitializeComponent();
        
        APIs.movies.Clear();
        moviesOptions.ItemsSource = APIs.movies;
        
        goMovieView = new Command<Movie>(openMoviePage);
        pageTheme = new Themes();
        BindingContext = this;
	}

    // Search Bar Functionality
    private async void searchOptions(string input)
    {
        APIs.movies.Clear();
        await APIs.getMovieData($"https://api.themoviedb.org/3/search/movie?query={input}&include_adult=false&language=en-US&page=1");
    }

    private void populateResults(object sender, TextChangedEventArgs e)
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

    // navigate to search bar selection
    private void openMoviePage(Movie callingMovie)
    {
        App.Current.MainPage = new MovieView(callingMovie);
    }

    private void goMovies(object sender, EventArgs e) { App.Current.MainPage = new MovieExplore(); }
    private void goWatchlist(object sender, EventArgs e) { App.Current.MainPage = new Watchlist(); }
    private void goAccount(object sender, EventArgs e) { App.Current.MainPage = new AccountPage(); }
}