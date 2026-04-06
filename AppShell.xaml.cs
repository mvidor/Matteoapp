namespace MatteoAPP1
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(GifPage), typeof(GifPage));
            Routing.RegisterRoute(nameof(CharacterDetailPage), typeof(CharacterDetailPage));
        }
    }
}
