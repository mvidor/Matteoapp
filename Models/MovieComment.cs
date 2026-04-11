namespace MatteoAPP1.Models
{
    public sealed class MovieComment
    {
        public string Id { get; init; } = string.Empty;

        public string MovieKey { get; init; } = string.Empty;

        public int Rating { get; init; }

        public string Comment { get; init; } = string.Empty;
    }
}
