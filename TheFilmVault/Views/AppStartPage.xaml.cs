namespace TheFilmVault.Views;

public partial class AppStartPage : ContentPage
{
	public AppStartPage()
	{
		InitializeComponent();
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
}