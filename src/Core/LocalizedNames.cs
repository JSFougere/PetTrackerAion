using System.Globalization;
using System.Text.Json;

namespace PetTracker;

public static class LocalizedNames
{
    public sealed record NamePair(string En, string Fr);
    sealed record NameData(string Source, Dictionary<int, NamePair> Pets, Dictionary<int, NamePair> Npcs);
    static readonly NameData data = Load();
    static readonly Dictionary<string, NamePair> monsters = data.Npcs.Values
        .GroupBy(n => n.En, StringComparer.OrdinalIgnoreCase)
        .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    static NameData Load()
    {
        using var stream = typeof(LocalizedNames).Assembly.GetManifestResourceStream("PetTracker.LocalizedNames.json")!;
        return JsonSerializer.Deserialize<NameData>(stream)!;
    }
    public static string Pet(int id, string fallback, string language) => language == "fr" && data.Pets.TryGetValue(id, out var name) ? name.Fr : fallback;
    public static string Monster(string name, string language)
    {
        if (language != "fr") return name;
        if (monsters.TryGetValue(name, out var pair)) return pair.Fr;
        return string.Join(", ", name.Split(", ", StringSplitOptions.None).Select(part => monsters.TryGetValue(part, out var entry) ? entry.Fr : part));
    }
    public static string Monster(int id, string fallback, string language) => language == "fr" && data.Npcs.TryGetValue(id, out var pair) ? pair.Fr : Monster(fallback, language);
    public static bool Contains(string text, string query) => CultureInfo.InvariantCulture.CompareInfo.IndexOf(text, query,
        CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreWidth) >= 0;

    // Search never depends on the display language. Keep legacy English names too.
    public static bool Matches(PetSearchResult pet, string query)
    {
        var terms = new List<string> { pet.Label, PetFamily.Label(pet.Pet.Id) };
        if (data.Pets.TryGetValue(pet.Pet.Id, out var pair)) { terms.Add(pair.En); terms.Add(pair.Fr); }
        foreach (string creature in pet.Creatures)
        {
            terms.Add(creature);
            if (monsters.TryGetValue(creature, out var name)) terms.Add(name.Fr);
        }
        foreach (var mob in pet.Location?.SoulSources ?? Enumerable.Empty<FarmingMob>())
        {
            terms.Add(mob.Name); terms.Add(mob.Region);
            if (data.Npcs.TryGetValue(mob.NpcId, out var name)) { terms.Add(name.En); terms.Add(name.Fr); }
        }
        terms.AddRange(pet.Location?.Maps.Select(m => m.Region) ?? Enumerable.Empty<string>());
        terms.AddRange(pet.Acquisition?.Aliases ?? Array.Empty<string>());
        return query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).All(word => terms.Any(term => Contains(term, word)));
    }
}
