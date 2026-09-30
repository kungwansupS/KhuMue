using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using System.Reflection;
using Terraria.Map;

namespace KhuMue.Core;

internal enum SourceKind
{
    Recipe,
    NpcDrop,
    GlobalDrop,
    Container,
    Shop,
    Shimmer,
    Catch,
    Banner,
    Fishing,
    Angler,
    Chest,
}

/// <summary>One way to obtain an item, read from the live game registries.</summary>
internal sealed class ItemSource
{
    public SourceKind Kind { get; init; }

    /// <summary>NPC that drops/sells/is caught (NpcDrop, Shop, Catch, Banner).</summary>
    public int Npc { get; init; }

    /// <summary>Container item (bag/crate) or shimmer input item.</summary>
    public int FromItem { get; init; }

    /// <summary>Chance per kill/opening, 0..1.</summary>
    public float Chance { get; init; } = 1f;

    public int Min { get; init; } = 1;
    public int Max { get; init; } = 1;

    /// <summary>Chest: the chest kind (see <see cref="ObtainInfo.Chest"/>).</summary>
    public string? Chest { get; init; }

    /// <summary>Fishing: condition sets of the catch (see <see cref="ObtainInfo.Where"/>).</summary>
    public IReadOnlyList<IReadOnlyList<string>> Where { get; init; } = Array.Empty<IReadOnlyList<string>>();

    /// <summary>Localized condition texts reported by the game (may be empty).</summary>
    public IReadOnlyList<string> Conditions { get; init; } = Array.Empty<string>();

    /// <summary>Shop price in copper coins (or special currency amount), -1 if unknown.</summary>
    public int Price { get; init; } = -1;

    /// <summary>Shop special currency id, -1 for coins.</summary>
    public int Currency { get; init; } = -1;

    public Recipe? Recipe { get; init; }
}

/// <summary>
/// Live index of items, NPCs, recipes and item sources for the loaded mod pack.
/// Built once (lazily, on the main thread) after all content has loaded.
/// </summary>
internal static class GuideCatalog
{
    private static bool _built;
    private static readonly Dictionary<string, int> ItemTypeCache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, int> NpcTypeCache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<int, List<ItemSource>> SourcesByItem = new();
    private static readonly Dictionary<int, List<Recipe>> RecipesByResult = new();
    private static readonly Dictionary<int, List<Recipe>> RecipesByIngredient = new();
    private static readonly Dictionary<int, List<(int Item, float Chance)>> DropsByNpc = new();
    private static readonly Dictionary<int, List<int>> BagsByNpc = new();
    private static readonly Dictionary<int, List<string>> NpcBiomes = new();
    private static readonly Dictionary<int, string> TileNames = new();

    internal static bool IsBuilt => _built;

    internal static void Unload()
    {
        _built = false;
        ItemTypeCache.Clear();
        NpcTypeCache.Clear();
        SourcesByItem.Clear();
        RecipesByResult.Clear();
        RecipesByIngredient.Clear();
        DropsByNpc.Clear();
        BagsByNpc.Clear();
        NpcBiomes.Clear();
        TileNames.Clear();
        _moddedMusicBoxes = null;
    }

    // ------------------------------------------------------------------ hints for items without a source

    /// <summary>Items of vanilla's developer sets (<c>Player.TryGettingDevArmor</c>, Terraria 1.4.4.9).</summary>
    private static readonly HashSet<int> VanillaDevItems = new()
    {
        665, 666, 667, 668, 1554, 1555, 1556, 1557, 1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565, 1566, 1567, 1568,
        1580, 1581, 1582, 1583, 1584, 1585, 1586, 1587, 1588, 3024, 3226, 3227, 3228, 3287, 3288, 3368, 3578, 3579,
        3580, 3581, 3582, 3583, 3585, 3586, 3587, 3588, 3589, 3590, 3591, 3592, 3599, 3921, 3922, 3923, 3924, 3925,
        3926, 3927, 3928, 3929, 4730, 4732, 4733, 4734, 4746, 4747, 4748, 4749, 4750, 4751, 4752, 4753, 4754, 4755,
        4756, 4757
    };

    private static HashSet<int>? _moddedMusicBoxes;

