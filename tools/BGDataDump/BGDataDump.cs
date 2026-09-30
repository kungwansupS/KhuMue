using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;

namespace BGDataDump;

public sealed class BGDataDump : Mod
{
}

/// <summary>
/// Writes the live registries of the loaded mod pack to JSON (BG_DUMP_OUT directory).
/// Everything here is read-only; nothing is changed in the game.
/// </summary>
public sealed class DumpSystem : ModSystem
{
    private static readonly JsonWriterOptions WriterOptions = new()
    {
        Indented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public override void PostSetupRecipes()
    {
        string outDir = Environment.GetEnvironmentVariable("BG_DUMP_OUT");
        if (string.IsNullOrWhiteSpace(outDir))
            return;

        Directory.CreateDirectory(outDir);
        var log = new List<string>();
        void Step(string name, Action<string> action)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                action(Path.Combine(outDir, name + ".json"));
                log.Add($"OK {name} {sw.ElapsedMilliseconds}ms");
            }
            catch (Exception e)
            {
                log.Add($"FAIL {name}: {e}");
            }
        }

        Step("mods", DumpMods);
        Step("items", DumpItems);
        Step("recipeGroups", DumpRecipeGroups);
        Step("recipes", DumpRecipes);
        Step("tiles", DumpTiles);
        Step("npcs", DumpNpcs);
        Step("npcLoot", DumpNpcLoot);
        Step("globalLoot", DumpGlobalLoot);
        Step("itemLoot", DumpItemLoot);
        Step("shops", DumpShops);
        Step("shimmer", DumpShimmer);
        Step("buffs", DumpBuffs);
        Step("bosschecklist", DumpBossChecklist);
        Step("bestiary", DumpBestiary);

        File.WriteAllLines(Path.Combine(outDir, "_log.txt"), log);
        File.WriteAllText(Path.Combine(outDir, "_done.txt"), DateTime.Now.ToString("O"));

