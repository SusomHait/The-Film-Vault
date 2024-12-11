using System.Text.Json;
using System.Collections.ObjectModel;
using TheFilmVault.Models;
using System.Windows.Input;

namespace TheFilmVault.Views;

public partial class Watchlist : ContentPage
{
    public ICommand moveToWatch { get; }
    public ICommand deleteFromWatch { get; }
    public ICommand deleteFromWatched { get; }
    public Themes pageTheme { get; set; }
    public ObservableCollection<ListEntry> listElements = new ObservableCollection<ListEntry>();

	public Watchlist()
	{
		InitializeComponent();
        listSelector.SelectedItem = "Watchlist";

        toWatchList.ItemsSource = listElements;
        watchedList.ItemsSource = listElements;

        moveToWatch = new Command<ListEntry>(dbMoveToWatch);
        deleteFromWatch = new Command<ListEntry>(dbDeleteFromWatch);
        deleteFromWatched = new Command<ListEntry>(dbDeleteFromWatched);

        pageTheme = new Themes();
        BindingContext = this;
    }

    private void checkStatus(string type)
    {
        HttpClient client = new HttpClient();
        int id = Preferences.Default.Get("userID", -1); 
        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/fetch_list?id={id}&list={type}").GetAwaiter().GetResult();

        int return_code = (int)response.StatusCode;
        switch (return_code)
        {
            case 200:
                loadList(response, type);
                break;
            case 201:
                toWatch.IsVisible = false;
                watched.IsVisible = false;

                noItemMsg.IsVisible = true;
                noItemMsg.Text = "Nothing yet on your " + type + "!";
                break;
            default:
                toWatch.IsVisible = false;
                watched.IsVisible = false;

                noItemMsg.IsVisible = true;
                noItemMsg.Text = "Something went wrong.";
                break;   
        }
    }

    private async void loadList(HttpResponseMessage response, string type)
    {
        listElements.Clear();

        string json = await response.Content.ReadAsStringAsync();
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            var root = doc.RootElement;
            foreach (JsonElement item in root.EnumerateArray())
            {
                listElements.Add(new ListEntry
                {
                    movieID = item.GetProperty("movie_id").GetInt64(),
                    movieTitle = item.GetProperty("title").GetString(),
                    runtime = item.GetProperty("length").GetInt32()
                });
            }
        }

        if (type == "Watchlist")
        {
            toWatch.IsVisible = true;
            watched.IsVisible = false;
        }
        else
        {
            watched.IsVisible = true;
            toWatch.IsVisible = false;
        }
        noItemMsg.IsVisible = false;
    }

    private void dbMoveToWatch(ListEntry caller)
    {
        HttpClient client = new HttpClient();
        int id = Preferences.Default.Get("userID", -1);
        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/update_entry?id={id}&movie_id={caller.movieID}&list_type=Watched").GetAwaiter().GetResult();

        int return_code = (int)response.StatusCode;
        switch (return_code)
        {
            case 200:
                foreach(ListEntry item in listElements)
                {
                    if (item.movieID == caller.movieID)
                    {
                        listElements.Remove(item);
                        break;
                    }
                }
                if (listElements.Count == 0)
                {
                    toWatch.IsVisible = false;
                    noItemMsg.IsVisible = true;
                    noItemMsg.Text = "Nothing yet on your Watchlist!";
                }
                break;
            default:
                DisplayAlert("Error", "An Error Occured. Please Try Again Later.", "OK");
                break;
        }
    }

    private void dbDeleteFromWatch(ListEntry caller)
    {
        HttpClient client = new HttpClient();
        int id = Preferences.Default.Get("userID", -1);
        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/delete_entry?id={id}&movie_id={caller.movieID}").GetAwaiter().GetResult();

        int return_code = (int)response.StatusCode;
        switch (return_code)
        {
            case 200:
                foreach (ListEntry item in listElements)
                {
                    if (item.movieID == caller.movieID)
                    {
                        listElements.Remove(item);
                        break;
                    }
                }
                if (listElements.Count == 0)
                {
                    toWatch.IsVisible = false;
                    noItemMsg.IsVisible = true;
                    noItemMsg.Text = "Nothing yet on your Watchlist!";
                }
                break;
            default:
                DisplayAlert("Error", "An Error Occured. Please Try Again Later.", "OK");
                break;
        }
    }

    private void dbDeleteFromWatched(ListEntry caller)
    {
        HttpClient client = new HttpClient();
        int id = Preferences.Default.Get("userID", -1);
        HttpResponseMessage response = client.GetAsync($"https://thefilmvault.pythonanywhere.com/delete_entry?id={id}&movie_id={caller.movieID}").GetAwaiter().GetResult();

        int return_code = (int)response.StatusCode;
        switch (return_code)
        {
            case 200:
                foreach (ListEntry item in listElements)
                {
                    if (item.movieID == caller.movieID)
                    {
                        listElements.Remove(item);
                        break;
                    }
                }
                if (listElements.Count == 0)
                {
                    watched.IsVisible = false;
                    noItemMsg.IsVisible = true;
                    noItemMsg.Text = "Nothing yet on your Watched!";
                }
                break;
            default:
                DisplayAlert("Error", "An Error Occured. Please Try Again Later.", "OK");
                break;
        }
    }

    // Event handlers for button clicks
    private void OnMoviesButtonClicked(object sender, EventArgs e)
    {
        // Set the MainPage to MoviesView
        App.Current.MainPage = new MovieExplore();
    }

    private void OnHomeButtonClicked(object sender, EventArgs e)
    {
        // Set the MainPage to HomePage
        App.Current.MainPage = new AppStartPage();
    }

    private void OnAccountButtonClicked(object sender, EventArgs e)
    {
        // Set the MainPage to AccountPage
        App.Current.MainPage = new AccountPage();
    }

    private void changeList(object sender, EventArgs e)
    {
        if ((string)listSelector.SelectedItem == "Watchlist")
        {
            checkStatus("Watchlist");
        }
        else
        {
            checkStatus("Watched");
        }
    }
}