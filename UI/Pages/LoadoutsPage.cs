using KhuMue.Common;
using KhuMue.Core;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Recommended gear per class and progression stage (Calamity or vanilla class setups).</summary>
internal sealed class LoadoutsPage : Page
{
    private string? _stage;
    private static string? _class;

    public LoadoutsPage(string? stage = null)
    {
        _stage = stage;
    }

    public override string Title => L("Loadouts.Title");
    public override Section Section => Section.Loadouts;
    public override string Key => "Loadouts:" + (_stage ?? "auto");

    internal static LoadoutSet? Set => SupportedMods.CalamityLoaded ? GuideData.CalamityLoadouts : GuideData.VanillaLoadouts;

    private static string ClassId => _class ?? CurrentClass();

    /// <summary>Stage shown: explicit choice, else the stage the world is in.</summary>
    private string StageId
    {
        get
        {
            LoadoutSet? set = Set;
            if (set is null || set.Stages.Count == 0)
                return "";
            if (_stage is not null && set.Stages.Contains(_stage))
                return _stage;
            string? world = ProgressService.Stage?.Id;
            return world is not null && set.Stages.Contains(world) ? world : set.Stages[0];
        }
    }

    internal static string ClassLabel(string cls) => cls switch
    {
        "all" or "all-but-summoner" or "all-but-stealth" or "all-but-summoner-stealth" => L("Classes.all"),
        _ => ClassName(cls == "summoner" ? "summon" : cls),
    };

    public override Node? BuildHeader(int width)
    {
        LoadoutSet? set = Set;
        var header = new VStack { Spacing = 8 };
        var classes = new Flow { Spacing = 8 };
        foreach (string cls in Classes())
        {
            string c = cls;
            classes.Add(new Chip(ClassName(c), ClassId == c, () =>
            {
                _class = c;
                if (Main.LocalPlayer.TryGetModPlayer(out GuidePlayer gp))
                    gp.PreferredClass = c;
                Dirty = true;
                Sounds.Click();
            }, ClassIcon(c)) { Color = Theme.ClassColor(c) });
        }
        header.Add(classes);

        if (set is not null && set.Stages.Count > 0)
        {
            int index = Math.Max(0, set.Stages.IndexOf(StageId));
            var row = new HStack { Spacing = 8 };
            var prev = new Button("<", () => Step(-1)) { Enabled = index > 0 };
            var next = new Button(">", () => Step(1)) { Enabled = index < set.Stages.Count - 1 };
            row.AddFixed(prev, 40);
            string stageName = L("Stages." + StageId + ".Name");
            bool isWorld = ProgressService.Stage?.Id == StageId;
            var label = new Label(stageName + (isWorld ? "  " + L("Loadouts.YourStage") : ""), isWorld ? Theme.Gold : Theme.Text, Theme.Title) { Center = true };
            row.AddFlex(label);
            row.AddFixed(next, 40);
            if (!isWorld && ProgressService.Stage is not null && set.Stages.Contains(ProgressService.Stage.Id))
                row.AddFixed(new Button(L("Loadouts.BackToWorld"), () => { _stage = null; Dirty = true; ScrollToTop = true; Sounds.Click(); }), 170);
            header.Add(row);
        }
        return header;
    }

    private void Step(int delta)
    {
        LoadoutSet? set = Set;
        if (set is null)
            return;
        int index = Math.Clamp(set.Stages.IndexOf(StageId) + delta, 0, set.Stages.Count - 1);
        _stage = set.Stages[index];
        Dirty = true;
        ScrollToTop = true;
        Sounds.Click();
    }

    public override Node Build(int width)
    {
        var page = new VStack { Spacing = 14 };
        LoadoutSet? set = Set;
        if (set is null || set.Rows.Count == 0)
        {
            page.Add(Callout(L("Loadouts.NoData"), Theme.Info));
            return page;
        }

        string stage = StageId;
        string cls = ClassId;
        if (Has("Stages." + stage + ".Hint"))
            page.Add(new Paragraph(L("Stages." + stage + ".Hint"), Theme.TextMuted, Theme.Small));

        List<LoadoutRow> rows = set.Rows.Where(r => r.Stage == stage || (stage == "pre-scal-exo" && r.Stage is "pre-scal" or "pre-exo")).ToList();
        foreach ((string sectionKey, int icon, List<(string SubKey, List<LoadoutRow> Rows)> groups) in Sections(rows, cls))
        {
            if (groups.All(g => g.Rows.Count == 0))
                continue;
            VStack section = PageKit.Section(L("Loadouts.Sections." + sectionKey), icon);
            foreach ((string subKey, List<LoadoutRow> subRows) in groups)
            {
                if (subRows.Count == 0)
                    continue;
                if (subKey.Length > 0)
                    section.Add(new Label(L("Loadouts.Sub." + subKey), Theme.Accent, Theme.Small));
                var flow = new Flow { Spacing = 8, RowSpacing = 8, CellWidth = Math.Max(220, (width - 16) / 3) };
                foreach (LoadoutRow row in subRows)
                    flow.Add(RowNode(row));
                section.Add(flow);
            }
            page.Add(section);
        }

        page.Add(Legend());
        page.Add(Muted(L("Loadouts.Attribution", set.Source, set.SourceVersion, set.Retrieved)));
        return page;
    }