        if (Environment.GetEnvironmentVariable("BG_DUMP_EXIT") == "1")
            Environment.Exit(0);
    }

    // ---------- helpers ----------

    private static void WithWriter(string path, Action<Utf8JsonWriter> body)
    {
        using var stream = File.Create(path);
        using var w = new Utf8JsonWriter(stream, WriterOptions);
        body(w);
        w.Flush();
    }

    private static string ItemKey(int type)
    {
        if (type <= 0)
            return "";
        if (type < ItemID.Count)
            return "Terraria/" + ItemID.Search.GetName(type);
        var mi = ItemLoader.GetItem(type);
        return mi != null ? mi.Mod.Name + "/" + mi.Name : "?/" + type;
    }

    private static string NpcKey(int type)
    {
        if (type < NPCID.Count)
            return "Terraria/" + NPCID.Search.GetName(type);
        var mn = NPCLoader.GetNPC(type);
        return mn != null ? mn.Mod.Name + "/" + mn.Name : "?/" + type;
    }

    private static string TileKey(int type)
    {
        if (type < 0)
            return "";
        if (type < TileID.Count)
            return "Terraria/" + TileID.Search.GetName(type);
        var mt = TileLoader.GetTile(type);
        return mt != null ? mt.Mod.Name + "/" + mt.Name : "?/" + type;
    }

    private static string Safe(Func<string> f)
    {
        try { return f() ?? ""; }
        catch { return ""; }
    }

    private static void WriteConditions(Utf8JsonWriter w, string name, IEnumerable<Condition> conditions)
    {
        w.WriteStartArray(name);
        if (conditions != null)
        {
            foreach (var c in conditions)
            {
                w.WriteStartObject();
                w.WriteString("key", Safe(() => c.Description.Key));
                w.WriteString("text", Safe(() => c.Description.Value));
                w.WriteEndObject();
            }
        }
        w.WriteEndArray();
    }

    private static void WriteDropRates(Utf8JsonWriter w, List<DropRateInfo> drops)
    {
        w.WriteStartArray();
        foreach (var d in drops)
        {
            w.WriteStartObject();
            w.WriteNumber("item", d.itemId);
            w.WriteString("itemKey", ItemKey(d.itemId));
            w.WriteNumber("min", d.stackMin);
            w.WriteNumber("max", d.stackMax);
            w.WriteNumber("rate", d.dropRate);
            w.WriteStartArray("conditions");
            if (d.conditions != null)
            {
                foreach (var c in d.conditions)
                {
                    w.WriteStartObject();
                    w.WriteString("type", c.GetType().FullName);
                    w.WriteString("text", Safe(() => c.GetConditionDescription()));
                    bool show = true;
                    try { show = c.CanShowItemDropInUI(); } catch { }
                    w.WriteBoolean("showInUI", show);
                    w.WriteEndObject();
                }
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }
        w.WriteEndArray();
    }

    private static List<DropRateInfo> Report(IEnumerable<IItemDropRule> rules)
    {
        var drops = new List<DropRateInfo>();
        var feed = new DropRateInfoChainFeed(1f);
        foreach (var rule in rules)
        {
            try { rule.ReportDroprates(drops, feed); }
            catch { }
        }
        return drops;
    }

    // ---------- dumps ----------

    private static void DumpMods(string path) => WithWriter(path, w =>
    {
        w.WriteStartArray();
        foreach (var mod in ModLoader.Mods)
        {
            w.WriteStartObject();
            w.WriteString("name", mod.Name);
            w.WriteString("displayName", mod.DisplayName);
            w.WriteString("version", mod.Version.ToString());
            w.WriteEndObject();
        }
        w.WriteEndArray();
    });

    private static void DumpItems(string path) => WithWriter(path, w =>
    {
        w.WriteStartArray();
        for (int type = 1; type < ItemLoader.ItemCount; type++)
        {
            if (!ContentSamples.ItemsByType.TryGetValue(type, out var item))
                continue;
            w.WriteStartObject();
            w.WriteNumber("type", type);
            w.WriteString("key", ItemKey(type));
            w.WriteString("name", Safe(() => Lang.GetItemNameValue(type)));
            w.WriteNumber("rare", item.rare);
            w.WriteNumber("value", item.value);
            w.WriteNumber("maxStack", item.maxStack);
            w.WriteNumber("damage", item.damage);
            w.WriteString("damageClass", Safe(() => item.DamageType?.FullName));
            w.WriteNumber("useTime", item.useTime);
            w.WriteNumber("useAnimation", item.useAnimation);
            w.WriteNumber("useStyle", item.useStyle);
            w.WriteNumber("knockBack", item.knockBack);
            w.WriteNumber("crit", item.crit);
            w.WriteNumber("mana", item.mana);
            w.WriteNumber("defense", item.defense);
            w.WriteBoolean("accessory", item.accessory);
            w.WriteBoolean("vanity", item.vanity);
            w.WriteNumber("headSlot", item.headSlot);
            w.WriteNumber("bodySlot", item.bodySlot);
            w.WriteNumber("legSlot", item.legSlot);
            w.WriteNumber("wingSlot", item.wingSlot);
            w.WriteNumber("pick", item.pick);
            w.WriteNumber("axe", item.axe);
            w.WriteNumber("hammer", item.hammer);
            w.WriteNumber("createTile", item.createTile);
            w.WriteString("createTileKey", TileKey(item.createTile));
            w.WriteNumber("createWall", item.createWall);
            w.WriteBoolean("consumable", item.consumable);
            w.WriteBoolean("material", item.material);
            w.WriteNumber("ammo", item.ammo);
            w.WriteNumber("useAmmo", item.useAmmo);
            w.WriteNumber("shoot", item.shoot);
            w.WriteNumber("buffType", item.buffType);
            w.WriteNumber("buffTime", item.buffTime);
            w.WriteNumber("healLife", item.healLife);
            w.WriteNumber("healMana", item.healMana);
            w.WriteNumber("fishingPole", item.fishingPole);
            w.WriteNumber("bait", item.bait);
            w.WriteNumber("mountType", item.mountType);
            w.WriteBoolean("expert", item.expert);
            w.WriteBoolean("master", item.master);
            w.WriteBoolean("questItem", item.questItem);
            w.WriteBoolean("channel", item.channel);
            w.WriteBoolean("noMelee", item.noMelee);
            w.WriteBoolean("bossBag", ItemID.Sets.BossBag[type]);
            w.WriteBoolean("crate", ItemID.Sets.IsFishingCrate[type]);
            w.WriteNumber("sortBossSpawn", ItemID.Sets.SortingPriorityBossSpawns[type]);
            w.WriteNumber("shimmerTo", ItemID.Sets.ShimmerTransformToItem[type]);
            w.WriteString("class", item.ModItem?.GetType().FullName ?? "");
            w.WriteStartArray("tooltip");
            try
            {
                var tip = Lang.GetTooltip(type);
                for (int i = 0; i < tip.Lines; i++)
                    w.WriteStringValue(tip.GetLine(i) ?? "");
            }
            catch { }
            w.WriteEndArray();
            w.WriteEndObject();
        }
        w.WriteEndArray();
    });

    private static void DumpRecipeGroups(string path) => WithWriter(path, w =>
    {
        w.WriteStartArray();
        foreach (var (id, group) in RecipeGroup.recipeGroups)
        {
            w.WriteStartObject();
            w.WriteNumber("id", id);
            string name = RecipeGroup.recipeGroupIDs.FirstOrDefault(p => p.Value == id).Key ?? "";
            w.WriteString("name", name);
            w.WriteString("text", Safe(() => group.GetText()));
            w.WriteNumber("iconic", group.IconicItemId);
            w.WriteStartArray("items");
            foreach (int t in group.ValidItems.OrderBy(t => t))
                w.WriteStringValue(ItemKey(t));
            w.WriteEndArray();
            w.WriteEndObject();
        }
        w.WriteEndArray();
    });

    private static void DumpRecipes(string path) => WithWriter(path, w =>
    {
        w.WriteStartArray();
        for (int i = 0; i < Recipe.numRecipes; i++)
        {
            var r = Main.recipe[i];
            if (r == null || r.createItem == null || r.createItem.type <= 0)
                continue;
            w.WriteStartObject();
            w.WriteNumber("index", i);
            w.WriteString("mod", r.Mod?.Name ?? "Terraria");
            w.WriteBoolean("disabled", r.Disabled);
            w.WriteString("result", ItemKey(r.createItem.type));
            w.WriteNumber("resultStack", r.createItem.stack);
            w.WriteStartArray("ingredients");
            foreach (var ing in r.requiredItem)
            {
                w.WriteStartObject();
                w.WriteString("item", ItemKey(ing.type));
                w.WriteNumber("stack", ing.stack);
                w.WriteEndObject();
            }
            w.WriteEndArray();
            w.WriteStartArray("groups");
            foreach (int g in r.acceptedGroups)
            {
                string name = RecipeGroup.recipeGroupIDs.FirstOrDefault(p => p.Value == g).Key ?? g.ToString();
                w.WriteStringValue(name);
            }
            w.WriteEndArray();
            w.WriteStartArray("tiles");
            foreach (int t in r.requiredTile)
                w.WriteStringValue(TileKey(t));
            w.WriteEndArray();
            WriteConditions(w, "conditions", r.Conditions);
            WriteConditions(w, "decraftConditions", r.DecraftConditions);
            w.WriteEndObject();
        }
        w.WriteEndArray();
    });

    private static void DumpTiles(string path) => WithWriter(path, w =>
    {
        var placers = new Dictionary<int, List<string>>();
        for (int type = 1; type < ItemLoader.ItemCount; type++)
        {
            if (!ContentSamples.ItemsByType.TryGetValue(type, out var item) || item.createTile < 0)
                continue;
            if (!placers.TryGetValue(item.createTile, out var list))
                placers[item.createTile] = list = new List<string>();
            if (list.Count < 6)
                list.Add(ItemKey(type));
        }

        w.WriteStartArray();
        for (int t = 0; t < TileLoader.TileCount; t++)
        {
            w.WriteStartObject();
            w.WriteNumber("type", t);
            w.WriteString("key", TileKey(t));
            w.WriteString("mapName", Safe(() => Lang.GetMapObjectName(MapHelper.TileToLookup(t, 0))));
            w.WriteStartArray("placedBy");
            if (placers.TryGetValue(t, out var list))
                foreach (var s in list)
                    w.WriteStringValue(s);
            w.WriteEndArray();
            w.WriteEndObject();
        }
        w.WriteEndArray();
    });

    private static void DumpNpcs(string path) => WithWriter(path, w =>
    {
        w.WriteStartArray();
        for (int type = -65; type < NPCLoader.NPCCount; type++)
        {
            if (type == 0 || !ContentSamples.NpcsByNetId.TryGetValue(type, out var npc))
                continue;
            w.WriteStartObject();
            w.WriteNumber("type", type);
            w.WriteString("key", type > 0 ? NpcKey(type) : "Terraria/" + NPCID.Search.GetName(type));
            w.WriteString("name", Safe(() => Lang.GetNPCNameValue(type)));
            w.WriteBoolean("boss", npc.boss);
            w.WriteNumber("lifeMax", npc.lifeMax);
            w.WriteNumber("damage", npc.damage);
            w.WriteNumber("defense", npc.defense);
            w.WriteBoolean("townNPC", npc.townNPC);
            w.WriteBoolean("friendly", npc.friendly);
            w.WriteBoolean("countsAsBoss", type > 0 && NPCID.Sets.ShouldBeCountedAsBoss[type]);
            w.WriteNumber("bossHead", type > 0 ? NPCID.Sets.BossHeadTextures[type] : -1);
            w.WriteNumber("catchItem", npc.catchItem);
            int banner = type > 0 ? Item.NPCtoBanner(type) : 0;
            w.WriteNumber("banner", banner);
            w.WriteString("bannerItem", banner > 0 ? ItemKey(Item.BannerToItem(banner)) : "");
            w.WriteNumber("value", npc.value);
            w.WriteString("class", npc.ModNPC?.GetType().FullName ?? "");
            w.WriteEndObject();
        }
        w.WriteEndArray();
    });

    private static void DumpNpcLoot(string path) => WithWriter(path, w =>
    {
        w.WriteStartObject();
        for (int type = -65; type < NPCLoader.NPCCount; type++)
        {
            if (type == 0)
                continue;
            List<IItemDropRule> rules;
            try { rules = Main.ItemDropsDB.GetRulesForNPCID(type, includeGlobalDrops: false); }
            catch { continue; }
            if (rules == null || rules.Count == 0)
                continue;
            var drops = Report(rules);
            if (drops.Count == 0)
                continue;
            w.WritePropertyName(type > 0 ? NpcKey(type) : "Terraria/" + NPCID.Search.GetName(type));
            WriteDropRates(w, drops);
        }
        w.WriteEndObject();
    });

    private static void DumpGlobalLoot(string path) => WithWriter(path, w =>
    {
        // Global rules = rules with globals minus rules without, sampled on a Blue Slime.
        var with = Main.ItemDropsDB.GetRulesForNPCID(NPCID.BlueSlime, includeGlobalDrops: true);
        var without = new HashSet<IItemDropRule>(Main.ItemDropsDB.GetRulesForNPCID(NPCID.BlueSlime, includeGlobalDrops: false));
        WriteDropRates(w, Report(with.Where(r => !without.Contains(r))));
    });

    private static void DumpItemLoot(string path) => WithWriter(path, w =>
    {
        w.WriteStartObject();
        for (int type = 1; type < ItemLoader.ItemCount; type++)
        {
            List<IItemDropRule> rules;
            try { rules = Main.ItemDropsDB.GetRulesForItemID(type); }
            catch { continue; }
            if (rules == null || rules.Count == 0)
                continue;
            var drops = Report(rules);
            if (drops.Count == 0)
                continue;
            w.WritePropertyName(ItemKey(type));
            WriteDropRates(w, drops);
        }
        w.WriteEndObject();
    });

    private static void DumpShops(string path) => WithWriter(path, w =>
    {
        w.WriteStartArray();
        foreach (var shop in NPCShopDatabase.AllShops)
        {
            w.WriteStartObject();
            w.WriteString("shopType", shop.GetType().FullName);
            w.WriteString("npc", NpcKey(shop.NpcType));
            w.WriteString("name", shop.Name);
            w.WriteString("fullName", shop.FullName);
            w.WriteStartArray("entries");
            IEnumerable<AbstractNPCShop.Entry> entries = shop is NPCShop npcShop
                ? npcShop.Entries.Where(e => !e.Disabled)
                : shop.ActiveEntries;
            foreach (var entry in entries)
            {
                var item = entry.Item;
                if (item == null || item.type <= 0)
                    continue;
                w.WriteStartObject();
                w.WriteString("item", ItemKey(item.type));
                w.WriteNumber("stack", item.stack);
                w.WriteNumber("value", item.value);
                w.WriteNumber("customPrice", item.shopCustomPrice ?? -1);
                w.WriteNumber("specialCurrency", item.shopSpecialCurrency);
                WriteConditions(w, "conditions", entry.Conditions);
                w.WriteEndObject();
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }
        w.WriteEndArray();
    });

    private static void DumpShimmer(string path) => WithWriter(path, w =>
    {
        w.WriteStartObject();
        w.WriteStartObject("items");
        for (int type = 1; type < ItemLoader.ItemCount; type++)
        {
            int to = ItemID.Sets.ShimmerTransformToItem[type];
            if (to > 0)
                w.WriteString(ItemKey(type), ItemKey(to));
        }
        w.WriteEndObject();
        w.WriteStartObject("npcs");
        for (int type = 1; type < NPCLoader.NPCCount; type++)
        {
            int to = NPCID.Sets.ShimmerTransformToNPC[type];
            if (to > 0)
                w.WriteString(NpcKey(type), NpcKey(to));
        }
        w.WriteEndObject();
        w.WriteEndObject();
    });

    private static void DumpBuffs(string path) => WithWriter(path, w =>
    {
        w.WriteStartArray();
        for (int type = 1; type < BuffLoader.BuffCount; type++)
        {
            w.WriteStartObject();
            w.WriteNumber("type", type);
            var mb = BuffLoader.GetBuff(type);
            w.WriteString("key", type < BuffID.Count ? "Terraria/" + BuffID.Search.GetName(type) : mb != null ? mb.Mod.Name + "/" + mb.Name : "?");
            w.WriteString("name", Safe(() => Lang.GetBuffName(type)));
            w.WriteString("description", Safe(() => Lang.GetBuffDescription(type)));
            w.WriteBoolean("debuff", Main.debuff[type]);
            w.WriteEndObject();
        }
        w.WriteEndArray();
    });

    private void DumpBossChecklist(string path)
    {
        if (!ModLoader.TryGetMod("BossChecklist", out var bc))
        {
            File.WriteAllText(path, "null");
            return;
        }
        var dict = bc.Call("GetBossInfoDictionary", Mod, "2.0") as Dictionary<string, Dictionary<string, object>>;
        WithWriter(path, w =>
        {
            w.WriteStartArray();
            if (dict != null)
            {
                foreach (var (key, info) in dict)
                {
                    w.WriteStartObject();
                    w.WriteString("key", key);
                    foreach (var (k, v) in info)
                    {
                        switch (v)
                        {
                            case string s: w.WriteString(k, s); break;
                            case bool b: w.WriteBoolean(k, b); break;
                            case float f: w.WriteNumber(k, f); break;
                            case int i: w.WriteNumber(k, i); break;
                            case LocalizedText lt: w.WriteString(k, Safe(() => lt.Value)); break;
                            case Func<LocalizedText> flt: w.WriteString(k, Safe(() => flt()?.Value)); break;
                            case Func<bool>: break; // world-dependent, not meaningful without a world
                            case List<int> ints when k == "npcIDs":
                                w.WriteStartArray(k);
                                foreach (int n in ints) w.WriteStringValue(NpcKey(n));
                                w.WriteEndArray();
                                break;
                            case List<int> ints:
                                w.WriteStartArray(k);
                                foreach (int n in ints) w.WriteStringValue(ItemKey(n));
                                w.WriteEndArray();
                                break;
                            case List<DropRateInfo> drops:
                                w.WritePropertyName(k);
                                WriteDropRates(w, drops);
                                break;
                            default:
                                w.WriteString(k, v?.GetType().FullName ?? "null");
                                break;
                        }
                    }
                    w.WriteEndObject();
                }
            }
            w.WriteEndArray();
        });
    }

    private static void DumpBestiary(string path) => WithWriter(path, w =>
    {
        w.WriteStartObject();
        if (Main.BestiaryDB == null)
        {
            w.WriteEndObject();
            return;
        }
        for (int type = 1; type < NPCLoader.NPCCount; type++)
        {
            BestiaryEntry entry;
            try { entry = Main.BestiaryDB.FindEntryByNPCID(type); }
            catch { continue; }
            if (entry?.Info == null || entry.Info.Count == 0)
                continue;
            w.WritePropertyName(NpcKey(type));
            w.WriteStartArray();
            foreach (var info in entry.Info)
            {
                if (info is IFilterInfoProvider provider)
                {
                    string keyText = Safe(() => provider.GetDisplayNameKey());
                    w.WriteStartObject();
                    w.WriteString("kind", info.GetType().Name);
                    w.WriteString("key", keyText);
                    w.WriteString("text", Safe(() => Language.GetTextValue(keyText)));
                    w.WriteEndObject();
                }
                else if (info is FlavorTextBestiaryInfoElement flavor)
                {
                    var f = typeof(FlavorTextBestiaryInfoElement).GetField("_key", BindingFlags.Instance | BindingFlags.NonPublic);
                    string key = f?.GetValue(flavor) as string ?? "";
                    w.WriteStartObject();
                    w.WriteString("kind", "Flavor");
                    w.WriteString("key", key);
                    w.WriteString("text", Safe(() => Language.GetTextValue(key)));
                    w.WriteEndObject();
                }
            }
            w.WriteEndArray();
        }
        w.WriteEndObject();
    });
}
