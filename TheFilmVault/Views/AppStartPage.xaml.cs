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

        signInButton.Text = Preferences.Default.Get("username", "Sign In");

        goMovieView = new Command<Movie>(openMoviePage);
        pageTheme = new Themes();
        BindingContext = this;
	}

    // Search Bar Functionality
    private async void searchOptions(string input)
    {
        APIs.movies.Clear();
        await APIs.getMovieData($"https://thefilmvault.pythonanywhere.com/search?query={input}&adult={Preferences.Default.Get("show_adult", "false")}");
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
    private void goWatchlist(object sender, EventArgs e) 
    {
        if (Preferences.Default.Get("logged_in", false))
        {
            App.Current.MainPage = new Watchlist();
        }
        else
        {
            App.Current.MainPage = new Intercept();
        }
    }
    private void goAccount(object sender, EventArgs e) 
    { 
        if (Preferences.Default.Get("logged_in", false))
        {
            App.Current.MainPage = new AccountPage();
            
        }
        else
        {
            App.Current.MainPage = new Intercept();
        }
    }
}