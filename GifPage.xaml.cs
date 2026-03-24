namespace MatteoAPP1
{
    public partial class GifPage : ContentPage
    {
        public GifPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            BackButton.IsEnabled = false;
            StatusLabel.Text = "Lecture du GIF en cours...";

            await Task.Delay(5000);

            BackButton.IsEnabled = true;
            StatusLabel.Text = "La scene est lancee, tu peux revenir a l'accueil.";
        }

        private async void OnBackClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
