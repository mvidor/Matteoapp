using MatteoAPP1.Models;

namespace MatteoAPP1.Services
{
    public interface IDemonSlayerApiService
    {
        Task<IReadOnlyList<DemonSlayerCharacter>> GetCharactersAsync(int limit = 10, CancellationToken cancellationToken = default);

        Task<DemonSlayerCharacter?> GetCharacterByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