    /// <summary>Modded music box items (tModLoader keeps them in MusicLoader.itemToMusic, which is internal).</summary>
    private static HashSet<int> ModdedMusicBoxes()
    {
        if (_moddedMusicBoxes is null)
        {
            _moddedMusicBoxes = new HashSet<int>();
            try
            {
                if (typeof(MusicLoader).GetField("itemToMusic", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) is Dictionary<int, int> map)
                    _moddedMusicBoxes.UnionWith(map.Keys);
            }
            catch
            {
                // Without the list only vanilla music boxes get the hint.
            }
        }
        return _moddedMusicBoxes;
    }

    /// <summary>
    /// Text key of a general "where to find it" hint for an item the game reports no source for: developer sets
    /// (Hardmode Treasure Bags), Angler quest fish, music boxes (recorded), natural blocks and placed furniture.
    /// </summary>
    internal static string? SourceHint(int type)
    {
        Item item = SampleItem(type);
        if (item.IsAir)
            return null;
        if (VanillaDevItems.Contains(type) || ItemKey(type).StartsWith("ModLoader/", StringComparison.Ordinal))
            return "Items.Hint.DevSet";
        if (item.questItem)
            return "Items.Hint.AnglerFish";
        if (type != ItemID.MusicBox && (item.createTile == TileID.MusicBoxes || ModdedMusicBoxes().Contains(type)))
            return "Items.Hint.MusicBox";
        if (item.createTile >= 0 && item.createTile < TileLoader.TileCount)
        {
            if (Main.tileFrameImportant[item.createTile])
                return "Items.Hint.Furniture";
            if (Main.tileSolid[item.createTile])
                return "Items.Hint.Block";
        }
        return null;
    }

    /// <summary>Call after recipes are finalized; cheap to call repeatedly.</summary>
    internal static void EnsureBuilt()
    {
        if (_built)
            return;
        _built = true;
        try
        {
            BuildRecipes();
            BuildNpcDrops();
            BuildContainerDrops();
            BuildShops();
            BuildShimmer();
            BuildCatchAndBanners();
            BuildObtain();
        }
        catch (Exception e)
        {
            KhuMue.Instance.Logger.Error("Building the item index failed; some sources may be missing.", e);
        }
    }

    /// <summary>Names depend on the active language; clear caches that hold text.</summary>
    internal static void OnLanguageChanged()
    {
        NpcBiomes.Clear();
        TileNames.Clear();
    }

    // ------------------------------------------------------------------ keys

    /// <summary>"Terraria/Name" or "Mod/Name" to item type; 0 when not loaded.</summary>
    internal static int ItemType(string key)
    {
        if (string.IsNullOrEmpty(key))
            return 0;
        if (ItemTypeCache.TryGetValue(key, out int cached))
            return cached;
        int type = 0;
        int slash = key.IndexOf('/');
        if (slash > 0)
        {
            string mod = key[..slash];
            string name = key[(slash + 1)..];
            if (mod == "Terraria")
            {
                if (ItemID.Search.TryGetId(name, out int id))
                    type = id;
            }
            else if (ModContent.TryFind(mod, name, out ModItem modItem))
            {
                type = modItem.Type;
            }
        }
        ItemTypeCache[key] = type;
        return type;
    }

    internal static int NpcType(string key)
    {
        if (string.IsNullOrEmpty(key))
            return 0;
        if (NpcTypeCache.TryGetValue(key, out int cached))
            return cached;
        int type = 0;
        int slash = key.IndexOf('/');
        if (slash > 0)
        {
            string mod = key[..slash];
            string name = key[(slash + 1)..];
            if (mod == "Terraria")
            {
                if (NPCID.Search.TryGetId(name, out int id))
                    type = id;
            }
            else if (ModContent.TryFind(mod, name, out ModNPC modNpc))
            {
                type = modNpc.Type;
            }
        }
        NpcTypeCache[key] = type;
        return type;
    }

    internal static string ItemKey(int type)
    {
        if (type <= 0)
            return "";
        if (type < ItemID.Count)
            return "Terraria/" + ItemID.Search.GetName(type);
        ModItem? modItem = ItemLoader.GetItem(type);
        return modItem is null ? "" : modItem.Mod.Name + "/" + modItem.Name;
    }

