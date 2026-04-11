using System.Windows.Input;
using MatteoAPP1.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using MatteoAPP1.Services;

namespace MatteoAPP1.ViewModels
{
    public sealed class AddCharacterViewModel : BaseViewModel
    {
        private readonly ICharacterCatalogService _catalogService;
        private string _title = string.Empty;
        private string _description = string.Empty;
        private string _imagePath = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private bool _isSubmitting;

        public AddCharacterViewModel(ICharacterCatalogService catalogService)
        {
            _catalogService = catalogService;
            PickImageCommand = new Command(async () => await PickImageAsync(), () => !IsSubmitting);
            AddCharacterCommand = new Command(async () => await AddCharacterAsync(), () => !IsSubmitting);
        }

        public ICommand PickImageCommand { get; }

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

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (SetProperty(ref _imagePath, value))
                {
                    OnPropertyChanged(nameof(HasImage));
                    OnPropertyChanged(nameof(ImagePreviewSource));
                    OnPropertyChanged(nameof(ImageLabel));
                }
            }
        }

        public string ImageLabel => HasImage ? Path.GetFileName(ImagePath) : "Aucune image selectionnee";

        public bool HasImage => !string.IsNullOrWhiteSpace(ImagePath);

        public ImageSource? ImagePreviewSource => ImageSourceHelper.Resolve(ImagePath);

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
                if (SetProperty(ref _isSubmitting, value))
                {
                    if (AddCharacterCommand is Command addCommand)
                    {
                        addCommand.ChangeCanExecute();
                    }

                    if (PickImageCommand is Command pickCommand)
                    {
                        pickCommand.ChangeCanExecute();
                    }
                }
            }
        }

        private async Task PickImageAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Choisir une image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result is null)
                {
                    return;
                }

                await using var sourceStream = await result.OpenReadAsync();
                var extension = Path.GetExtension(result.FileName);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    extension = ".jpg";
                }

                var localFileName = $"{Guid.NewGuid():N}{extension}";
                var localPath = Path.Combine(FileSystem.AppDataDirectory, localFileName);

                await using var destinationStream = File.Create(localPath);
                await sourceStream.CopyToAsync(destinationStream);

                ImagePath = localPath;
            }
            catch
            {
                ErrorMessage = "Impossible de selectionner cette image pour le moment.";
            }
        }

        private async Task AddCharacterAsync()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Title) ||
                string.IsNullOrWhiteSpace(Description) ||
                string.IsNullOrWhiteSpace(ImagePath))
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
                    ImageUrl = ImagePath,
                    CustomDescription = Description.Trim(),
                    Race = "Creation utilisateur",
                    Gender = "Inconnu",
                    Affiliation = "Ajout manuel"
                });

                Title = string.Empty;
                Description = string.Empty;
                ImagePath = string.Empty;
                SuccessMessage = "Personnage ajoute avec une image locale.";

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
