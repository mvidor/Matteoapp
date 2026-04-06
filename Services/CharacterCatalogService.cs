using MatteoAPP1.Models;

namespace MatteoAPP1.Services
{
    public sealed class CharacterCatalogService : ICharacterCatalogService
    {
        private readonly IDemonSlayerApiService _apiService;
        private readonly List<DemonSlayerCharacter> _customCharacters = [];

        public CharacterCatalogService(IDemonSlayerApiService apiService)
        {
            _apiService = apiService;
        }

        public event EventHandler? CharactersChanged;

        public async Task<IReadOnlyList<DemonSlayerCharacter>> GetCharactersAsync(CancellationToken cancellationToken = default)
        {
            var apiCharacters = await _apiService.GetCharactersAsync(12, cancellationToken);

            return _customCharacters
                .Concat(apiCharacters)
                .ToList();
        }

        public Task AddCharacterAsync(DemonSlayerCharacter character, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(character.Name))
            {
                throw new ArgumentException("Le personnage doit avoir un nom.", nameof(character));
            }

            _customCharacters.Insert(0, character);
            CharactersChanged?.Invoke(this, EventArgs.Empty);
            return Task.CompletedTask;
        }
    }
}
