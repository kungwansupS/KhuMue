using KhuMue.Core;
using Terraria.GameContent.UI;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Everything about one item: stats, the live tooltip, how to get it and what it makes.</summary>
internal sealed class ItemPage(int type) : Page
{
    private readonly int _type = type;
    private static bool _showAllUses;

    public override string Title => GuideCatalog.ItemName(_type);
    public override Section Section => Section.Items;
    public override string Key => "Item:" + _type;

    public override Node Build(int width)
    {
        var page = new VStack { Spacing = 14 };
        Item item = GuideCatalog.SampleItem(_type);
        page.Add(Header(item));
        string? warning = ItemWarning(_type);
        if (warning is not null)
            page.Add(Callout(warning, Theme.Warning, L("Entry.Warning")));

        List<(string Text, Color Color)> tooltip = TooltipLines(_type);
        if (tooltip.Count > 0)
        {
            var lines = new VStack { Spacing = 0 };
            foreach ((string text, Color color) in tooltip)
                lines.Add(new Paragraph(text, color, Theme.Body));
            page.Add(new Card(lines) { Fill = Theme.Slot, Padding = 12 });
        }

        page.Add(Sources(width));
        Node? uses = Uses();
        if (uses is not null)
            page.Add(uses);
        Node? related = Related();
        if (related is not null)
            page.Add(related);
        return page;
    }

    private Node Header(Item item)
    {
        var row = new HStack { Spacing = 14 };
        row.AddFixed(new ItemSlotNode(_type, 72) { OnClick = null }, 72);
        var info = new VStack { Spacing = 4 };
        info.Add(new Label(GuideCatalog.ItemName(_type), RichText.ItemColor(_type), Theme.Hero));
        var badges = new Flow { Spacing = 6 };
        string mod = GuideCatalog.ItemModName(_type);
        badges.Add(ModBadge(mod));
        string category = ItemSearch.CategoryOf(item);
        badges.Add(new Badge(L("Items.Categories." + category), Theme.Info));
        string cls = ItemSearch.ClassOf(item);
        if (cls.Length > 0)
            badges.Add(new Badge(ClassName(cls), Theme.ClassColor(cls)));
        int owned = Inventory.Count(_type);
        if (owned > 0)
            badges.Add(new Badge(L("Items.InInventory", owned), Theme.Success, solid: true));
        info.Add(badges);
        info.Add(new Label(GuideCatalog.ItemKey(_type), Theme.TextFaint, Theme.Small));
        row.AddFlex(info);
        return new Card(row) { Padding = 12 };
    }

    // ------------------------------------------------------------------ tooltip

    /// <summary>The same tooltip lines the game shows on hover (after every mod's ModifyTooltips).</summary>
    internal static List<(string Text, Color Color)> TooltipLines(int type)
    {
        var result = new List<(string, Color)>();
        try
        {
            Item item = GuideCatalog.SampleItem(type).Clone();
            int yoyoLogo = -1, researchLine = -1;
            int max = 30 + (item.ToolTip?.Lines ?? 0);
            int numLines = 1;
            var text = new string[max];
            var modifier = new bool[max];
            var badModifier = new bool[max];
            var names = new string[max];
            Main.MouseText_DrawItemTooltip_GetLinesInfo(item, ref yoyoLogo, ref researchLine, item.knockBack, ref numLines, text, modifier, badModifier, names, out int prefixIndex);
            List<TooltipLine> lines = ItemLoader.ModifyTooltips(item, ref numLines, names, ref text, ref modifier, ref badModifier, ref yoyoLogo, out Color?[] overrideColor, prefixIndex);
            foreach (TooltipLine line in lines)
            {
                if (!line.Visible || line.Name is "ItemName" or "Price" or "JourneyResearch" || string.IsNullOrWhiteSpace(line.Text))
                    continue;
                Color color = line.OverrideColor ?? (line.IsModifier ? (line.IsModifierBad ? new Color(190, 120, 120) : new Color(120, 190, 120)) : Theme.Text);
                result.Add((line.Text, color));
            }
        }
        catch
        {
            // Fall back to the static tooltip if a mod's tooltip hook throws outside a normal hover.
            result.Clear();
            var tip = Lang.GetTooltip(type);
            for (int i = 0; i < tip.Lines; i++)
            {
                string? line = tip.GetLine(i);
                if (!string.IsNullOrWhiteSpace(line))
                    result.Add((line!, Theme.Text));
            }
        }
        return result;
    }

