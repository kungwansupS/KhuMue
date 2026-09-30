using KhuMue.Core;
using Terraria.GameContent.ItemDropRules;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Boss / event / quest detail.</summary>
internal sealed class EntryPage(string id) : Page
{
    private readonly string _id = id;

    public override string Title => EntryNames.Name(_id);
    public override Section Section => Section.Progression;
    public override string Key => "Entry:" + _id;

    public override Node Build(int width)
    {
        EntryStatus? status = ProgressService.Get(_id);
        if (!GuideData.EntriesById.TryGetValue(_id, out ProgressionEntry? entry))
            return new Paragraph(L("Entry.Missing"), Theme.Danger);
        status ??= new EntryStatus(entry) { State = EntryState.Unknown };

        var page = new VStack { Spacing = 14 };
        page.Add(Header(entry, status, width));

        string warning = EntryNames.Text(entry, "Warning");
        if (warning.Length > 0)
            page.Add(Callout(warning, Theme.Warning, L("Entry.Warning")));

        if (Main.netMode == NetmodeID.MultiplayerClient && UnsyncedInMultiplayer.TryGetValue(entry.Id, out string? fixedBy)
            && (fixedBy is null || !ModLoader.HasMod(fixedBy)))
            page.Add(Callout(L("Entry.MultiplayerNote"), Theme.Warning));

        string summon = EntryNames.Text(entry, "Summon");
        if (summon.Length > 0 || entry.Summons.Count > 0)
        {
            VStack section = PageKit.Section(entry.Kind == "event" ? L("Entry.HowToStartEvent") : L("Entry.HowToStart"), ItemID.SuspiciousLookingEye);
            if (summon.Length > 0)
                section.Add(new Paragraph(summon));
            foreach (string key in entry.Summons)
            {
                int type = GuideCatalog.ItemType(key);
                if (type > 0)
                    section.Add(SummonRow(type));
            }
            page.Add(section);
        }

        string unlocks = EntryNames.Text(entry, "Unlocks");
        if (unlocks.Length > 0)
        {
            VStack section = PageKit.Section(L("Entry.Unlocks"), ItemID.GoldenKey);
            section.Add(new Paragraph(unlocks));
            page.Add(section);
        }

        string tips = EntryNames.Text(entry, "Tips");
        if (tips.Length > 0)
        {
            VStack section = PageKit.Section(L("Entry.Tips"), ItemID.Book);
            section.Add(new Paragraph(tips));
            page.Add(section);
        }

        // Sub-entries (event minibosses) and parent event.
        var children = ProgressService.Entries.Where(s => s.Entry.Parent == _id).ToList();
        if (children.Count > 0)
        {
            VStack section = PageKit.Section(L("Entry.Minibosses"));
            foreach (EntryStatus child in children)
                section.Add(EntryRow(child));
            page.Add(section);
        }
        if (entry.Parent is not null && ProgressService.Get(entry.Parent) is EntryStatus parent)
        {
            VStack section = PageKit.Section(L("Entry.PartOf"));
            section.Add(EntryRow(parent));
            page.Add(section);
        }

        Node? drops = Drops(entry, width);
        if (drops is not null)
            page.Add(drops);

        Node? stats = Stats(entry);
        if (stats is not null)
            page.Add(stats);

        page.Add(Muted(L("Entry.Source", SupportedMods.DisplayName(entry.Mod))));
        return page;
    }

    private static Node Header(ProgressionEntry entry, EntryStatus status, int width)
    {
        var row = new HStack { Spacing = 16, CenterY = false };
        int size = width < 700 ? 96 : 128;
        row.AddFixed(Portrait(entry, size), size);
        var info = new VStack { Spacing = 6 };
        var badges = new Flow { Spacing = 6 };
        badges.Add(StateBadge(status));
        badges.Add(new Badge(EraName(entry.Era), Theme.Era(entry.Era)));
        badges.Add(new Badge(KindName(entry.Kind), Theme.Info));
        badges.Add(ModBadge(entry.Mod));
        badges.Add(entry.Gate ? new Badge(L("Badges.Gate"), Theme.Danger) : entry.IsMain ? new Badge(L("Badges.Main"), Theme.Gold) : new Badge(L("Badges.Optional"), Theme.TextFaint));
        info.Add(badges);
        string summary = EntryNames.Text(entry, "Summary");
        if (summary.Length > 0)
            info.Add(new Paragraph(summary, Theme.Text, Theme.Body));
        if (status.State == EntryState.Locked)
        {
            string req = EntryNames.Text(entry, "Requires");
            info.Add(new Paragraph(req.Length > 0 ? req : L("Entry.LockedGeneric"), Theme.Warning, Theme.Small));
        }
        if (status.State == EntryState.Unknown)
            info.Add(new Paragraph(L("Entry.UnknownState"), Theme.Warning, Theme.Small));

        // Link to the loadout for the stage that prepares for this entry.
        StageDefinition? prep = ProgressService.Ladder.FirstOrDefault(s => s.Targets.Contains(entry.Id));
        var buttons = new Flow { Spacing = 8 };
        if (prep is not null)
        {
            string stageId = prep.Id;
            buttons.Add(new Button(L("Entry.PrepareGear"), () => Navigator.Go(new LoadoutsPage(stageId))) { Icon = ClassIcon(CurrentClass()), Primary = true });
        }
        if (buttons.Children.Count > 0)
            info.Add(buttons);
        row.AddFlex(info);
        return new Card(row) { Padding = 14, Accent = Theme.Era(entry.Era) };
    }

