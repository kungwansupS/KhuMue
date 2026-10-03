using System.Text;
using System.Text.RegularExpressions;
using KhuMue.Core;

namespace KhuMue.UI.Pages;

/// <summary>Search index over every loaded item, rebuilt when the language changes.</summary>
internal static class ItemSearch
{
    internal sealed class Entry
    {
        public int Type;
        public string Name = "";
        public string Lower = "";
        /// <summary>Name and internal name without the mod prefix, matched against the language's search words.</summary>
        public string Words = "";
        public string Key = "";
        public string Mod = "";
        public string Category = "";
        public string Class = "";

        private string? _tooltip;

        /// <summary>Lower-case tooltip text, read the first time a search needs it.</summary>
        public string Tooltip => _tooltip ??= TooltipText(Type);
    }

    private static List<Entry>? _entries;
    private static Dictionary<string, string[]>? _searchWords;
    private static int _longestWord;

    internal static void Invalidate()
    {
        _entries = null;
        _searchWords = null;
    }

    internal static IReadOnlyList<Entry> All
    {
        get
        {
            if (_entries is null)
                Build();
            return _entries!;
        }
    }

    private static void Build()
    {
        var list = new List<Entry>(ItemLoader.ItemCount);
        for (int type = 1; type < ItemLoader.ItemCount; type++)
        {
            if (!ContentSamples.ItemsByType.TryGetValue(type, out Item? item))
                continue;
            if (ItemID.Sets.Deprecated[type])
                continue;
            string name = GuideCatalog.ItemName(type);
            if (string.IsNullOrWhiteSpace(name))
                continue;
            string key = GuideCatalog.ItemKey(type);
            list.Add(new Entry
            {
                Type = type,
                Name = name,
                Lower = (name + " " + key).ToLowerInvariant(),
                Words = (name + " " + key[(key.IndexOf('/') + 1)..]).ToLowerInvariant(),
                Key = key,
                Mod = GuideCatalog.ItemModName(type),
                Category = CategoryOf(item),
                Class = ClassOf(item),
            });
        }
        _entries = list;
    }

    /// <summary>weapon, tool, armor, accessory, ammo, potion, placeable, material, other.</summary>
    internal static string CategoryOf(Item item)
    {
        if (item.pick > 0 || item.axe > 0 || item.hammer > 0 || item.fishingPole > 0)
            return "tool";
        if (item.accessory && !item.vanity)
            return "accessory";
        if ((item.headSlot >= 0 || item.bodySlot >= 0 || item.legSlot >= 0) && !item.vanity)
            return "armor";
        if (item.ammo > 0 && item.damage > 0)
            return "ammo";
        if (item.damage > 0 && item.useStyle != ItemUseStyleID.None && item.createTile < 0)
            return "weapon";
        // Pets and mounts also carry a buff, but one without a duration.
        if (item.buffType > 0 && item.buffTime > 0 || item.healLife > 0 || item.healMana > 0)
            return "potion";
        if (item.createTile >= 0 || item.createWall >= 0)
            return "placeable";
        // Reusable gadgets (Rod of Discord, Magic Mirror, hooks, wrenches ...), before "material": being a recipe
        // ingredient says little about what an item is for. Pets, mounts and boss summons stay "other".
        if (item.useStyle != ItemUseStyleID.None && !item.consumable && item.buffType <= 0 && item.mountType < 0
            && ItemID.Sets.SortingPriorityBossSpawns[item.type] < 0)
            return "tool";
        if (item.material)
            return "material";
        return "other";
    }

    /// <summary>melee, ranged, magic, summon, rogue or "" for classless items.</summary>
    internal static string ClassOf(Item item)
    {
        if (item is null || item.IsAir || item.damage <= 0)
            return "";
        DamageClass? type = item.DamageType;
        if (type is null)
            return "";
        string full = type.FullName;
        if (full.Contains("Rogue", StringComparison.OrdinalIgnoreCase) || full.Contains("Throwing", StringComparison.OrdinalIgnoreCase))
            return SupportedMods.CalamityLoaded ? "rogue" : "";
        if (type.CountsAsClass(DamageClass.Summon) || type.CountsAsClass(DamageClass.SummonMeleeSpeed))
            return "summon";
        if (type.CountsAsClass(DamageClass.Melee))
            return "melee";
        if (type.CountsAsClass(DamageClass.Ranged))
            return "ranged";
        if (type.CountsAsClass(DamageClass.Magic))
            return "magic";
        return "";
    }

