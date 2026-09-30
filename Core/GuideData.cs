using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KhuMue.Core;

/// <summary>
/// Curated, version-checked guide data shipped in <c>Data/*.json</c>.
/// Everything the game can report by itself (recipes, drops, shops, stats, names) is read live
/// instead; see <see cref="GuideCatalog"/>.
/// </summary>
internal static class GuideData
{
    internal static IReadOnlyList<ProgressionEntry> Entries { get; private set; } = Array.Empty<ProgressionEntry>();
    internal static IReadOnlyDictionary<string, ProgressionEntry> EntriesById { get; private set; } = new Dictionary<string, ProgressionEntry>();
    internal static StageLadders Stages { get; private set; } = new();
    internal static LoadoutSet? CalamityLoadouts { get; private set; }
    internal static LoadoutSet? VanillaLoadouts { get; private set; }
    internal static IReadOnlyList<UpgradeDefinition> Upgrades { get; private set; } = Array.Empty<UpgradeDefinition>();
    internal static IReadOnlyList<GuideArticle> Articles { get; private set; } = Array.Empty<GuideArticle>();

    /// <summary>Fishing catches and Angler rewards by item key (tools/data/obtain_sources.py, from the game code).</summary>
    internal static IReadOnlyDictionary<string, List<ObtainInfo>> Obtain { get; private set; } = new Dictionary<string, List<ObtainInfo>>();
    internal static IReadOnlyList<string> LoadErrors => _errors;

    private static readonly List<string> _errors = new();

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    internal static void Load(Mod mod)
    {
        _errors.Clear();
        GuideCondition.ClearCache();

        var entries = Read<ProgressionFile>(mod, "Data/progression.json")?.Entries ?? new List<ProgressionEntry>();
        entries.Sort((a, b) => a.Order.CompareTo(b.Order));
        Entries = entries;
        EntriesById = entries.ToDictionary(e => e.Id);

        Stages = Read<StageLadders>(mod, "Data/stages.json") ?? new StageLadders();
        CalamityLoadouts = Read<LoadoutSet>(mod, "Data/loadouts-calamity.json");
        VanillaLoadouts = Read<LoadoutSet>(mod, "Data/loadouts-vanilla.json");
        Upgrades = Read<List<UpgradeDefinition>>(mod, "Data/upgrades.json") ?? new List<UpgradeDefinition>();
        Articles = Read<List<GuideArticle>>(mod, "Data/guides.json") ?? new List<GuideArticle>();
        Obtain = Read<Dictionary<string, List<ObtainInfo>>>(mod, "Data/obtain.json") ?? new Dictionary<string, List<ObtainInfo>>();

        foreach (string error in _errors)
            mod.Logger.Error(error);
    }

    internal static void Unload()
    {
        Entries = Array.Empty<ProgressionEntry>();
        EntriesById = new Dictionary<string, ProgressionEntry>();
        Stages = new StageLadders();
        CalamityLoadouts = null;
        VanillaLoadouts = null;
        Upgrades = Array.Empty<UpgradeDefinition>();
        Articles = Array.Empty<GuideArticle>();
        Obtain = new Dictionary<string, List<ObtainInfo>>();
        _errors.Clear();
        GuideCondition.ClearCache();
    }

    private static T? Read<T>(Mod mod, string path) where T : class
    {
        try
        {
            if (!mod.FileExists(path))
            {
                _errors.Add($"Guide data file missing: {path}");
                return null;
            }
            using Stream stream = mod.GetFileStream(path);
            return JsonSerializer.Deserialize<T>(stream, Options);
        }
        catch (Exception e)
        {
            _errors.Add($"Guide data file {path} could not be read: {e.Message}");
            return null;
        }
    }
}

// ---------------------------------------------------------------- progression

internal sealed class ProgressionFile
{
    public List<ProgressionEntry> Entries { get; set; } = new();
}

/// <summary>One boss, miniboss, event or quest step.</summary>
internal sealed class ProgressionEntry
{
    public string Id { get; set; } = "";

    /// <summary>Internal name of the content mod; "Terraria" for vanilla.</summary>
    public string Mod { get; set; } = "Terraria";

    /// <summary>boss, miniboss, event or quest.</summary>
    public string Kind { get; set; } = "boss";

    /// <summary>pre, hm, post or end.</summary>
    public string Era { get; set; } = "pre";

    /// <summary>Sort order; matches Boss Checklist progression values where they exist.</summary>
    public float Order { get; set; }

    /// <summary>main = on the recommended route; optional = side content.</summary>
    public string Path { get; set; } = "optional";

    /// <summary>True when later progression is impossible without it.</summary>
    public bool Gate { get; set; }

    /// <summary>Events that can be repeated and have no "completed" state (Blood Moon, Eclipse...).</summary>
    public bool Repeatable { get; set; }

    /// <summary>NPC keys ("Mod/Name"); the first one is used for the portrait and name.</summary>
    public List<string> Npcs { get; set; } = new();

    public List<string> Summons { get; set; } = new();

    /// <summary>Condition: completed in this world.</summary>
    public string Done { get; set; } = "never";

    /// <summary>Condition: can be started/fought now.</summary>
    public string? Available { get; set; }

