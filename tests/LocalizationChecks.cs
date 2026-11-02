namespace PetTracker;

public static class LocalizationChecks
{
    public static void Run(Action<bool, string> check)
    {
        check(LocalizedNames.Contains("Étoilée", "etoilee"), "Search ignores accents and case");
        check(LocalizedNames.Contains("E\u0301toile\u0301e", "étoilée"), "Search handles decomposed Unicode accents");
        check(PetFinder.Search("tortue etoilee").Select(p => p.Pet.Id).SequenceEqual(PetFinder.Search("Starturtle").Select(p => p.Pet.Id)), "French and English pet names resolve to the same IDs");
        check(PetFinder.Search("tortue etoilee").Any(p => p.Pet.Id == 1060), "French pet names are searchable without accents");
        check(PetFinder.Search("soldat arme krall").Any(p => p.Pet.Id == 1002), "French monster names resolve through existing NPC IDs");
        check(LocalizedNames.Pet(1060, "Starturtle", "fr") == "Tortue étoilée", "French display uses client names");
        check(LocalizedNames.Pet(1060, "Starturtle", "en") == "Starturtle", "English display preserves legacy names");
        check(LocalizedNames.Pet(-1, "Unknown", "fr") == "Unknown", "Missing translations preserve the original name");
        check(LocalizedNames.Monster("Krall Armed Soldier, Missing NPC", "fr") == "Soldat armé Krall, Missing NPC", "Nearby lists localize each monster and preserve unknown names");
        var settings = new AppSettings { NameLanguage = "invalid", OverlayWidth = double.NaN, OverlayHeight = double.PositiveInfinity };
        settings.Normalize();
        check(settings.NameLanguage == "en" && settings.OverlayWidth == null && settings.OverlayHeight == null, "Invalid locale and dimensions normalize safely");
        settings = new() { NameLanguage = "fr", OverlayWidth = 2, OverlayHeight = 99999 };
        settings.Normalize();
        check(settings.NameLanguage == "fr" && settings.OverlayWidth == 260 && settings.OverlayHeight == 1600, "Resize dimensions respect supported bounds");
        var roundtrip = System.Text.Json.JsonSerializer.Deserialize<AppSettings>(System.Text.Json.JsonSerializer.Serialize(settings))!;
        check(roundtrip.NameLanguage == "fr" && roundtrip.OverlayWidth == 260 && roundtrip.OverlayHeight == 1600, "Language and size survive JSON persistence");
    }
}
