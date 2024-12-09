namespace TheFilmVault.Views
{
    public partial class Intercept : ContentPage
    {
        public Intercept()
        {
            InitializeComponent();
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

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Username and password cannot be empty.", "OK");
                return;
            }

            // Navigate to MainPage after account creation
            await NavigateToMainPage();
        }

        // Handle Login submission
        private async void OnLoginSubmit(object sender, EventArgs e)
        {
            var username = LoginUsername.Text?.Trim();
            var password = LoginPassword.Text?.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Username and password cannot be empty.", "OK");
                return;
            }

            // Navigate to MainPage after login
            await NavigateToMainPage();
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

        // Navigate to MainPage.xaml
        private async Task NavigateToMainPage()
        {
            await Navigation.PushAsync(new AppStartPage());
        }
    }
}