    internal static string ItemModName(int type)
    {
        if (type < ItemID.Count)
            return "Terraria";
        return ItemLoader.GetItem(type)?.Mod.Name ?? "Terraria";
    }

    internal static string NpcModName(int type)
    {
        if (type < NPCID.Count)
            return "Terraria";
        return NPCLoader.GetNPC(type)?.Mod.Name ?? "Terraria";
    }

    internal static Item SampleItem(int type)
    {
        if (ContentSamples.ItemsByType.TryGetValue(type, out Item? item))
            return item;
        var created = new Item();
        created.SetDefaults(type);
        return created;
    }

    /// <summary>
    /// Display name of an item. Some mods keep a format placeholder in the base name and fill it in per item
    /// ("{0} Prefix Voucher"); the sample item's own name is used then, or the placeholder is dropped.
    /// </summary>
    internal static string ItemName(int type)
    {
        string name = Lang.GetItemNameValue(type);
        if (!name.Contains('{'))
            return name;
        string sample = SampleItem(type).Name;
        if (!sample.Contains('{') && sample.Length > 0)
            return sample;
        return System.Text.RegularExpressions.Regex.Replace(name, @"\{\d+\}\s*", "").Trim();
    }

    internal static NPC? SampleNpc(int type)
    {
        return ContentSamples.NpcsByNetId.TryGetValue(type, out NPC? npc) ? npc : null;
    }

    // ------------------------------------------------------------------ queries

    internal static IReadOnlyList<ItemSource> SourcesFor(int itemType)
    {
        EnsureBuilt();
        return SourcesByItem.TryGetValue(itemType, out List<ItemSource>? list) ? list : Array.Empty<ItemSource>();
    }

    internal static IReadOnlyList<Recipe> RecipesFor(int itemType)
    {
        EnsureBuilt();
        return RecipesByResult.TryGetValue(itemType, out List<Recipe>? list) ? list : Array.Empty<Recipe>();
    }

    internal static IReadOnlyList<Recipe> RecipesUsing(int itemType)
    {
        EnsureBuilt();
        return RecipesByIngredient.TryGetValue(itemType, out List<Recipe>? list) ? list : Array.Empty<Recipe>();
    }

    /// <summary>Items an NPC can drop directly (not through its treasure bag).</summary>
    internal static IReadOnlyList<(int Item, float Chance)> DropsOf(int npcType)
    {
        EnsureBuilt();
        return DropsByNpc.TryGetValue(npcType, out var list) ? list : Array.Empty<(int, float)>();
    }

    /// <summary>Treasure bag(s) an NPC drops, if any.</summary>
    internal static IReadOnlyList<int> BagsOf(int npcType)
    {
        EnsureBuilt();
        return BagsByNpc.TryGetValue(npcType, out List<int>? list) ? list : Array.Empty<int>();
    }

    /// <summary>Contents of a bag/crate/container item.</summary>
    internal static List<DropRateInfo> ContainerContents(int itemType)
    {
        var drops = new List<DropRateInfo>();
        try
        {
            var feed = new DropRateInfoChainFeed(1f);
            foreach (IItemDropRule rule in Main.ItemDropsDB.GetRulesForItemID(itemType))
                rule.ReportDroprates(drops, feed);
        }
        catch
        {
            // A broken third-party rule must not break the guide.
        }
        return drops;
    }

    /// <summary>Full drop table of an NPC, including conditions.</summary>
    internal static List<DropRateInfo> NpcLoot(int npcType)
    {
        var drops = new List<DropRateInfo>();
        try
        {
            var feed = new DropRateInfoChainFeed(1f);
            foreach (IItemDropRule rule in Main.ItemDropsDB.GetRulesForNPCID(npcType, includeGlobalDrops: false))
                rule.ReportDroprates(drops, feed);
        }
        catch
        {
        }
        return drops;
    }

