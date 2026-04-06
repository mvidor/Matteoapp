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

        public string CustomDescription { get; init; } = string.Empty;

        public ImageSource? DisplayImageSource => ImageSourceHelper.Resolve(ImageUrl);

        public string Description => !string.IsNullOrWhiteSpace(CustomDescription)
            ? CustomDescription
            : $"{Name} est un personnage de l'univers Demon Slayer. " +
              $"Race : {Race}. Genre : {Gender}. Affiliation : {Affiliation}.";

        public bool HasImage => DisplayImageSource is not null;

        public bool HasNoImage => !HasImage;
    }
}
