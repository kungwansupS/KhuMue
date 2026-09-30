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
        public string Key = "";
        public string Mod = "";
        public string Category = "";
        public string Class = "";
    }

    private static List<Entry>? _entries;

    internal static void Invalidate() => _entries = null;

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

    /// <summary>Matches all whitespace-separated words of the query (any order), best matches first.</summary>
    internal static List<int> Search(string query, string category, string cls, string mod)
    {
        string q = query.Trim().ToLowerInvariant();
        string[] words = q.Split(' ', StringSplitOptions.RemoveEmptyEntries);
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
            if (words.Length > 0)
            {
                bool all = true;
                foreach (string w in words)
                {
                    if (!e.Lower.Contains(w, StringComparison.Ordinal))
                    {
                        all = false;
                        break;
                    }
                }
                if (!all)
                    continue;
                string lowerName = e.Name.ToLowerInvariant();
                if (lowerName == q)
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
}
