using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class AppStartPage : ContentPage
{
	public AppStartPage()
	{
		InitializeComponent();
        
        APIs.movies.Clear();
        moviesOptions.ItemsSource = APIs.movies;
	}

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

    private void goMovies(object sender, EventArgs e) { App.Current.MainPage = new MovieExplore(); }
    private void goWatchlist(object sender, EventArgs e) { App.Current.MainPage = new Watchlist(); }
    private void goAccount(object sender, EventArgs e) { App.Current.MainPage = new AccountPage(); }
}