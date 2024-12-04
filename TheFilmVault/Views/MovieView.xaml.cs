using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class MovieView : ContentPage
{
	public MovieView(Movie caller)
	{
		InitializeComponent();

		IMG.Source = caller.posterPath;
		DESC.Text = caller.movieDesc;
		RATING.Text = caller.movieRating;
	}
}