    // ------------------------------------------------------------------ sources

    private Node Sources(int width)
    {
        VStack section = PageKit.Section(L("Items.HowToGet"), ItemID.WorkBench);
        bool any = false;

        IReadOnlyList<Recipe> recipes = GuideCatalog.RecipesFor(_type);
        if (recipes.Count > 0)
        {
            any = true;
            section.Add(new Label(L("Items.Crafting", recipes.Count), Theme.Accent, Theme.Small));
            foreach (Recipe recipe in recipes.Take(8))
                section.Add(RecipeCard(recipe, width));
            if (recipes.Count > 8)
                section.Add(Muted(L("Items.MoreRecipes", recipes.Count - 8)));
            if (TotalsCard(recipes[0]) is Node totals)
            {
                section.Add(new Label(L("Items.TotalMaterials"), Theme.Accent, Theme.Small));
                section.Add(totals);
            }
        }

        IReadOnlyList<ItemSource> sources = GuideCatalog.SourcesFor(_type);

        var drops = sources.Where(s => s.Kind == SourceKind.NpcDrop).OrderByDescending(s => SortNpc(s.Npc)).ThenByDescending(s => s.Chance).ToList();
        if (drops.Count > 0)
        {
            any = true;
            section.Add(new Label(L("Items.DroppedBy", drops.Select(d => d.Npc).Distinct().Count()), Theme.Accent, Theme.Small));
            foreach (ItemSource drop in DedupeByNpc(drops).Take(12))
                section.Add(NpcSourceRow(drop.Npc, $"{Percent(drop.Chance)}{Stack(drop)}", drop.Conditions));
            int more = drops.Select(d => d.Npc).Distinct().Count() - 12;
            if (more > 0)
                section.Add(Muted(L("Items.MoreSources", more)));
        }

        var containers = sources.Where(s => s.Kind == SourceKind.Container).OrderByDescending(s => s.Chance).ToList();
        if (containers.Count > 0)
        {
            any = true;
            section.Add(new Label(L("Items.FromContainers"), Theme.Accent, Theme.Small));
            foreach (ItemSource source in containers.GroupBy(s => s.FromItem).Select(g => g.First()).Take(12))
            {
                var row = new ItemRow(source.FromItem, L("Items.ContainerChance", Percent(source.Chance)) + Stack(source) + ConditionSuffix(source.Conditions));
                section.Add(new Card(row) { Padding = 4, Fill = Theme.SurfaceAlt });
            }
        }

        var shops = sources.Where(s => s.Kind == SourceKind.Shop).ToList();
        if (shops.Count > 0)
        {
            any = true;
            section.Add(new Label(L("Items.SoldBy"), Theme.Accent, Theme.Small));
            foreach (ItemSource shop in shops.GroupBy(s => s.Npc).Select(g => g.First()))
            {
                string price = shop.Currency == -1 ? Coins(shop.Price) : L("Items.SpecialCurrency");
                section.Add(NpcSourceRow(shop.Npc, price, shop.Conditions));
            }
        }

        foreach (ItemSource shimmer in sources.Where(s => s.Kind == SourceKind.Shimmer))
        {
            any = true;
            section.Add(new Label(L("Items.Shimmer"), Theme.Accent, Theme.Small));
            section.Add(new Card(new ItemRow(shimmer.FromItem, L("Items.ShimmerDetail"))) { Padding = 4, Fill = Theme.SurfaceAlt });
        }

        foreach (ItemSource caught in sources.Where(s => s.Kind == SourceKind.Catch).GroupBy(s => s.Npc).Select(g => g.First()))
        {
            any = true;
            section.Add(NpcSourceRow(caught.Npc, L("Items.CatchWithNet"), Array.Empty<string>()));
        }

        var chests = sources.Where(s => s.Kind == SourceKind.Chest).Select(s => s.Chest!).Distinct().ToList();
        if (chests.Count > 0)
        {
            any = true;
            section.Add(new Label(L("Items.Chests"), Theme.Accent, Theme.Small));
            foreach (string chest in chests)
                section.Add(new Paragraph("• " + ChestText(chest), Theme.Text, Theme.Small));
        }

        ItemSource? fishing = sources.FirstOrDefault(s => s.Kind == SourceKind.Fishing);
        if (fishing is not null)
        {
            any = true;
            section.Add(new Label(L("Items.Fishing"), Theme.Accent, Theme.Small));
            foreach (IReadOnlyList<string> where in fishing.Where.DefaultIfEmpty(Array.Empty<string>()))
                section.Add(new Paragraph("• " + WhereText(where), Theme.Text, Theme.Small));
            if (GuideCatalog.SampleItem(_type).questItem)
                section.Add(new Paragraph(L("Items.Hint.AnglerFish"), Theme.TextMuted, Theme.Small));
        }

        if (sources.Any(s => s.Kind == SourceKind.Angler))
        {
            any = true;
            section.Add(Callout(L("Items.AnglerReward"), Theme.Info, L("Items.AnglerTitle")));
        }

        foreach (ItemSource banner in sources.Where(s => s.Kind == SourceKind.Banner).Take(1))
        {
            any = true;
            section.Add(NpcSourceRow(banner.Npc, L("Items.BannerKills", banner.Min), Array.Empty<string>()));
        }

        var global = sources.Where(s => s.Kind == SourceKind.GlobalDrop).ToList();
        if (global.Count > 0)
        {
            any = true;
            section.Add(new Label(L("Items.GlobalDrop"), Theme.Accent, Theme.Small));
            foreach (ItemSource g in global.Take(3))
                section.Add(new Paragraph(Percent(g.Chance) + ConditionSuffix(g.Conditions), Theme.TextMuted, Theme.Small));
        }

        string specialKey = "Items.Special." + GuideCatalog.ItemKey(_type).Replace('/', '_');
        if (Has(specialKey))
        {
            any = true;
            section.Add(Callout(L(Variant(specialKey)), Theme.Info, L("Items.SpecialTitle")));
        }

        if (!any && GuideCatalog.SourceHint(_type) is string hint)
        {
            any = true;
            section.Add(Callout(L(hint), Theme.Info, L("Items.SpecialTitle")));
        }

        if (!any)
            section.Add(Callout(L("Items.NoKnownSource"), Theme.TextMuted));
        return section;
    }