    /// <summary>
    /// Matches all words of the query (any order), best matches first. A word matches the item name, the internal
    /// name, the language's search words (Thai "ดาบ" finds Swords and Blades, since item names stay in English), or
    /// else the tooltip text; items found only through their tooltip come last.
    /// </summary>
    internal static List<int> Search(string query, string category, string cls, string mod)
    {
        string q = query.Trim().ToLowerInvariant();
        List<string> words = q.Split(' ', StringSplitOptions.RemoveEmptyEntries).SelectMany(Split).ToList();
        Dictionary<string, string[]> searchWords = SearchWords;
        var results = new List<(int Score, int Type)>();
        foreach (Entry e in All)
        {
            if (category.Length > 0 && e.Category != category)
                continue;
            if (cls.Length > 0 && e.Class != cls)
                continue;
            if (mod.Length > 0 && e.Mod != mod)
                continue;
            int score = 0;
            if (words.Count > 0)
            {
                bool all = true;
                bool tooltipOnly = false;
                foreach (string w in words)
                {
                    if (e.Lower.Contains(w, StringComparison.Ordinal))
                        continue;
                    if (searchWords.TryGetValue(w, out string[]? tokens) && tokens.Any(t => HasToken(e.Words, t)))
                        continue;
                    if (e.Tooltip.Contains(w, StringComparison.Ordinal))
                    {
                        tooltipOnly = true;
                        continue;
                    }
                    all = false;
                    break;
                }
                if (!all)
                    continue;
                string lowerName = e.Name.ToLowerInvariant();
                if (tooltipOnly)
                    score = 4;
                else if (lowerName == q)
                    score = 0;
                else if (lowerName.StartsWith(q, StringComparison.Ordinal))
                    score = 1;
                else if (lowerName.Contains(q, StringComparison.Ordinal))
                    score = 2;
                else
                    score = 3;
            }
            results.Add((score, e.Type));
        }
        results.Sort((a, b) => a.Score != b.Score ? a.Score.CompareTo(b.Score) : a.Type.CompareTo(b.Type));
        return results.Select(r => r.Type).ToList();
    }

    /// <summary>
    /// Search words of the current language from <c>Items.SearchWords</c> ("word=token token;word=..."), empty in
    /// English.
    /// </summary>
    private static Dictionary<string, string[]> SearchWords
    {
        get
        {
            if (_searchWords is not null)
                return _searchWords;
            var words = new Dictionary<string, string[]>();
            _longestWord = 0;
            foreach (string pair in PageKit.L("Items.SearchWords").Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                int eq = pair.IndexOf('=');
                if (eq <= 0)
                    continue;
                string word = pair[..eq].Trim().ToLowerInvariant();
                string[] tokens = pair[(eq + 1)..].ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (word.Length == 0 || tokens.Length == 0)
                    continue;
                words[word] = tokens;
                _longestWord = Math.Max(_longestWord, word.Length);
            }
            return _searchWords = words;
        }
    }

    /// <summary>
    /// Thai is typed without spaces: a word that is not a search word itself but is made of search words
    /// ("ดาบไฟ") is split into them (longest first); anything else stays one word.
    /// </summary>
    private static IEnumerable<string> Split(string word)
    {
        Dictionary<string, string[]> words = SearchWords;
        if (words.Count == 0 || words.ContainsKey(word))
            return new[] { word };
        var parts = new List<string>();
        int i = 0;
        while (i < word.Length)
        {
            int length = Math.Min(_longestWord, word.Length - i);
            while (length > 0 && !words.ContainsKey(word.Substring(i, length)))
                length--;
            if (length == 0)
                return new[] { word };
            parts.Add(word.Substring(i, length));
            i += length;
        }
        return parts;
    }

    /// <summary>Long tokens match anywhere ("sword" in "greatsword"); short ones only at a word start ("ice", not "slice").</summary>
    private static bool HasToken(string text, string token)
    {
        if (token.Length >= 5)
            return text.Contains(token, StringComparison.Ordinal);
        for (int at = text.IndexOf(token, StringComparison.Ordinal); at >= 0; at = text.IndexOf(token, at + 1, StringComparison.Ordinal))
        {
            if (at == 0 || !char.IsLetter(text[at - 1]))
                return true;
        }
        return false;
    }

    private static readonly Regex ChatTags = new(@"\[c(?:olor)?/[^:\]]*:([^\]]*)\]|\[[a-z]+(?:/[^:\]]*)?:[^\]]*\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static string TooltipText(int type)
    {
        try
        {
            var tip = Lang.GetTooltip(type);
            var text = new StringBuilder();
            for (int i = 0; i < tip.Lines; i++)
                text.Append(tip.GetLine(i)).Append(' ');
            return ChatTags.Replace(text.ToString(), "$1").ToLowerInvariant();
        }
        catch
        {
            return "";
        }
    }
}
