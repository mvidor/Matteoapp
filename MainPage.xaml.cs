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
            new("tanjiro_card.svg", "Tanjiro", "Le heros principal affronte les demons avec la respiration solaire.", Color.FromArgb("#F8F5F1")),
            new("nezuko_card.svg", "Nezuko", "Sa force et sa protection de son frere marquent les scenes les plus fortes.", Color.FromArgb("#F6F1F7")),
            new("movie_tab.svg", "Le film", "Le carrousel met en avant les personnages et l'ambiance du dernier film.", Color.FromArgb("#F3F4F6"))
        ];

        private async void OnGifButtonClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(GifPage));
        }
    }
}
