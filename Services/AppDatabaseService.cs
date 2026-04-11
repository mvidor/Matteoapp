using MatteoAPP1.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Maui.Storage;

namespace MatteoAPP1.Services
{
    public sealed class AppDatabaseService : IAppDatabaseService
    {
        private readonly string _connectionString;
        private readonly SemaphoreSlim _gate = new(1, 1);
        private bool _isInitialized;

        public AppDatabaseService()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "matteoapp1.db");
            _connectionString = $"Data Source={databasePath}";
        }

        public async Task<IReadOnlyList<StoredCharacter>> GetStoredCharactersAsync(CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);
            await _gate.WaitAsync(cancellationToken);

            try
            {
                var characters = new List<StoredCharacter>();
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT Id, Name, Description, ImagePath
                    FROM Characters
                    ORDER BY rowid DESC;
                    """;

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    characters.Add(new StoredCharacter
                    {
                        Id = reader.GetString(0),
                        Name = reader.GetString(1),
                        Description = reader.GetString(2),
                        ImagePath = reader.GetString(3)
                    });
                }

                return characters;
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task SaveCharacterAsync(StoredCharacter character, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);
            await _gate.WaitAsync(cancellationToken);

            try
            {
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO Characters (Id, Name, Description, ImagePath)
                    VALUES ($id, $name, $description, $imagePath);
                    """;
                command.Parameters.AddWithValue("$id", character.Id);
                command.Parameters.AddWithValue("$name", character.Name);
                command.Parameters.AddWithValue("$description", character.Description);
                command.Parameters.AddWithValue("$imagePath", character.ImagePath);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task<IReadOnlyList<MovieComment>> GetMovieCommentsAsync(string movieKey, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);
            await _gate.WaitAsync(cancellationToken);

            try
            {
                var comments = new List<MovieComment>();
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT Id, MovieKey, Rating, Comment
                    FROM MovieComments
                    WHERE MovieKey = $movieKey
                    ORDER BY rowid DESC;
                    """;
                command.Parameters.AddWithValue("$movieKey", movieKey);

                await using var reader = await command.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    comments.Add(new MovieComment
                    {
                        Id = reader.GetString(0),
                        MovieKey = reader.GetString(1),
                        Rating = reader.GetInt32(2),
                        Comment = reader.GetString(3)
                    });
                }

                return comments;
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task SaveMovieCommentAsync(MovieComment movieComment, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);
            await _gate.WaitAsync(cancellationToken);

            try
            {
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO MovieComments (Id, MovieKey, Rating, Comment)
                    VALUES ($id, $movieKey, $rating, $comment);
                    """;
                command.Parameters.AddWithValue("$id", movieComment.Id);
                command.Parameters.AddWithValue("$movieKey", movieComment.MovieKey);
                command.Parameters.AddWithValue("$rating", movieComment.Rating);
                command.Parameters.AddWithValue("$comment", movieComment.Comment);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task DeleteMovieCommentAsync(string id, CancellationToken cancellationToken = default)
        {
            await EnsureInitializedAsync(cancellationToken);
            await _gate.WaitAsync(cancellationToken);

            try
            {
                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = """
                    DELETE FROM MovieComments
                    WHERE Id = $id;
                    """;
                command.Parameters.AddWithValue("$id", id);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            finally
            {
                _gate.Release();
            }
        }

        private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
        {
            if (_isInitialized)
            {
                return;
            }

            await _gate.WaitAsync(cancellationToken);

            try
            {
                if (_isInitialized)
                {
                    return;
                }

                await using var connection = new SqliteConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var command = connection.CreateCommand();
                command.CommandText = """
                    CREATE TABLE IF NOT EXISTS Characters (
                        Id TEXT NOT NULL PRIMARY KEY,
                        Name TEXT NOT NULL,
                        Description TEXT NOT NULL,
                        ImagePath TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS MovieComments (
                        Id TEXT NOT NULL PRIMARY KEY,
                        MovieKey TEXT NOT NULL,
                        Rating INTEGER NOT NULL,
                        Comment TEXT NOT NULL
                    );
                    """;

                await command.ExecuteNonQueryAsync(cancellationToken);
                _isInitialized = true;
            }
            finally
            {
                _gate.Release();
            }
        }
    }
}
