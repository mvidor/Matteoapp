using MatteoAPP1.Models;

namespace MatteoAPP1.Services
{
    public interface ICharacterCatalogService
    {
        event EventHandler? CharactersChanged;

        Task<IReadOnlyList<DemonSlayerCharacter>> GetCharactersAsync(CancellationToken cancellationToken = default);

        Task AddCharacterAsync(DemonSlayerCharacter character, CancellationToken cancellationToken = default);
    }
}
