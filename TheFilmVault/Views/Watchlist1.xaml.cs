namespace TheFilmVault.Views;

public partial class Watchlist : ContentPage
{
    public Watchlist()
    {
        InitializeComponent();
    }

    // Event handlers for button clicks
    private void OnMoviesButtonClicked(object sender, EventArgs e)
    {
        // Set the MainPage to MoviesView
        App.Current.MainPage = new MovieView();
    }

    private void OnHomeButtonClicked(object sender, EventArgs e)
    {
        // Set the MainPage to HomePage
        App.Current.MainPage = new AppStartPage();
    }

    private void OnAccountButtonClicked(object sender, EventArgs e)
    {
        // Set the MainPage to AccountPage
        App.Current.MainPage = new AccountPage();
    }
}