    /// <summary>Summon item with a one-line "how to get it".</summary>
    private static Node SummonRow(int type)
    {
        var row = new ItemRow(type, HowToGet(type));
        return new Card(row) { Padding = 4, Fill = Theme.SurfaceAlt };
    }

    /// <summary>Short sentence describing the main way to obtain an item (live data).</summary>
    internal static string HowToGet(int type)
    {
        IReadOnlyList<Recipe> recipes = GuideCatalog.RecipesFor(type);
        if (recipes.Count > 0)
        {
            Recipe recipe = recipes[0];
            string station = recipe.requiredTile.Count == 0 ? L("Items.ByHand") : string.Join(" + ", recipe.requiredTile.Select(GuideCatalog.TileName));
            string ingredients = string.Join(", ", recipe.requiredItem.Take(4).Select(i => $"{i.stack} {GuideCatalog.ItemName(i.type)}"));
            return L("Items.CraftShort", ingredients, station);
        }
        IReadOnlyList<ItemSource> sources = GuideCatalog.SourcesFor(type);
        ItemSource? shop = sources.FirstOrDefault(s => s.Kind == SourceKind.Shop);
        if (shop is not null)
            return L("Items.ShopShort", Lang.GetNPCNameValue(shop.Npc));
        ItemSource? drop = sources.Where(s => s.Kind == SourceKind.NpcDrop).OrderByDescending(s => s.Chance).FirstOrDefault();
        if (drop is not null)
            return L("Items.DropShort", Lang.GetNPCNameValue(drop.Npc), Percent(drop.Chance));
        ItemSource? chest = sources.FirstOrDefault(s => s.Kind == SourceKind.Chest);
        if (chest is not null)
            return L("Items.ChestShort", ItemPage.ChestText(chest.Chest!));
        ItemSource? fishing = sources.FirstOrDefault(s => s.Kind == SourceKind.Fishing);
        if (fishing is not null)
            return L("Items.FishingShort", ItemPage.WhereText(fishing.Where.FirstOrDefault() ?? Array.Empty<string>()));
        if (sources.Any(s => s.Kind == SourceKind.Angler))
            return L("Items.AnglerShort");
        string note = "Items.Special." + GuideCatalog.ItemKey(type).Replace('/', '_');
        if (Has(note))
            return L(note);
        return L("Items.OpenForSources");
    }

