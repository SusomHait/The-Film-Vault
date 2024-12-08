using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class MovieView : ContentPage
{
	public Themes pageTheme { get; set; }
	public Movie current { get; }

	public MovieView(Movie caller)
	{
		InitializeComponent();
		getDetails(caller.movieId);
		current = caller;

		//IMG.Source = caller.posterPath;
		DESC.Text = caller.movieDesc;
		RATING.Text = caller.movieRating;

		pageTheme = new Themes();
		BindingContext = this;
	}

	private async void getDetails(long ID)
	{
		HttpClient client = new HttpClient();
		HttpResponseMessage response = await client.GetAsync($"https://thefilmvault.pythonanywhere.com/details?id={ID}");

		string json = await response.Content.ReadAsStringAsync();
	}

    private void startSearch(object sender, EventArgs e)
    {

    }

    private void removeSearchOptions(object sender, EventArgs e)
    {

    }

    private void populateResults(object sender, TextChangedEventArgs e)
    {

    }
}