    /// <summary>Condition: the entry applies to this world at all (e.g. only with Infernum on).</summary>
    public string? Show { get; set; }

    /// <summary>Event id this miniboss/step belongs to.</summary>
    public string? Parent { get; set; }

    /// <summary>Entries hidden while this one is shown (e.g. a reworked boss replacing the original).</summary>
    public List<string> Replaces { get; set; } = new();

    /// <summary>Item key drawn instead of the NPC when the NPC has no usable picture (custom-drawn bosses).</summary>
    public string? Icon { get; set; }

    [JsonIgnore]
    public bool IsMain => Path == "main";

    [JsonIgnore]
    public string LocalizationPrefix => "Mods.KhuMue.Guide.Entries." + Id;
}

// ---------------------------------------------------------------- stages

internal sealed class StageLadders
{
    public List<StageDefinition> Calamity { get; set; } = new();
    public List<StageDefinition> Vanilla { get; set; } = new();
}

internal sealed class StageDefinition
{
    public string Id { get; set; } = "";
    public string Era { get; set; } = "pre";

    /// <summary>Condition that is true once the world has reached this stage.</summary>
    public string When { get; set; } = "always";

    /// <summary>Progression entries this stage prepares for ("gear for ...").</summary>
    public List<string> Targets { get; set; } = new();
}

// ---------------------------------------------------------------- loadouts

internal sealed class LoadoutSet
{
    /// <summary>Where the recommendations come from (shown to players).</summary>
    public string Source { get; set; } = "";
    public string SourceUrl { get; set; } = "";
    public string SourceVersion { get; set; } = "";
    public string Retrieved { get; set; } = "";
    public List<string> Stages { get; set; } = new();
    public List<LoadoutRow> Rows { get; set; } = new();
}

internal sealed class LoadoutRow
{
    /// <summary>melee, ranged, magic, summon, rogue, all, all-but-summoner, ...</summary>
    public string Class { get; set; } = "";
    public string Stage { get; set; } = "";

    /// <summary>weapon, armor, accessoryOffense, buffOffense, ammo, minions, ...</summary>
    public string Type { get; set; } = "";

    /// <summary>Alternatives; each alternative is one item or one armor set (several keys).</summary>
    public List<List<string>> Items { get; set; } = new();

    /// <summary>Names of multi-piece alternatives ("Statigel Armor"), aligned with <see cref="Items"/>; null for single items.</summary>
    public List<string?>? Labels { get; set; }

    /// <summary>Wiki symbols: † C + ≤ Δ * Ω.</summary>
    public string Flags { get; set; } = "";

    /// <summary>Only available in Revengeance or higher.</summary>
    public bool Revengeance { get; set; }

    /// <summary>Localization id of the note (Mods.KhuMue.Guide.LoadoutNotes.*), if any.</summary>
    public string? Note { get; set; }
}

// ---------------------------------------------------------------- upgrades

internal sealed class UpgradeDefinition
{
    public string Id { get; set; } = "";
    public string Item { get; set; } = "";

    /// <summary>life, mana, slot, stats, qol, rage, adrenaline, souls, world.</summary>
    public string Group { get; set; } = "stats";
    public string Mod { get; set; } = "Terraria";

    /// <summary>How many can be used (1 for one-time upgrades).</summary>
    public int Max { get; set; } = 1;

    /// <summary>Condition: obtainable at this point of progression (highlight only).</summary>
    public string? Available { get; set; }

    /// <summary>Condition: obtainable in this world at all (difficulty); false = listed separately, not counted.</summary>
    public string? Requires { get; set; }

    /// <summary>Condition: the upgrade exists with the loaded mods (hidden otherwise).</summary>
    public string? Show { get; set; }
}

// ---------------------------------------------------------------- articles

internal sealed class GuideArticle
{
    public string Id { get; set; } = "";
    public string Category { get; set; } = "basics";

    /// <summary>Item key used as the article icon.</summary>
    public string Icon { get; set; } = "";

    /// <summary>Only show when these mods are loaded.</summary>
    public List<string> Mods { get; set; } = new();

    /// <summary>Section ids; text lives in Mods.KhuMue.Guide.Guides.{Id}.{Section}.</summary>
    public List<string> Sections { get; set; } = new();

    [JsonIgnore]
    public string LocalizationPrefix => "Mods.KhuMue.Guide.Guides." + Id;
}

/// <summary>A way to get an item that the game has no registry for (Data/obtain.json).</summary>
internal sealed class ObtainInfo
{
    /// <summary>"fishing", "angler" or "chest".</summary>
    public string How { get; set; } = "";

    /// <summary>For chests: the chest kind (text in Mods.KhuMue.Guide.Items.Chest.{Chest}).</summary>
    public string? Chest { get; set; }

    /// <summary>The entry only applies while this mod is loaded (e.g. vanilla items in Calamity structures).</summary>
    public string? With { get; set; }

    /// <summary>The entry only applies while this mod is not loaded (e.g. loot a mod removes).</summary>
    public string? Without { get; set; }

    /// <summary>For fishing: the condition sets (biome, layer, liquid, "after:Boss" ...) of the branches that yield it.</summary>
    public List<List<string>> Where { get; set; } = new();
}