    private static Node? Drops(ProgressionEntry entry, int width)
    {
        var npcs = entry.Npcs.Select(GuideCatalog.NpcType).Where(t => t != 0).Distinct().ToList();
        if (npcs.Count == 0)
            return null;

        var bags = new List<int>();
        var direct = new Dictionary<int, float>();
        foreach (int npc in npcs)
        {
            foreach (DropRateInfo drop in GuideCatalog.NpcLoot(npc))
            {
                if (drop.itemId <= 0 || !InThisDifficulty(drop))
                    continue;
                if (ItemID.Sets.BossBag[drop.itemId])
                {
                    if (!bags.Contains(drop.itemId))
                        bags.Add(drop.itemId);
                    continue;
                }
                direct[drop.itemId] = Math.Max(direct.TryGetValue(drop.itemId, out float c) ? c : 0f, drop.dropRate);
            }
        }

        var bagContents = new Dictionary<int, float>();
        foreach (int bag in bags)
        {
            foreach (DropRateInfo drop in GuideCatalog.ContainerContents(bag))
            {
                if (drop.itemId > 0 && InThisDifficulty(drop))
                    bagContents[drop.itemId] = Math.Max(bagContents.TryGetValue(drop.itemId, out float c) ? c : 0f, drop.dropRate);
            }
        }

        if (direct.Count == 0 && bagContents.Count == 0)
            return null;

        VStack section = PageKit.Section(L("Entry.Drops"), ItemID.GoldCoin, L("Entry.DropsHint"));
        if (bags.Count > 0)
        {
            var bagLine = new Flow { Spacing = 6 };
            bagLine.Add(new Label(L("Entry.TreasureBag"), Theme.TextMuted, Theme.Small));
            foreach (int bag in bags)
                bagLine.Add(new ItemSlotNode(bag, 36));
            section.Add(bagLine);
        }
        if (bagContents.Count > 0)
        {
            section.Add(new Label(L("Entry.FromBag"), Theme.Accent, Theme.Small));
            section.Add(DropGrid(bagContents));
        }
        if (direct.Count > 0)
        {
            section.Add(new Label(bags.Count > 0 ? L("Entry.DirectDrops") : L("Entry.Drops"), Theme.Accent, Theme.Small));
            section.Add(DropGrid(direct));
        }
        return section;
    }

    /// <summary>
    /// False for drops this world's difficulty rules out: Classic-only drops (the Treasure Bag replaces them)
    /// in Expert and Master, Expert- or Master-only drop rates in Classic.
    /// </summary>
    private static bool InThisDifficulty(DropRateInfo drop)
    {
        if (drop.conditions is null)
            return true;
        foreach (IItemDropRuleCondition condition in drop.conditions)
        {
            if (condition is Conditions.NotExpert && Main.expertMode
                || condition is Conditions.IsExpert && !Main.expertMode
                || condition is Conditions.IsMasterMode && !Main.masterMode
                || condition is Conditions.NotMasterMode && Main.masterMode)
                return false;
        }
        return true;
    }

    private static Node DropGrid(Dictionary<int, float> drops)
    {
        var flow = new Flow { Spacing = 8, RowSpacing = 10 };
        foreach ((int item, float chance) in drops.OrderByDescending(d => GuideCatalog.SampleItem(d.Key).rare).ThenBy(d => d.Key))
        {
            var slot = new ItemSlotNode(item, 42) { Caption = Percent(chance) };
            flow.Add(slot);
        }
        return flow;
    }

    /// <summary>
    /// Entries whose "done" state the owning mod does not send to multiplayer clients (checked in the sources of the
    /// versions in SupportedMods), with the mod that fixes it, if any: a client that joined later sees them as not done.
    /// </summary>
    private static readonly Dictionary<string, string?> UnsyncedInMultiplayer = new()
    {
        // InfernumMode WorldSaveSystem.DownedBereftVassal is saved but never sent (no NetSend).
        ["BereftVassal"] = null,
        // Wrath of the Gods sends Solyn quest stages only when they change; WotGMultiplayerFix replicates them.
        ["SolynMeet"] = "WotGMultiplayerFix",
        ["SolynCamp"] = "WotGMultiplayerFix",
        ["SolynStargaze"] = "WotGMultiplayerFix",
        ["PermafrostKeep"] = "WotGMultiplayerFix",
        ["GenesisReveal"] = "WotGMultiplayerFix",
        ["Antiseed"] = "WotGMultiplayerFix",
    };

    private static Node? Stats(ProgressionEntry entry)
    {
        if (entry.Kind == "event" || entry.Kind == "quest" || entry.Npcs.Count == 0)
            return null;
        // Multi-part bosses: the part with the most life is the one that matters (Moon Lord's core, not its head).
        NPC? npc = entry.Npcs.Select(GuideCatalog.NpcType).Where(t => t != 0).Select(GuideCatalog.SampleNpc)
            .Where(n => n is not null).OrderByDescending(n => n!.lifeMax).FirstOrDefault();
        if (npc is null || npc.lifeMax <= 5)
            return null;
        VStack section = PageKit.Section(L("Entry.BaseStats"));
        var flow = new Flow { Spacing = 8 };
        flow.Add(new Badge(L("Entry.Life", npc.lifeMax.ToString("N0")), Theme.Danger));
        flow.Add(new Badge(L("Entry.Defense", npc.defense), Theme.Info));
        if (npc.damage > 0)
            flow.Add(new Badge(L("Entry.Damage", npc.damage), Theme.Warning));
        section.Add(flow);
        section.Add(Muted(L("Entry.BaseStatsNote")));
        return section;
    }
}
