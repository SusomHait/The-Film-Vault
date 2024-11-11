using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Timers;
using System.Text.Json;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Maui.Controls.Internals;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using Microsoft.Maui.ApplicationModel;

namespace TheFilmVault.Views;

public class CarouselItem: BindableObject, INotifyPropertyChanged
{
	private string? img_path;
	private string? rating;
	public string? movieTitle { get; set; }
	public string? posterPath
	{
		get { return img_path; }
		set
		{
			img_path = "https://image.tmdb.org/t/p/original" + value;
		}
	}
	public string? movieDesc { get; set; }
	public string? movieRating
	{
		get { return rating; }
		set
		{
			rating = value + "/10";
		}
	}
}

public partial class MovieExplore : ContentPage
{
    private readonly HttpClient client = new HttpClient();

	public async void loadCarousel()
	{
        ObservableCollection<CarouselItem> cv = new ObservableCollection<CarouselItem>();

        try
		{
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "");
			HttpResponseMessage response = await client.GetAsync("https://api.themoviedb.org/3/movie/now_playing?language=en-US&page=1");
			response.EnsureSuccessStatusCode();

			string json = await response.Content.ReadAsStringAsync();
			
			using (JsonDocument doc = JsonDocument.Parse(json))
			{
				var root = doc.RootElement;
				var nowPlaying = root.GetProperty("results");

				int count = 0;
				foreach (JsonElement movie in nowPlaying.EnumerateArray())
				{
					if (count > 5) break;
					string? title = movie.GetProperty("title").GetString();
					string? path = movie.GetProperty("backdrop_path").GetString();
					string? desc = movie.GetProperty("overview").GetString();
					string? rating = movie.GetProperty("vote_average").GetDouble().ToString();

					Console.WriteLine("{0} {1} {2} {3}", title, path, desc, rating);
					cv.Add(new CarouselItem { movieTitle = title, posterPath = path, movieDesc = desc, movieRating = rating });
					count++;
				}
			}
		}
		catch (Exception)
		{
            Console.WriteLine("Error in Getting Data");
        }

		newMovies.ItemsSource = cv;
    }
    public MovieExplore()
	{
		InitializeComponent();
		loadCarousel();
		initTimer();
	}

    private async void goRight(object sender, EventArgs e)
    {
		int current = newMovies.Position;
		int nextIndex = (current + 1) % 6;

		nextButton.IsEnabled = false;
		resetTimer();
        await asyncScroll(nextIndex);
		nextButton.IsEnabled = true;
    }

    private async void goLeft(object sender, EventArgs e)
    {
        int current = newMovies.Position;
		
		if (current > 0)
		{
			prevButton.IsEnabled = false;
			resetTimer();
            await asyncScroll(current - 1);
            prevButton.IsEnabled = true;
        }
    }
	
	public static Mutex m = new Mutex();
	private async Task asyncScroll(int pos)
	{
		m.WaitOne();
		if (pos >= 0 && pos <= 5)
		{
			newMovies.ScrollTo(pos);
		}
		m.ReleaseMutex();
	}

	private System.Timers.Timer _timer;
	private readonly int autoplay_time = 5000;

    private void initTimer()
	{
		_timer = new System.Timers.Timer(autoplay_time);
		_timer.Elapsed += OnTimerElapsed;
		_timer.AutoReset = false;
		_timer.Start();
	}

    private void resetTimer()
    {
        _timer.Stop();
        _timer.Start();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            int current = newMovies.Position;
            int nextIndex = (current + 1) % 6;

            nextButton.IsEnabled = false;
			newMovies.ScrollTo(nextIndex);
            nextButton.IsEnabled = true;

            _timer.Start();
        });
    }

    protected override void OnDisappearing()
	{
		_timer?.Stop();
		_timer?.Dispose();
		base.OnDisappearing();
	}

}