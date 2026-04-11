namespace MatteoAPP1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public List<HomeSlide> Slides { get; } =
        [
            new("tanjiro.webp", "Tanjiro", "Le heros principal affronte les demons avec la respiration solaire.", Color.FromArgb("#F8F5F1")),
            new("nezuko.jpg", "Nezuko", "Sa force et sa protection de son frere marquent les scenes les plus fortes.", Color.FromArgb("#F6F1F7")),
            new("zenitsu.jpg", "Zenitsu", "Il combat avec une vitesse fulgurante et la respiration de la foudre.", Color.FromArgb("#FFF7ED")),
            new("inousuke.jpg", "Inosuke", "Il fonce au combat avec ses lames et son temperament sauvage.", Color.FromArgb("#EEF2FF"))
        ];

        private async void OnGifButtonClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(GifPage));
        }
    }
}
