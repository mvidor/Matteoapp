using System.Text.Json;
using MatteoAPP1.Models;

namespace MatteoAPP1.Services
{
    public sealed class DemonSlayerApiService : IDemonSlayerApiService
    {
        private readonly HttpClient _httpClient;

        public DemonSlayerApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<DemonSlayerCharacter>> GetCharactersAsync(int limit = 10, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync($"characters?limit={limit}", cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            return ReadCharacters(document.RootElement);
        }

        private static IReadOnlyList<DemonSlayerCharacter> ReadCharacters(JsonElement root)
        {
            var source = root.ValueKind switch
            {
                JsonValueKind.Array => root,
                JsonValueKind.Object => FindArray(root),
                _ => default
            };

            if (source.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<DemonSlayerCharacter>();
            }

            var characters = new List<DemonSlayerCharacter>();

            foreach (var item in source.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    continue;
                }

                characters.Add(new DemonSlayerCharacter
                {
                    Id = ReadString(item, "id", "_id"),
                    Name = ReadString(item, "name", "characterName"),
                    Race = ReadString(item, "race", "species"),
                    Gender = ReadString(item, "gender", "sex"),
                    Affiliation = ReadComposite(item, "affiliation", "group", "organization"),
                    ImageUrl = ReadString(item, "image", "img", "photo")
                });
            }

            return characters;
        }

        private static JsonElement FindArray(JsonElement root)
        {
            foreach (var propertyName in new[] { "content", "data", "items", "results", "characters" })
            {
                if (root.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Array)
                {
                    return value;
                }
            }

            return default;
        }

        private static string ReadComposite(JsonElement item, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (!item.TryGetProperty(propertyName, out var value))
                {
                    continue;
                }

                if (value.ValueKind == JsonValueKind.Array)
                {
                    var values = value.EnumerateArray()
                        .Select(x => x.ValueKind == JsonValueKind.String ? x.GetString() : x.ToString())
                        .Where(x => !string.IsNullOrWhiteSpace(x));

                    var joined = string.Join(", ", values!);
                    if (!string.IsNullOrWhiteSpace(joined))
                    {
                        return joined;
                    }
                }

                var text = value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text!;
                }
            }

            return "Non precisee";
        }

        private static string ReadString(JsonElement item, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (!item.TryGetProperty(propertyName, out var value))
                {
                    continue;
                }

                var text = value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text!;
                }
            }

            return string.Empty;
        }
    }
}
