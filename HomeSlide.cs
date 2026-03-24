namespace MatteoAPP1
{
    public sealed class HomeSlide
    {
        public HomeSlide(string imageSource, string title, string description, Color cardColor)
        {
            ImageSource = imageSource;
            Title = title;
            Description = description;
            CardColor = cardColor;
        }

        public string ImageSource { get; }

        public string Title { get; }

        public string Description { get; }

        public Color CardColor { get; }
    }
}
