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
        APIs.getMovieData($"https://thefilmvault.pythonanywhere.com/genre_search?genre={page_genre}&adult={Preferences.Default.Get("show_adult", "false")}&page={current_page}");

        genreGrid.ItemsSource = APIs.movies;
    }

    private async void loadMore(object sender, EventArgs e)
    {
        current_page++;
        string url = $"https://thefilmvault.pythonanywhere.com/genre_search?genre={page_genre}&adult={Preferences.Default.Get("show_adult", "false")}&page={current_page}";
        if (current_page <= 500) await APIs.getMovieData(url);
    }
}