    /// <summary>Biome/time tags from the Bestiary (localized), e.g. "The Dungeon", "Nighttime".</summary>
    internal static IReadOnlyList<string> Biomes(int npcType)
    {
        if (NpcBiomes.TryGetValue(npcType, out List<string>? cached))
            return cached;
        var result = new List<string>();
        try
        {
            BestiaryEntry? entry = Main.BestiaryDB?.FindEntryByNPCID(npcType);
            if (entry?.Info is not null)
            {
                foreach (IBestiaryInfoElement info in entry.Info)
                {
                    if (info is ModSourceBestiaryInfoElement)
                        continue;
                    if (info is SpawnConditionBestiaryInfoElement or SpawnConditionBestiaryOverlayInfoElement or ModBiomeBestiaryInfoElement
                        && info is IFilterInfoProvider provider)
                    {
                        string text = Language.GetTextValue(provider.GetDisplayNameKey());
                        if (!string.IsNullOrWhiteSpace(text) && !result.Contains(text))
                            result.Add(text);
                    }
                }
            }
        }
        catch
        {
        }
        NpcBiomes[npcType] = result;
        return result;
    }

    /// <summary>Localized crafting station name.</summary>
    internal static string TileName(int tileType)
    {
        if (TileNames.TryGetValue(tileType, out string? cached))
            return cached;
        string name = "";
        try
        {
            name = Lang.GetMapObjectName(MapHelper.TileToLookup(tileType, 0));
        }
        catch
        {
        }
        if (string.IsNullOrWhiteSpace(name))
        {
            int placer = TilePlacer(tileType);
            name = placer > 0 ? ItemName(placer) : (TileLoader.GetTile(tileType)?.Name ?? "#" + tileType);
        }
        TileNames[tileType] = name;
        return name;
    }

    private static readonly Dictionary<int, int> TilePlacers = new();

    /// <summary>An item that places this tile (for icons and links), 0 if none.</summary>
    internal static int TilePlacer(int tileType)
    {
        if (TilePlacers.Count == 0)
        {
            foreach ((int type, Item item) in ContentSamples.ItemsByType)
            {
                if (item.createTile >= 0 && !TilePlacers.ContainsKey(item.createTile))
                    TilePlacers[item.createTile] = type;
            }
            // Vanilla ids share several stations; prefer the plain versions.
            TilePlacers[TileID.WorkBenches] = ItemID.WorkBench;
            TilePlacers[TileID.Anvils] = ItemID.IronAnvil;
            TilePlacers[TileID.MythrilAnvil] = ItemID.MythrilAnvil;
            TilePlacers[TileID.Furnaces] = ItemID.Furnace;
            TilePlacers[TileID.Hellforge] = ItemID.Hellforge;
            TilePlacers[TileID.AdamantiteForge] = ItemID.AdamantiteForge;
            TilePlacers[TileID.Tables] = ItemID.WoodenTable;
            TilePlacers[TileID.Chairs] = ItemID.WoodenChair;
            TilePlacers[TileID.Bottles] = ItemID.Bottle;
            TilePlacers[TileID.Sawmill] = ItemID.Sawmill;
            TilePlacers[TileID.Loom] = ItemID.Loom;
            TilePlacers[TileID.Sinks] = ItemID.MetalSink;
            TilePlacers[TileID.DemonAltar] = ItemID.None;
        }
        return TilePlacers.TryGetValue(tileType, out int placer) ? placer : 0;
    }

    // ------------------------------------------------------------------ building

    private static void AddSource(int item, ItemSource source)
    {
        if (item <= 0)
            return;
        if (!SourcesByItem.TryGetValue(item, out List<ItemSource>? list))
            SourcesByItem[item] = list = new List<ItemSource>();
        list.Add(source);
    }

    private static void BuildRecipes()
    {
        for (int i = 0; i < Recipe.numRecipes; i++)
        {
            Recipe recipe = Main.recipe[i];
            if (recipe?.createItem is null || recipe.createItem.type <= ItemID.None || recipe.Disabled)
                continue;

            int result = recipe.createItem.type;
            if (!RecipesByResult.TryGetValue(result, out List<Recipe>? byResult))
                RecipesByResult[result] = byResult = new List<Recipe>();
            byResult.Add(recipe);

            var ingredients = new HashSet<int>();
            foreach (Item ingredient in recipe.requiredItem)
            {
                if (ingredient.type > ItemID.None)
                    ingredients.Add(ingredient.type);
            }
            foreach (int groupId in recipe.acceptedGroups)
            {
                if (RecipeGroup.recipeGroups.TryGetValue(groupId, out RecipeGroup? group))
                {
                    foreach (int member in group.ValidItems)
                        ingredients.Add(member);
                }
            }
            foreach (int ingredient in ingredients)
            {
                if (!RecipesByIngredient.TryGetValue(ingredient, out List<Recipe>? byIngredient))
                    RecipesByIngredient[ingredient] = byIngredient = new List<Recipe>();
                byIngredient.Add(recipe);
            }
        }
    }

