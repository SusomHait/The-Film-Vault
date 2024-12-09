using System.Timers;
using System.Windows.Input;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class MovieExplore : ContentPage
{
    public ICommand goGenrePage { get; }
    public ICommand goMoviePage { get; }
    public Themes pageTheme { get; set; }

    public MovieExplore()
    {
        InitializeComponent();

        newMovies.ItemsSource = App.catalog;
        genreList.ItemsSource = APIs.genres;

        APIs.movies.Clear();
        moviesOptions.ItemsSource = APIs.movies;

        goGenrePage = new Command<Genre>(openGenrePage);
        goMoviePage = new Command<Movie>(openMoviePage);

        pageTheme = new Themes();
        BindingContext = this;

        initTimer();
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

    // navigation to genre pages
    private void openGenrePage(Genre calling_genre)
    {
        App.Current.MainPage = new GenrePage(calling_genre);
    }

    private void openMoviePage(Movie calling_movie)
    {
        App.Current.MainPage = new MovieView(calling_movie);
    }

    // Carousel control features
    private void goRight(object sender, EventArgs e) { MainThread.BeginInvokeOnMainThread(() => scroll(true)); }

    private void goLeft(object sender, EventArgs e) { MainThread.BeginInvokeOnMainThread(() => scroll(false)); }

    public static Mutex m = new Mutex();
    private void scroll(bool direction)
    {
        m.WaitOne();
        int current = newMovies.Position;
        int nextIndex;

        if (direction) nextIndex = (current + 1) % App.carousel_count;
        else
        {
            if (current > 0) nextIndex = current - 1;
            else nextIndex = -1;
        }

        if (nextIndex >= 0 && nextIndex <= App.carousel_count - 1)
        {
            prevButton.IsEnabled = false;
            nextButton.IsEnabled = false;

            resetTimer();
            newMovies.ScrollTo(nextIndex);

            prevButton.IsEnabled = true;
            nextButton.IsEnabled = true;
        }
        m.ReleaseMutex();
    }

    // Carousel autoplay functionality
    private System.Timers.Timer _timer = new System.Timers.Timer(5000);

    private void initTimer()
    {
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = false;
        _timer.Start();
    }

    private void resetTimer()
    {
        _timer.Stop();
        _timer.Start();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e) { MainThread.BeginInvokeOnMainThread(() => scroll(true)); }

    protected override void OnDisappearing()
    {
        _timer?.Stop();
        _timer?.Dispose();
        base.OnDisappearing();
    }
}