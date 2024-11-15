using System.Timers;
using System.Windows.Input;
using TheFilmVault.Models;

namespace TheFilmVault.Views;

public partial class MovieExplore : ContentPage
{
    public ICommand goGenrePage { get; }
    private readonly int carousel_count = 10;

    // init app functions through class constructor
    public MovieExplore()
    {
        InitializeComponent();
        loadPageElements();
        initTimer();

        goGenrePage = new Command<Genre>(openGenrePage);
        BindingContext = this;
    }

    public async void loadPageElements()
    {
        APIs.movies.Clear();
        await APIs.getMovieData("https://api.themoviedb.org/3/movie/now_playing?language=en-US&page=1", carousel_count);
        newMovies.ItemsSource = APIs.movies;

        APIs.genres.Clear();
        await APIs.getGenreList();
        genreList.ItemsSource = APIs.genres;
    }

    // navigation to genre pages
    private void openGenrePage(Genre calling_genre)
    {
        App.Current.MainPage = new Views.GenrePage(calling_genre);
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

        if (direction) nextIndex = (current + 1) % carousel_count; 
        else
        {
            if (current > 0) nextIndex = current - 1;
            else nextIndex = -1;
        }

		if (nextIndex >= 0 && nextIndex <= carousel_count - 1)
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