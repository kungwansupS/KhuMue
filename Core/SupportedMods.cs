namespace KhuMue.Core;

/// <summary>A content mod the guide has checked data for.</summary>
/// <param name="Name">Internal mod name.</param>
/// <param name="VerifiedVersion">The exact version the guide data was checked against.</param>
/// <param name="SameMinor">True when every patch release of that minor version is expected to match.</param>
internal sealed record SupportedMod(string Name, string VerifiedVersion, bool SameMinor = false)
{
    public bool IsLoaded => ModLoader.HasMod(Name);

    public Version? LoadedVersion => ModLoader.TryGetMod(Name, out Mod mod) ? mod.Version : null;

    public Version Verified => Version.Parse(VerifiedVersion);

    /// <summary>Exact = checked version. Near = same major.minor. Different = anything else.</summary>
    public VersionMatch Match
    {
        get
        {
            Version? loaded = LoadedVersion;
            if (loaded is null)
                return VersionMatch.NotLoaded;
            if (loaded == Verified)
                return VersionMatch.Exact;
            if (loaded.Major == Verified.Major && loaded.Minor == Verified.Minor)
                return VersionMatch.Near;
            return VersionMatch.Different;
        }
    }
}

internal enum VersionMatch
{
    NotLoaded,
    Exact,
    Near,
    Different,
}

internal static class SupportedMods
{
    internal const string Calamity = "CalamityMod";
    internal const string Catalyst = "CatalystMod";
    internal const string Hunt = "CalamityHunt";
    internal const string WrathOfTheGods = "NoxusBoss";
    internal const string Infernum = "InfernumMode";
    internal const string Fables = "CalamityFables";
    internal const string SpiritReforged = "SpiritReforged";
    internal const string BossChecklist = "BossChecklist";

    internal static readonly SupportedMod[] All =
    {
        new(Calamity, "2.2.4"),
        new(Catalyst, "1.1.8"),
        new(Hunt, "1.2.3"),
        new(WrathOfTheGods, "1.2.31"),
        new(Infernum, "2.0.1.35"),
        new(Fables, "0.1.24"),
        new(SpiritReforged, "0.2.2.8"),
    };

    internal static bool CalamityLoaded => ModLoader.HasMod(Calamity);

    internal static SupportedMod? Find(string name) => All.FirstOrDefault(m => m.Name == name);

    /// <summary>Display name of a loaded mod without chat tags; falls back to the internal name.</summary>
    internal static string DisplayName(string name)
    {
        if (name == "Terraria")
            return "Terraria";
        if (!ModLoader.TryGetMod(name, out Mod mod))
            return name;
        string text = mod.DisplayNameClean;
        return string.IsNullOrWhiteSpace(text) ? name : text;
    }
}
