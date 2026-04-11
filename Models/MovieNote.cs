namespace MatteoAPP1.Models
{
    public sealed class MovieNote
    {
        public string MovieKey { get; init; } = string.Empty;

        public int Rating { get; init; }

        public string Review { get; init; } = string.Empty;
    }
}