    /// <summary>Raw materials and stations of the first recipe, with every craftable part crafted as well.</summary>
    private static Node? TotalsCard(Recipe recipe)
    {
        MaterialTotals totals = MaterialTree.Expand(recipe);
        if (!totals.Expanded)
            return null;
        var stack = new VStack { Spacing = 6 };
        var flow = new Flow { Spacing = 6, RowSpacing = 6 };
        foreach (Material material in totals.Materials)
        {
            var slot = new ItemSlotNode(material.Type, 42, material.Count);
            if (material.Group is RecipeGroup group)
            {
                slot.Tooltip = () => group.GetText() + "\n" + string.Join(", ", group.ValidItems.Take(12).Select(GuideCatalog.ItemName));
                slot.TooltipItem = 0;
                slot.Caption = L("Items.AnyOf");
            }
            flow.Add(slot);
        }
        stack.Add(flow);
        stack.Add(Muted(string.Join(", ", totals.Materials.Select(m => $"{m.Count} {(m.Group is not null ? m.Group.GetText() : GuideCatalog.ItemName(m.Type))}"))));
        if (totals.Stations.Count > 0)
            stack.Add(Muted(L("Items.StationsUsed", string.Join(", ", totals.Stations.Select(GuideCatalog.TileName)))));
        return new Card(stack) { Padding = 10, Fill = Theme.SurfaceAlt };
    }

    private static float SortNpc(int npc)
    {
        NPC? sample = GuideCatalog.SampleNpc(npc);
        return sample is not null && (sample.boss || NPCID.Sets.ShouldBeCountedAsBoss[Math.Max(0, npc)]) ? 1 : 0;
    }

