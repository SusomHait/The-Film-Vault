using System.Windows.Input;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class AccountPage : ContentPage
{
    public ICommand goMoviePage { get; }
    public Themes pageTheme { get; set; }

    public AccountPage()
	{
		InitializeComponent();
        pageTheme = new Themes();

        welcomeMsg.Text = "Welcome: " + Preferences.Default.Get("username", "none");
		userEntry.Text = Preferences.Default.Get("username", "none");
		pswdEntry.Text = Preferences.Default.Get("password", "none");
        themeSelector.SelectedItem = Preferences.Default.Get("theme", "dark");

		if (Preferences.Default.Get("show_adult", "false") == "true")
		{
			contentFilter.IsChecked = true;
        }
		else
		{
            contentFilter.IsChecked = false;
        }

        APIs.movies.Clear();
        moviesOptions.ItemsSource = APIs.movies;

        goMoviePage = new Command<Movie>(openMoviePage);

        BindingContext = this;
    }

	private void OnMoviesButtonClicked(object sneder, EventArgs e)
	{
		App.Current.MainPage = new MovieExplore();
	}

	private void OnHomeButtonClicked(object sender, EventArgs e)
	{
		App.Current.MainPage = new AppStartPage();
	}

	private void OnWatchlistButtonClicked(object sender, EventArgs e)
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

    private void openMoviePage(Movie calling_movie)
    {
        App.Current.MainPage = new MovieView(calling_movie);
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

    private void showPass(object sender, CheckedChangedEventArgs e)
    {
		pswdEntry.IsPassword = !showPswd.IsChecked;
    }

    private void changeFilter(object sender, CheckedChangedEventArgs e)
    {
		if (contentFilter.IsChecked)
		{
			Preferences.Default.Set("show_adult", "true");
		}
		else
		{
            Preferences.Default.Set("show_adult", "false");
        }
    }

    private void changeTheme(object sender, EventArgs e)
    {
		string selected = (string)themeSelector.SelectedItem;

        Preferences.Default.Set("theme", selected);
		pageTheme.refresh();
    }

    private void signOut(object sender, EventArgs e)
    {
		HttpClient client = new HttpClient();

		int id = Preferences.Default.Get("userID", -1);
		string adult = Preferences.Default.Get("show_adult", "false");
		string theme = Preferences.Default.Get("theme", "dark");

        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/sign_out?id={id}&show_adult={adult}&theme={theme}").GetAwaiter().GetResult();

		int return_code = (int)response.StatusCode;

		switch (return_code)
		{
			case 200:
				resetDefault();
				App.Current.MainPage = new AppStartPage();
				break;
			default:
				DisplayAlert("Error", "Error Signing Out", "OK");
				break;
		}
    }

    private void delAcc(object sender, EventArgs e)
    {
        HttpClient client = new HttpClient();
        int id = Preferences.Default.Get("userID", -1);
        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/delete_account?id={id}").GetAwaiter().GetResult();

        int return_code = (int)response.StatusCode;

        switch (return_code)
        {
            case 200:
                resetDefault();
                App.Current.MainPage = new AppStartPage();
                break;
            default:
                DisplayAlert("Error", "Error Signing Out", "OK");
                break;
        }
    }

    private void resetDefault()
	{
        Preferences.Default.Set("logged_in", false);
        Preferences.Default.Set("userID", -1);
        Preferences.Default.Set("username", "Sign In");
        Preferences.Default.Set("password", "none");
        Preferences.Default.Set("theme", "dark");
        Preferences.Default.Set("show_adult", "false");
    }
}