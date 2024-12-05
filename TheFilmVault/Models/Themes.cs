using System.ComponentModel;

namespace TheFilmVault.Models
{
    public class Themes : BindableObject, INotifyPropertyChanged
    {
        public Color background { get; set; }
        public Color accent { get; set; }
        public Color highlight { get; set; }

        public Themes()
        {
            if (Preferences.Default.Get("theme", "dark") == "dark")
            {
                loadDark();
            }
            else
            {
                loadLight();
            }
        }

        private void loadDark()
        {
            background = Color.FromArgb("#151515");
            accent = Color.Parse("#FFFFFF");
            highlight = Color.Parse("#f3c045");
        }

        private void loadLight()
        {
            background = Color.Parse("#f2e8cf");
            accent = Color.Parse("#353535");
            highlight = Color.Parse("#bc4749");
        } 

        private void refresh()
        {
            OnPropertyChanged(nameof(background));
            OnPropertyChanged(nameof(accent));
            OnPropertyChanged(nameof(highlight));
        }
    }
}
