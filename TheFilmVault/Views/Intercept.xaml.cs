using System.Text.Json;
using TheFilmVault.Models;

namespace TheFilmVault.Views
{
    public partial class Intercept : ContentPage
    {
        public Themes pageTheme { get; set; }

        public Intercept()
        {
            InitializeComponent();

            pageTheme = new Themes();
            BindingContext = this;
        }

        // Show Create Account Form
        private void OnCreateAccountClicked(object sender, EventArgs e)
        {
            // Hide main stack and show Create Account form
            MainStack.IsVisible = false;
            CreateAccountStack.IsVisible = true;
            LoginStack.IsVisible = false;
        }

        // Show Login Form
        private void OnLoginClicked(object sender, EventArgs e)
        {
            // Hide main stack and show Login form
            MainStack.IsVisible = false;
            LoginStack.IsVisible = true;
            CreateAccountStack.IsVisible = false;
        }

        // Handle Create Account submission
        private async void OnCreateAccountSubmit(object sender, EventArgs e)
        {
            var username = CreateUsername.Text?.Trim();
            var password = CreatePassword.Text?.Trim();

            if (string.IsNullOrEmpty(username))
            {
                createErr.Text = "Username cannot be empty.";
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                createErr.Text = "Password cannot be empty.";
                return;
            }

            HttpResponseMessage response = await callDB($"https://thefilmvault.pythonanywhere.com/create_account?username={username}&pswd={password}");
            int return_code = (int)response.StatusCode;

            switch (return_code)
            {
                case 200:
                    string json = await response.Content.ReadAsStringAsync();

                    using(JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;

                        Preferences.Default.Set("logged_in", true);
                        Preferences.Default.Set("username", username);
                        Preferences.Default.Set("password", password);
                        Preferences.Default.Set("userID", root.GetProperty("id").GetInt32());
                    }

                    App.Current.MainPage = new AccountPage();
                    break;
                case 400:
                    createErr.Text = "Username taken. Please, select another username.";
                    break;
                default:
                    createErr.Text = "Account Creation Error. Please try again later.";
                    break;
            }
        }

        // Handle Login submission
        private async void OnLoginSubmit(object sender, EventArgs e)
        {
            var username = LoginUsername.Text?.Trim();
            var password = LoginPassword.Text?.Trim();

            if (string.IsNullOrEmpty(username))
            {
                loginErr.Text = "Username cannot be empty.";
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                loginErr.Text = "Password cannot be empty.";
                return;
            }

            HttpResponseMessage response = await callDB($"https://thefilmvault.pythonanywhere.com/login?username={username}&pswd={password}");
            int return_code = (int)response.StatusCode;

            switch (return_code)
            {
                case 200:
                    string json = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;

                        Preferences.Default.Set("logged_in", true);
                        Preferences.Default.Set("username", username);
                        Preferences.Default.Set("password", password);

                        Preferences.Default.Set("userID", root.GetProperty("id").GetInt32());
                        Preferences.Default.Set("show_adult", root.GetProperty("show_adult").GetString());
                        Preferences.Default.Set("theme", root.GetProperty("theme").GetString());
                    }

                    App.Current.MainPage = new AccountPage();
                    break;
                case 400:
                    loginErr.Text = "Incorrect Password";
                    break;
                case 401:
                    loginErr.Text = "Username not found.";
                    break;
                default:
                    loginErr.Text = "Login Error. Please try again later.";
                    break;
            }
        }

        // Reset UI to show the main stack
        private void ResetToMainStack()
        {
            MainStack.IsVisible = true;
            CreateAccountStack.IsVisible = false;
            LoginStack.IsVisible = false;

            // Clear form fields
            CreateUsername.Text = string.Empty;
            CreatePassword.Text = string.Empty;
            LoginUsername.Text = string.Empty;
            LoginPassword.Text = string.Empty;
        }

        private void reset(object sender, EventArgs e)
        {
            ResetToMainStack();
        }

        private async Task<HttpResponseMessage> callDB(string url)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            return response;
        }
    }
}