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

        public async Task<DemonSlayerCharacter?> GetCharacterByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var encodedName = Uri.EscapeDataString(name.Trim());
            using var response = await _httpClient.GetAsync($"characters?name={encodedName}", cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var characters = ReadCharacters(document.RootElement);

            return characters
                .FirstOrDefault(character => string.Equals(character.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
                ?? characters.FirstOrDefault();
        }

        private static IReadOnlyList<DemonSlayerCharacter> ReadCharacters(JsonElement root)
        {
            var characters = new List<DemonSlayerCharacter>();

            if (root.ValueKind == JsonValueKind.Object && LooksLikeCharacter(root))
            {
                AddCharacter(root, characters);
                return characters;
            }

            var source = root.ValueKind switch
            {
                JsonValueKind.Array => root,
                JsonValueKind.Object => FindArray(root),
                _ => default
            };

            if (source.ValueKind != JsonValueKind.Array)
            {
                return characters;
            }

            foreach (var item in source.EnumerateArray())
            {
                AddCharacter(item, characters);
            }

            return characters;
        }

        private static void AddCharacter(JsonElement item, List<DemonSlayerCharacter> characters)
        {
            if (item.ValueKind != JsonValueKind.Object)
            {
                return;
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

        private static bool LooksLikeCharacter(JsonElement item)
        {
            return item.TryGetProperty("name", out _)
                || item.TryGetProperty("characterName", out _)
                || item.TryGetProperty("image", out _)
                || item.TryGetProperty("img", out _)
                || item.TryGetProperty("photo", out _);
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