    private static IReadOnlyList<string> ConditionTexts(List<IItemDropRuleCondition>? conditions)
    {
        if (conditions is null || conditions.Count == 0)
            return Array.Empty<string>();
        var texts = new List<string>();
        foreach (IItemDropRuleCondition condition in conditions)
        {
            string? text = null;
            try
            {
                if (condition.CanShowItemDropInUI())
                    text = condition.GetConditionDescription();
            }
            catch
            {
            }
            if (!string.IsNullOrWhiteSpace(text) && !texts.Contains(text))
                texts.Add(text);
        }
        return texts;
    }

    private static void BuildNpcDrops()
    {
        for (int npc = -65; npc < NPCLoader.NPCCount; npc++)
        {
            if (npc == 0 || !ContentSamples.NpcsByNetId.ContainsKey(npc))
                continue;
            List<IItemDropRule> rules;
            try
            {
                rules = Main.ItemDropsDB.GetRulesForNPCID(npc, includeGlobalDrops: false);
            }
            catch
            {
                continue;
            }
            if (rules.Count == 0)
                continue;

            var drops = new List<DropRateInfo>();
            var feed = new DropRateInfoChainFeed(1f);
            foreach (IItemDropRule rule in rules)
            {
                try
                {
                    rule.ReportDroprates(drops, feed);
                }
                catch
                {
                }
            }

            var summary = new List<(int, float)>();
            foreach (DropRateInfo drop in drops)
            {
                if (drop.itemId <= 0)
                    continue;
                if (ItemID.Sets.BossBag[drop.itemId])
                {
                    if (!BagsByNpc.TryGetValue(npc, out List<int>? bags))
                        BagsByNpc[npc] = bags = new List<int>();
                    if (!bags.Contains(drop.itemId))
                        bags.Add(drop.itemId);
                }
                summary.Add((drop.itemId, drop.dropRate));
                AddSource(drop.itemId, new ItemSource
                {
                    Kind = SourceKind.NpcDrop,
                    Npc = npc,
                    Chance = drop.dropRate,
                    Min = drop.stackMin,
                    Max = drop.stackMax,
                    Conditions = ConditionTexts(drop.conditions),
                });
            }
            DropsByNpc[npc] = summary;
        }

        // Global rules (e.g. hearts, souls): rules with globals minus rules without, sampled on one NPC.
        try
        {
            var without = new HashSet<IItemDropRule>(Main.ItemDropsDB.GetRulesForNPCID(NPCID.BlueSlime, includeGlobalDrops: false));
            var drops = new List<DropRateInfo>();
            var feed = new DropRateInfoChainFeed(1f);
            foreach (IItemDropRule rule in Main.ItemDropsDB.GetRulesForNPCID(NPCID.BlueSlime, includeGlobalDrops: true))
            {
                if (without.Contains(rule))
                    continue;
                try
                {
                    rule.ReportDroprates(drops, feed);
                }
                catch
                {
                }
            }
            foreach (DropRateInfo drop in drops)
            {
                AddSource(drop.itemId, new ItemSource
                {
                    Kind = SourceKind.GlobalDrop,
                    Chance = drop.dropRate,
                    Min = drop.stackMin,
                    Max = drop.stackMax,
                    Conditions = ConditionTexts(drop.conditions),
                });
            }
        }
        catch
        {
        }
    }

    private static void BuildContainerDrops()
    {
        for (int container = 1; container < ItemLoader.ItemCount; container++)
        {
            List<IItemDropRule> rules;
            try
            {
                rules = Main.ItemDropsDB.GetRulesForItemID(container);
            }
            catch
            {
                continue;
            }
            if (rules.Count == 0)
                continue;
            var drops = new List<DropRateInfo>();
            var feed = new DropRateInfoChainFeed(1f);
            foreach (IItemDropRule rule in rules)
            {
                try
                {
                    rule.ReportDroprates(drops, feed);
                }
                catch
                {
                }
            }
            foreach (DropRateInfo drop in drops)
            {
                AddSource(drop.itemId, new ItemSource
                {
                    Kind = SourceKind.Container,
                    FromItem = container,
                    Chance = drop.dropRate,
                    Min = drop.stackMin,
                    Max = drop.stackMax,
                    Conditions = ConditionTexts(drop.conditions),
                });
            }
        }
    }

