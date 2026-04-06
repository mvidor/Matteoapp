using MatteoAPP1.Models;
using Microsoft.Maui.Controls;

namespace MatteoAPP1.ViewModels
{
    public sealed class CharacterDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private const string CharacterQueryKey = "character";
        private string _name = "Personnage";
        private string _description = string.Empty;
        private string _imageUrl = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set
            {
                if (SetProperty(ref _imageUrl, value))
                {
                    OnPropertyChanged(nameof(DisplayImageSource));
                    OnPropertyChanged(nameof(HasImage));
                    OnPropertyChanged(nameof(HasNoImage));
                }
            }
        }

        public ImageSource? DisplayImageSource => ImageSourceHelper.Resolve(ImageUrl);

        public bool HasImage => DisplayImageSource is not null;

        public bool HasNoImage => !HasImage;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue(CharacterQueryKey, out var value) || value is not DemonSlayerCharacter character)
            {
                Name = "Personnage";
                Description = "Aucune description disponible.";
                ImageUrl = string.Empty;
                return;
            }

            Name = string.IsNullOrWhiteSpace(character.Name) ? "Personnage" : character.Name;
            Description = string.IsNullOrWhiteSpace(character.Description)
                ? "Aucune description disponible."
                : character.Description;
            ImageUrl = character.ImageUrl;
        }
    }
}