    // ------------------------------------------------------------------ class/type rules
    // Mirrors the Calamity Wiki's Template:ClassSetupsTier queries so the page shows exactly what the wiki shows.

    private static bool Standard(LoadoutRow r, string cls) => r.Class == cls || r.Class == "all" || (cls != "summon" && r.Class == "all-but-summoner");

    private static IEnumerable<(string, int, List<(string, List<LoadoutRow>)>)> Sections(List<LoadoutRow> rows, string cls)
    {
        List<LoadoutRow> Of(Func<LoadoutRow, bool> pick) => rows.Where(pick).ToList();

        var weapons = new List<(string, List<LoadoutRow>)> { ("", Of(r => r.Type == "weapon" && Standard(r, cls))) };
        if (cls == "rogue")
        {
            weapons.Add(("Spam", Of(r => r.Type == "weaponSpam" && Standard(r, cls))));
            weapons.Add(("Stealth", Of(r => r.Type == "weaponStealth" && Standard(r, cls))));
        }
        if (cls == "summon")
        {
            weapons.Add(("Minions", Of(r => r.Type == "minions" && (r.Class == cls || r.Class == "all"))));
            weapons.Add(("Sentries", Of(r => r.Type == "sentries" && Standard(r, cls))));
            weapons.Add(("Held", Of(r => r.Type == "weaponSummon" && Standard(r, cls))));
        }
        weapons.Add(("Support", Of(r => r.Type == "support" && Standard(r, cls))));
        yield return ("Weapons", ClassIcon(cls), weapons);

        yield return ("Armor", ItemID.IronChainmail, new List<(string, List<LoadoutRow>)>
        {
            ("ClassArmor", Of(r => r.Type == "armor" && r.Class == cls)),
            ("AllArmor", Of(r => r.Type == "armor" && r.Class == "all")),
            ("PrimaryMobility", Of(r => r.Type == "accessoryMobilityPrimary" && (r.Class == cls || r.Class == "all"))),
        });

        var accessories = new List<(string, List<LoadoutRow>)>
        {
            ("", Of(r => r.Type == "accessory" && Standard(r, cls))),
            ("General", Of(r => r.Type is "accessoryGeneral" or "accessorGeneral" && Standard(r, cls))),
            ("Mobility", Of(r => r.Type == "accessoryMobility" && Standard(r, cls))),
        };
        if (cls == "rogue")
        {
            accessories.Add(("Spam", Of(r => (r.Type == "accessorySpam" && Standard(r, cls))
                                           || (r.Type == "accessoryOffense" && r.Class is "all" or "all-but-summoner" or "all-but-stealth" or "all-but-summoner-stealth"))));
            accessories.Add(("Stealth", Of(r => (r.Type == "accessoryStealth" && Standard(r, cls))
                                              || (r.Type == "accessoryOffense" && r.Class is "all" or "all-but-summoner"))));
        }
        else
        {
            accessories.Add(("Offense", Of(r => r.Type == "accessoryOffense"
                                              && (Standard(r, cls) || r.Class == "all-but-stealth" || (cls != "summon" && r.Class == "all-but-summoner-stealth")))));
        }
        accessories.Add(("Defense", Of(r => r.Type == "accessoryDefense" && Standard(r, cls))));
        yield return ("Accessories", ItemID.HermesBoots, accessories);

        yield return ("Buffs", ItemID.IronskinPotion, new List<(string, List<LoadoutRow>)>
        {
            ("", Of(r => r.Type == "buff" && Standard(r, cls))),
            ("General", Of(r => r.Type == "buffGeneral" && Standard(r, cls))),
            ("Defense", Of(r => r.Type == "buffDefense" && Standard(r, cls))),
            ("Offense", Of(r => r.Type == "buffOffense" && Standard(r, cls))),
            ("Mobility", Of(r => r.Type == "buffMobility" && Standard(r, cls))),
        });

        yield return ("Ammo", ItemID.MusketBall, new List<(string, List<LoadoutRow>)>
        {
            ("", Of(r => r.Type == "ammo" && Standard(r, cls))),
        });
    }

    // ------------------------------------------------------------------ rows