    private static void BuildShops()
    {
        foreach (AbstractNPCShop shop in NPCShopDatabase.AllShops)
        {
            IEnumerable<AbstractNPCShop.Entry> entries;
            try
            {
                entries = shop is NPCShop npcShop ? npcShop.Entries.Where(e => !e.Disabled) : shop.ActiveEntries;
            }
            catch
            {
                continue;
            }
            foreach (AbstractNPCShop.Entry entry in entries)
            {
                Item? item = entry.Item;
                if (item is null || item.type <= ItemID.None)
                    continue;
                var conditions = new List<string>();
                foreach (Terraria.Condition condition in entry.Conditions)
                {
                    string text = condition.Description.Value;
                    if (!string.IsNullOrWhiteSpace(text) && !conditions.Contains(text))
                        conditions.Add(text);
                }
                AddSource(item.type, new ItemSource
                {
                    Kind = SourceKind.Shop,
                    Npc = shop.NpcType,
                    Price = item.shopCustomPrice ?? item.value,
                    Currency = item.shopSpecialCurrency,
                    Conditions = conditions,
                });
            }
        }
    }

    private static void BuildShimmer()
    {
        for (int type = 1; type < ItemLoader.ItemCount; type++)
        {
            int to = ItemID.Sets.ShimmerTransformToItem[type];
            if (to > 0)
                AddSource(to, new ItemSource { Kind = SourceKind.Shimmer, FromItem = type });
        }
    }

    private static void BuildCatchAndBanners()
    {
        var bannersSeen = new HashSet<int>();
        foreach ((int npc, NPC sample) in ContentSamples.NpcsByNetId)
        {
            if (npc <= 0)
                continue;
            if (sample.catchItem > 0)
                AddSource(sample.catchItem, new ItemSource { Kind = SourceKind.Catch, Npc = npc });

            int banner = Item.NPCtoBanner(npc);
            if (banner <= 0 || !bannersSeen.Add(banner))
                continue;
            int bannerItem = Item.BannerToItem(banner);
            if (bannerItem > 0)
                AddSource(bannerItem, new ItemSource { Kind = SourceKind.Banner, Npc = npc, Min = ItemID.Sets.KillsToBanner[bannerItem] });
        }
    }

    private static void BuildObtain()
    {
        foreach ((string key, List<ObtainInfo> infos) in GuideData.Obtain)
        {
            int type = ItemType(key);
            if (type <= 0)
                continue;
            foreach (ObtainInfo info in infos)
            {
                if (info.With is not null && !ModLoader.HasMod(info.With) || info.Without is not null && ModLoader.HasMod(info.Without))
                    continue;
                if (info.How == "chest" && info.Chest is not null)
                    AddSource(type, new ItemSource { Kind = SourceKind.Chest, Chest = info.Chest });
                else if (info.How == "fishing")
                    AddSource(type, new ItemSource { Kind = SourceKind.Fishing, Where = info.Where });
                else if (info.How == "angler")
                    AddSource(type, new ItemSource { Kind = SourceKind.Angler });
            }
        }
    }

    // ------------------------------------------------------------------ recipes

    internal static IEnumerable<Recipe> AllRecipes()
    {
        for (int i = 0; i < Recipe.numRecipes; i++)
        {
            Recipe recipe = Main.recipe[i];
            if (recipe?.createItem is not null && recipe.createItem.type > ItemID.None && !recipe.Disabled)
                yield return recipe;
        }
    }

    /// <summary>Recipe group accepted for an ingredient, or null when the ingredient is exact.</summary>
    internal static RecipeGroup? GroupFor(Recipe recipe, Item ingredient)
    {
        foreach (int groupId in recipe.acceptedGroups)
        {
            if (RecipeGroup.recipeGroups.TryGetValue(groupId, out RecipeGroup? group) && group.ContainsItem(ingredient.type))
                return group;
        }
        return null;
    }
}
