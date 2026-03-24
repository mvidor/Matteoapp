namespace MatteoAPP1.Models
{
    public sealed class DemonSlayerCharacter
    {
        public string Id { get; init; } = string.Empty;

        public string Name { get; init; } = "Personnage inconnu";

        public string Race { get; init; } = "Inconnue";

        public string Gender { get; init; } = "Inconnu";

        public string Affiliation { get; init; } = "Non precisee";

        public string ImageUrl { get; init; } = string.Empty;
    }
}
