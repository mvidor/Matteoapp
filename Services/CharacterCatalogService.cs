using MatteoAPP1.Models;

namespace MatteoAPP1.Services
{
    public sealed class CharacterCatalogService : ICharacterCatalogService
    {
        private const int DefaultApiCharacterCount = 4;
        private readonly IDemonSlayerApiService _apiService;
        private readonly IAppDatabaseService _databaseService;

        public CharacterCatalogService(IDemonSlayerApiService apiService, IAppDatabaseService databaseService)
        {
            _apiService = apiService;
            _databaseService = databaseService;
        }

        public event EventHandler? CharactersChanged;

        public async Task<IReadOnlyList<DemonSlayerCharacter>> GetCharactersAsync(CancellationToken cancellationToken = default)
        {
            var apiCharacters = await _apiService.GetCharactersAsync(DefaultApiCharacterCount, cancellationToken);
            var customCharacters = await _databaseService.GetStoredCharactersAsync(cancellationToken);

            return customCharacters
                .Select(character => new DemonSlayerCharacter
                {
                    Id = character.Id,
                    Name = character.Name,
                    ImageUrl = character.ImagePath,
                    CustomDescription = character.Description,
                    Race = "Creation utilisateur",
                    Gender = "Inconnu",
                    Affiliation = "Ajout manuel"
                })
                .Concat(apiCharacters)
                .ToList();
        }

        public async Task AddCharacterAsync(DemonSlayerCharacter character, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(character.Name))
            {
                throw new ArgumentException("Le personnage doit avoir un nom.", nameof(character));
            }

            await _databaseService.SaveCharacterAsync(new StoredCharacter
            {
                Id = character.Id,
                Name = character.Name,
                Description = character.CustomDescription,
                ImagePath = character.ImageUrl
            }, cancellationToken);

            CharactersChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
