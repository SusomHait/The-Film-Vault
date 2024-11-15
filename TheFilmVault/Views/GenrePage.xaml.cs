using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class GenrePage : ContentPage
{
    private int current_page = 1;
    private int page_genre;

    public GenrePage(Genre caller)
	{
        InitializeComponent();
        initPage(caller);
    }

    private void initPage(Genre caller)
    {
        page_genre = caller.genreId;
        dynamicTitle.Text = $"{caller.genreName.Trim()}";

        APIs.movies.Clear();
        APIs.getMovieData($"https://api.themoviedb.org/3/discover/movie?include_adult=false&language=en-US&page={current_page}&sort_by=popularity.desc&with_genres={page_genre}");

        genreGrid.ItemsSource = APIs.movies;
    }

    private async void loadMore(object sender, EventArgs e)
    {
        current_page++;
        string url = $"https://api.themoviedb.org/3/discover/movie?include_adult=false&language=en-US&page={current_page}&sort_by=popularity.desc&with_genres={page_genre}";
        if (current_page <= 500) await APIs.getMovieData(url);
    }
}