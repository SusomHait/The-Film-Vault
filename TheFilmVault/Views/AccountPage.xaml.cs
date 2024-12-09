namespace TheFilmVault.Views;

public partial class AccountPage : ContentPage
{
	public AccountPage()
	{
		InitializeComponent();
	}

	private void OnMoviesButtonClicked(object sneder, EventArgs e)
	{
		App.Current.MainPage = new MovieView();
	}

	private void OnHomeButtonClicked(object sender, EventArgs e)
	{
		App.Current.MainPage = new AppStartPage();
	}

	private void OnWatchlistButtonClicked(object sender, EventArgs e)
	{
		App.Current.MainPage = new Watchlist();
	}
}