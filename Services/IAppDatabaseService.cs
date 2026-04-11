using MatteoAPP1.Models;

namespace MatteoAPP1.Services
{
    public interface IAppDatabaseService
    {
        Task<IReadOnlyList<StoredCharacter>> GetStoredCharactersAsync(CancellationToken cancellationToken = default);

        Task SaveCharacterAsync(StoredCharacter character, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<MovieComment>> GetMovieCommentsAsync(string movieKey, CancellationToken cancellationToken = default);

        Task SaveMovieCommentAsync(MovieComment movieComment, CancellationToken cancellationToken = default);

        Task DeleteMovieCommentAsync(string id, CancellationToken cancellationToken = default);
    }
}
