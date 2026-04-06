using System.Collections.ObjectModel;
using System.Windows.Input;
using MatteoAPP1.Models;
using MatteoAPP1.Services;

namespace MatteoAPP1.ViewModels
{
    public sealed class CharactersViewModel : BaseViewModel
    {
        private const string CharacterQueryKey = "character";
        private readonly ICharacterCatalogService _catalogService;
        private bool _isLoading;
        private string _errorMessage = string.Empty;
        private DemonSlayerCharacter? _selectedCharacter;
        private bool _needsRefresh = true;

        public CharactersViewModel(ICharacterCatalogService catalogService)
        {
            _catalogService = catalogService;
            _catalogService.CharactersChanged += (_, _) => _needsRefresh = true;
            RefreshCommand = new Command(async () => await LoadCharactersAsync());
        }

        public ObservableCollection<DemonSlayerCharacter> Characters { get; } = [];

        public ICommand RefreshCommand { get; }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
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

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public DemonSlayerCharacter? SelectedCharacter
        {
            get => _selectedCharacter;
            set
            {
                if (SetProperty(ref _selectedCharacter, value) && value is not null)
                {
                    MainThread.BeginInvokeOnMainThread(async () => await ShowCharacterDetailAsync(value));
                }
            }
        }

        public async Task EnsureLoadedAsync()
        {
            if (!_needsRefresh)
            {
                return;
            }

            await LoadCharactersAsync();
        }

        public async Task LoadCharactersAsync()
        {
            if (IsLoading)
            {
                return;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;
                Characters.Clear();

                var characters = await _catalogService.GetCharactersAsync();
                foreach (var character in characters)
                {
                    Characters.Add(character);
                }

                _needsRefresh = false;

                if (Characters.Count == 0)
                {
                    ErrorMessage = "L'API n'a retourne aucun personnage.";
                }
            }
            catch
            {
                _needsRefresh = true;
                ErrorMessage = "Impossible de charger les personnages depuis l'API Demon Slayer.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task ShowCharacterDetailAsync(DemonSlayerCharacter character)
        {
            SelectedCharacter = null;
            await Shell.Current.GoToAsync(
                nameof(CharacterDetailPage),
                new Dictionary<string, object>
                {
                    [CharacterQueryKey] = character
                });
        }
    }
}
