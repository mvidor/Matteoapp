using System.Windows.Input;
using MatteoAPP1.Models;
using MatteoAPP1.Services;

namespace MatteoAPP1.ViewModels
{
    public sealed class AddCharacterViewModel : BaseViewModel
    {
        private readonly ICharacterCatalogService _catalogService;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _imageUrl = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private bool _isSubmitting;

        public AddCharacterViewModel(ICharacterCatalogService catalogService)
        {
            _catalogService = catalogService;
            AddCharacterCommand = new Command(async () => await AddCharacterAsync(), () => !IsSubmitting);
        }

        public ICommand AddCharacterCommand { get; }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string ImageUrl
        {
            get => _imageUrl;
            set => SetProperty(ref _imageUrl, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (SetProperty(ref _errorMessage, value))
                {
                    OnPropertyChanged(nameof(HasError));
                }
            }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                if (SetProperty(ref _successMessage, value))
                {
                    OnPropertyChanged(nameof(HasSuccess));
                }
            }
        }

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public bool HasSuccess => !string.IsNullOrWhiteSpace(SuccessMessage);

        public bool IsSubmitting
        {
            get => _isSubmitting;
            set
            {
                if (SetProperty(ref _isSubmitting, value) && AddCharacterCommand is Command command)
                {
                    command.ChangeCanExecute();
                }
            }
        }

        private async Task AddCharacterAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Title) ||
                string.IsNullOrWhiteSpace(Description) ||
                string.IsNullOrWhiteSpace(ImageUrl))
            {
                ErrorMessage = "Merci de saisir un titre, une description et une image.";
                return;
            }

            try
            {
                IsSubmitting = true;

                await _catalogService.AddCharacterAsync(new DemonSlayerCharacter
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Name = Title.Trim(),
                    ImageUrl = ImageUrl.Trim(),
                    CustomDescription = Description.Trim(),
                    Race = "Creation utilisateur",
                    Gender = "Inconnu",
                    Affiliation = "Ajout manuel"
                });

                Title = string.Empty;
                Description = string.Empty;
                ImageUrl = string.Empty;
                SuccessMessage = "Item ajoute. Il est maintenant visible dans la liste des personnages.";

                await Shell.Current.GoToAsync("//CharactersPage");
            }
            catch
            {
                ErrorMessage = "Impossible d'ajouter l'item pour le moment.";
            }
            finally
            {
                IsSubmitting = false;
            }
        }
    }
}