    private static Node RowNode(LoadoutRow row)
    {
        var stack = new VStack { Spacing = 4 };
        for (int i = 0; i < row.Items.Count; i++)
        {
            List<string> alternative = row.Items[i];
            var types = alternative.Select(GuideCatalog.ItemType).Where(t => t > 0).ToList();
            if (types.Count == 0)
                continue;
            if (types.Count == 1)
            {
                stack.Add(new ItemRow(types[0]) { Compact = true });
                string? warning = ItemWarning(types[0]);
                if (warning is not null)
                    stack.Add(new Paragraph(warning, Theme.Warning, Theme.Small));
            }
            else
            {
                // Armor set: the set name, then its pieces as slots.
                string? label = row.Labels is { } labels && i < labels.Count ? labels[i] : null;
                if (!string.IsNullOrEmpty(label))
                    stack.Add(new Label(label, RichText.ItemColor(types[0]), Theme.Body));
                var line = new Flow { Spacing = 4 };
                foreach (int t in types.Take(6))
                    line.Add(new ItemSlotNode(t, 32));
                stack.Add(line);
            }
            if (i < row.Items.Count - 1)
                stack.Add(new Label(L("Loadouts.Or"), Theme.TextFaint, Theme.Small));
        }

        var extras = new Flow { Spacing = 4 };
        foreach (string flag in FlagCodes(row.Flags))
            extras.Add(new Badge(L("Loadouts.Flags." + flag), FlagColor(flag)) { Tooltip = () => L("Loadouts.FlagHelp." + flag) });
        if (extras.Children.Count > 0)
            stack.Add(extras);
        if (row.Note is not null && Has("LoadoutNotes." + row.Note))
            stack.Add(new Paragraph(L("LoadoutNotes." + row.Note), Theme.TextMuted, Theme.Small));

        return new Card(stack) { Padding = 8, Fill = Theme.SurfaceAlt };
    }

    /// <summary>
    /// Wiki symbols to flag codes. Calamity wiki: † Hard, C Crowd, + Support, ≤ Upgrades, Δ Changed, * Rare, Ω1/Ω2 Combo.
    /// Terraria wiki: B = listed in a "Best" box, C = listed under crowd control weapons.
    /// </summary>
    internal static IEnumerable<string> FlagCodes(string symbols)
    {
        if (string.IsNullOrEmpty(symbols))
            yield break;
        if (symbols.Contains('B')) yield return "Best";
        if (symbols.Contains('†')) yield return "Hard";
        if (symbols.Contains('C')) yield return "Crowd";
        if (symbols.Contains('+')) yield return "Support";
        if (symbols.Contains('≤')) yield return "Upgrades";
        if (symbols.Contains('Δ')) yield return "Changed";
        if (symbols.Contains('*')) yield return "Rare";
        if (symbols.Contains("Ω1")) yield return "Combo1";
        else if (symbols.Contains("Ω2")) yield return "Combo2";
        else if (symbols.Contains('Ω')) yield return "Combo1";
    }

    private static Color FlagColor(string flag) => flag switch
    {
        "Best" => Theme.Gold,
        "Hard" => Theme.Danger,
        "Crowd" => Theme.Info,
        "Support" => Theme.Success,
        "Upgrades" => Theme.Accent,
        "Changed" => new Color(206, 132, 227),
        "Rare" => Theme.Warning,
        _ => Theme.Gold,
    };

    private static Node Legend()
    {
        var card = new VStack { Spacing = 4 };
        card.Add(new Label(L("Loadouts.LegendTitle"), Theme.Text, Theme.Small));
        var flow = new Flow { Spacing = 6 };
        bool vanilla = Set == GuideData.VanillaLoadouts;
        string[] flags = vanilla ? new[] { "Best", "Crowd" } : new[] { "Hard", "Crowd", "Support", "Upgrades", "Changed", "Rare", "Combo1" };
        foreach (string flag in flags)
            flow.Add(new Badge(L("Loadouts.Flags." + flag), FlagColor(flag)) { Tooltip = () => L("Loadouts.FlagHelp." + flag) });
        card.Add(flow);
        card.Add(Muted(L(vanilla ? "Loadouts.LegendNoteVanilla" : "Loadouts.LegendNote")));
        return new Card(card) { Padding = 10, Fill = Theme.Slot };
    }

    /// <summary>Loadout rows that recommend an item (for the item page).</summary>
    internal static List<LoadoutRow> RowsContaining(int type)
    {
        LoadoutSet? set = Set;
        if (set is null || type <= 0)
            return new List<LoadoutRow>();
        string key = GuideCatalog.ItemKey(type);
        return set.Rows.Where(r => r.Items.Any(a => a.Contains(key))).ToList();
    }
}