    private static IEnumerable<ItemSource> DedupeByNpc(List<ItemSource> drops)
    {
        var seen = new HashSet<int>();
        foreach (ItemSource drop in drops)
        {
            if (seen.Add(drop.Npc))
                yield return drop;
        }
    }

    private static string Stack(ItemSource source)
    {
        if (source.Min <= 1 && source.Max <= 1)
            return "";
        return source.Min == source.Max ? $"  x{source.Min}" : $"  x{source.Min}-{source.Max}";
    }

    internal static string ChestText(string kind) => Has("Items.Chest." + kind) ? L("Items.Chest." + kind) : kind;

    /// <summary>"Jungle · Caverns · after Skeletron" for a fishing condition set; "anywhere" when it has none.</summary>
    internal static string WhereText(IReadOnlyList<string> where)
    {
        var parts = new List<string>();
        foreach (string tag in where)
        {
            if (tag == "crate")
                continue; // crates are the item itself
            if (tag.StartsWith("after:", StringComparison.Ordinal))
            {
                string id = tag[6..];
                string boss = GuideData.EntriesById.TryGetValue(id, out ProgressionEntry? entry) ? EntryNames.Name(entry.Id) : id;
                parts.Add(L("Items.Where.after", boss));
            }
            else if (Has("Items.Where." + tag))
            {
                parts.Add(L("Items.Where." + tag));
            }
        }
        string text = parts.Count == 0 ? L("Items.Where.anywhere") : string.Join(" · ", parts);
        return char.ToUpperInvariant(text[0]) + text[1..];
    }

    private static string ConditionSuffix(IReadOnlyList<string> conditions) => conditions.Count == 0 ? "" : "  (" + string.Join(", ", conditions) + ")";

    private static Node NpcSourceRow(int npc, string detail, IReadOnlyList<string> conditions)
    {
        var row = new HStack { Spacing = 10 };
        row.AddFixed(new NpcPortrait(npc, 36), 38);
        var text = new VStack { Spacing = 0 };
        text.Add(new Label(Lang.GetNPCNameValue(npc), Theme.Text, Theme.Body));
        string line = detail;
        IReadOnlyList<string> biomes = GuideCatalog.Biomes(npc);
        if (biomes.Count > 0)
            line += "  |  " + string.Join(", ", biomes.Take(3));
        text.Add(new Label(line, Theme.TextMuted, Theme.Small));
        if (conditions.Count > 0)
            text.Add(new Paragraph(string.Join("\n", conditions), Theme.Warning, Theme.Small));
        row.AddFlex(text);
        var card = new Card(row) { Padding = 6, Fill = Theme.SurfaceAlt };
        string? entry = EntryForNpc(npc);
        if (entry is not null)
        {
            card.OnClick = () => Navigator.OpenEntry(entry);
            card.Tooltip = () => L("Items.OpenBossPage");
        }
        return card;
    }

    /// <summary>The progression entry that lists this NPC, if any.</summary>
    internal static string? EntryForNpc(int npc)
    {
        foreach (EntryStatus status in ProgressService.Entries)
        {
            foreach (string key in status.Entry.Npcs)
            {
                if (GuideCatalog.NpcType(key) == npc)
                    return status.Entry.Id;
            }
        }
        return null;
    }

