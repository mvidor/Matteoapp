using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MatteoAPP1.Models;
using MatteoAPP1.Services;

namespace MatteoAPP1
{
    public partial class CharactersPage : ContentPage, INotifyPropertyChanged
    {
        private readonly IDemonSlayerApiService _apiService;
        private bool _hasLoaded;
        private bool _isLoading;
        private string _errorMessage = string.Empty;

        public CharactersPage()
        {
            InitializeComponent();
            _apiService = ServiceHelper.GetService<IDemonSlayerApiService>();
            BindingContext = this;
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<DemonSlayerCharacter> Characters { get; } = [];

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading == value)
                {
                    return;
                }

                _isLoading = value;
                RaisePropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage == value)
                {
                    return;
                }

                _errorMessage = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (_hasLoaded)
            {
                return;
            }

            _hasLoaded = true;
            await LoadCharactersAsync();
        }

        private async void OnRefreshClicked(object? sender, EventArgs e)
        {
            await LoadCharactersAsync();
        }

        private async Task LoadCharactersAsync()
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

                var characters = await _apiService.GetCharactersAsync(12);
                foreach (var character in characters)
                {
                    Characters.Add(character);
                }

                if (Characters.Count == 0)
                {
                    ErrorMessage = "L'API n'a retourne aucun personnage.";
                }
            }
            catch
            {
                ErrorMessage = "Impossible de charger les personnages depuis l'API Demon Slayer.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
