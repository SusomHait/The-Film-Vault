using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Timers;
using System.Text.Json;
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

	public class CarouselModel : BindableObject, INotifyPropertyChanged
	{
		private int _currentPosition;
		private readonly System.Timers.Timer _timer;
		public int CurrentPosition
		{
			get => _currentPosition;
			set
			{
				_currentPosition = value;
				OnPropertyChanged();
			}
		}
		public ICommand goRight { get; }
		public ICommand goLeft { get; }
		public CarouselModel()
		{
			_timer = new System.Timers.Timer(5000);
			_timer.Elapsed += OnTimedEvent;
			_timer.AutoReset = true;
			_timer.Enabled = true;

			goRight = new Command(() => goNext());
			goLeft = new Command(() => goPrev());
		}
		private void goNext()
		{
			if (CurrentPosition == 5) CurrentPosition = 0;
			else CurrentPosition++;
		}
		private void goPrev()
		{
			if (CurrentPosition == 0) CurrentPosition = 5;
			else CurrentPosition--; 
		}
		private void OnTimedEvent(object sender, ElapsedEventArgs e)
		{
			MainThread.BeginInvokeOnMainThread(() => goNext());
		}
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		public void Dispose()
		{
			_timer?.Stop();
			_timer?.Dispose();
		}
	}

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
		(BindingContext as CarouselModel)?.Dispose();
    }

    public MovieExplore()
	{
		InitializeComponent();
		BindingContext = new CarouselModel();
		loadCarousel();
	}
}