    /// <summary>One recipe: ingredients → result, with stations and conditions.</summary>
    internal static Node RecipeCard(Recipe recipe, int width)
    {
        var stack = new VStack { Spacing = 6 };
        var ingredients = new Flow { Spacing = 6, RowSpacing = 6 };
        foreach (Item ingredient in recipe.requiredItem)
        {
            RecipeGroup? group = GuideCatalog.GroupFor(recipe, ingredient);
            var slot = new ItemSlotNode(ingredient.type, 42, ingredient.stack);
            if (group is not null)
            {
                slot.Tooltip = () => group.GetText() + "\n" + string.Join(", ", group.ValidItems.Take(12).Select(GuideCatalog.ItemName));
                slot.TooltipItem = 0;
                slot.Caption = L("Items.AnyOf");
            }
            ingredients.Add(slot);
        }
        stack.Add(ingredients);
        // Names under the icons, so a recipe can be read without hovering every slot.
        IEnumerable<string> names = recipe.requiredItem.Select(i =>
        {
            RecipeGroup? g = GuideCatalog.GroupFor(recipe, i);
            string name = g is not null ? g.GetText() : GuideCatalog.ItemName(i.type);
            return i.stack > 1 ? $"{i.stack} {name}" : name;
        });
        stack.Add(Muted(string.Join(", ", names)));

        var station = new Flow { Spacing = 6 };
        if (recipe.requiredTile.Count == 0)
        {
            station.Add(new Badge(L("Items.ByHand"), Theme.TextMuted));
        }
        else
        {
            foreach (int tile in recipe.requiredTile)
            {
                int placer = GuideCatalog.TilePlacer(tile);
                if (placer > 0)
                {
                    var button = new Button(GuideCatalog.TileName(tile), () => Navigator.OpenItem(placer), placer) { Scale = Theme.Small };
                    station.Add(button);
                }
                else
                {
                    station.Add(new Badge(GuideCatalog.TileName(tile), Theme.TextMuted));
                }
            }
        }
        foreach (Terraria.Condition condition in recipe.Conditions)
        {
            string text = condition.Description.Value;
            if (!string.IsNullOrWhiteSpace(text))
                station.Add(new Badge(text, Theme.Warning));
        }
        stack.Add(station);
        if (recipe.createItem.stack > 1)
            stack.Add(Muted(L("Items.MakesAmount", recipe.createItem.stack)));
        return new Card(stack) { Padding = 10, Fill = Theme.SurfaceAlt };
    }

    // ------------------------------------------------------------------ uses

    private Node? Uses()
    {
        IReadOnlyList<Recipe> uses = GuideCatalog.RecipesUsing(_type);
        if (uses.Count == 0)
            return null;
        var results = uses.Select(r => r.createItem.type).Distinct().ToList();
        VStack section = PageKit.Section(L("Items.UsedIn"), ItemID.WorkBench, L("Items.UsedInCount", results.Count));
        int limit = _showAllUses ? results.Count : Math.Min(results.Count, 60);
        var flow = new Flow { Spacing = 6, RowSpacing = 6 };
        foreach (int result in results.Take(limit))
            flow.Add(new ItemSlotNode(result, 42));
        section.Add(flow);
        if (results.Count > 60)
        {
            section.Add(new Button(_showAllUses ? L("Items.ShowLess") : L("Items.ShowAll", results.Count), () =>
            {
                _showAllUses = !_showAllUses;
                Dirty = true;
            }));
        }
        return section;
    }

    // ------------------------------------------------------------------ related

    private Node? Related()
    {
        var entries = ProgressService.Entries.Where(s => s.Entry.Summons.Any(k => GuideCatalog.ItemType(k) == _type)).ToList();
        List<LoadoutRow> recommended = LoadoutsPage.RowsContaining(_type);
        if (entries.Count == 0 && recommended.Count == 0)
            return null;

        VStack section = PageKit.Section(L("Items.Related"));
        foreach (EntryStatus status in entries)
        {
            section.Add(new Label(L("Items.SummonsBoss"), Theme.Accent, Theme.Small));
            section.Add(EntryRow(status, showWhy: false));
        }
        if (recommended.Count > 0)
        {
            section.Add(new Label(L("Items.RecommendedIn"), Theme.Accent, Theme.Small));
            var flow = new Flow { Spacing = 6 };
            foreach (IGrouping<string, LoadoutRow> byStage in recommended.GroupBy(r => r.Stage))
            {
                string stage = byStage.Key;
                string classes = string.Join(", ", byStage.Select(r => r.Class).Distinct().Select(LoadoutsPage.ClassLabel));
                flow.Add(new Button($"{L("Stages." + stage + ".Name")} ({classes})", () => Navigator.Go(new LoadoutsPage(stage))) { Scale = Theme.Small });
            }
            section.Add(flow);
        }
        return section;
    }
}
