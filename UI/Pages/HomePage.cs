using KhuMue.Common;
using KhuMue.Core;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Start screen: where the world is, what to do next, what else is open, what to prepare.</summary>
internal sealed class HomePage : Page
{
    private string _stateSignature = "";

    public override string Title => L("Home.Title");
    public override Section Section => Section.Home;

    public override void Tick()
    {
        // Rebuild when the world progresses while the guide is open.
        if (Main.GameUpdateCount % 60 == 0)
        {
            string signature = Signature();
            if (signature != _stateSignature)
                Dirty = true;
        }
    }

    private static string Signature()
    {
        EntryStatus? next = ProgressService.Next;
        // The difficulty badges too: Calamity switches Revengeance to Death by itself in Master worlds.
        return (next?.Entry.Id ?? "-") + "|" + ProgressService.StageIndex + "|" + ProgressService.Entries.Count(e => e.Done == true)
            + "|" + Main.GameMode + "|" + Flags.Get("cal.Revengeance") + "|" + Flags.Get("cal.Death") + "|" + Flags.Get("inf.Active");
    }

    public override Node Build(int width)
    {
        ProgressService.Refresh(force: true);
        _stateSignature = Signature();
        var page = new VStack { Spacing = 14 };

        // tModLoader leaves new mod keys unbound: say how to open the guide until a key is set.
        if (KhuMue.AssignedKeys().Count == 0)
            page.Add(Callout(L("Settings.KeyNone"), Theme.Warning));

        page.Add(WorldCard());

        EntryStatus? next = ProgressService.Next;
        if (next is not null)
            page.Add(NextCard(next, width));
        else
            page.Add(Callout(L("Home.AllDone"), Theme.Success, L("Home.AllDoneTitle")));

        List<EntryStatus> open = ProgressService.AvailableNow(8);
        if (open.Count > 0)
        {
            VStack section = PageKit.Section(L("Home.AlsoOpen"), ItemID.Compass, L("Home.AlsoOpenHint"));
            foreach (EntryStatus status in open)
                section.Add(EntryRow(status));
            page.Add(section);
        }

        Node? upgrades = UpgradeReminder();
        if (upgrades is not null)
            page.Add(upgrades);

        Node? notice = VersionNotice();
        if (notice is not null)
            page.Add(notice);

        page.Add(Muted(L("Home.Footer")));
        return page;
    }

    private static Node WorldCard()
    {
        var stack = new VStack { Spacing = 6 };
        var top = new Flow { Spacing = 8 };
        top.Add(new Label(Main.worldName, Theme.Text, Theme.Title));
        top.Add(new Badge(DifficultyName(), Theme.Info));
        if (SupportedMods.CalamityLoaded)
        {
            if (Flags.Get("cal.Death") == true)
                top.Add(new Badge(L("Home.Death"), Theme.Danger, solid: true));
            else if (Flags.Get("cal.Revengeance") == true)
                top.Add(new Badge(L("Home.Revengeance"), Theme.Warning, solid: true));
        }
        if (Flags.Get("inf.Active") == true)
            top.Add(new Badge("Infernum", new Color(255, 120, 40), solid: true));
        if (Main.maxTilesX <= 4200)
            top.Add(new Badge(L("Home.SmallWorld"), Theme.TextMuted));
        stack.Add(top);

        StageDefinition? stage = ProgressService.Stage;
        if (stage is not null)
        {
            string stageName = L("Stages." + stage.Id + ".Name");
            stack.Add(new Paragraph(L("Home.StageLine", stageName), Theme.Era(stage.Era), Theme.Body));
            string hint = L("Stages." + stage.Id + ".Hint");
            if (Has("Stages." + stage.Id + ".Hint"))
                stack.Add(new Paragraph(hint, Theme.TextMuted, Theme.Small));
        }

        (int done, int total) = ProgressService.MainProgress();
        if (total > 0)
        {
            stack.Add(new ProgressBar
            {
                Value = done / (float)total,
                Caption = L("Home.RouteProgress", done, total),
                Color = stage is null ? Theme.Accent : Theme.Era(stage.Era),
            });
        }
        return new Card(stack) { Padding = 14 };
    }

    private static string DifficultyName()
    {
        if (Main.GameModeInfo.IsJourneyMode)
            return L("Home.Journey");
        if (Main.masterMode)
            return L("Home.Master");
        if (Main.expertMode)
            return L("Home.Expert");
        return L("Home.Classic");
    }

    private static Node NextCard(EntryStatus next, int width)
    {
        ProgressionEntry entry = next.Entry;
        var row = new HStack { Spacing = 16, CenterY = false };
        int portrait = width < 700 ? 88 : 112;
        row.AddFixed(Portrait(entry, portrait), portrait);

        var info = new VStack { Spacing = 6 };
        info.Add(new Label(L("Home.NextLabel"), Theme.Gold, Theme.Small));
        var title = new Flow { Spacing = 8 };
        title.Add(new Label(EntryNames.Name(entry.Id), Theme.Text, Theme.Hero));
        if (entry.Mod != "Terraria")
            title.Add(ModBadge(entry.Mod));
        title.Add(new Badge(EraName(entry.Era), Theme.Era(entry.Era)));
        if (next.State == EntryState.Locked)
            title.Add(new Badge(StateName(EntryState.Locked), Theme.TextFaint));
        info.Add(title);

        string summary = EntryNames.Text(entry, "Summary");
        if (summary.Length > 0)
            info.Add(new Paragraph(summary, Theme.TextMuted, Theme.Body));

        string summon = EntryNames.Text(entry, "Summon");
        if (summon.Length > 0)
        {
            info.Add(new Label(L(entry.Kind == "event" ? "Entry.HowToStartEvent" : "Entry.HowToStart"), Theme.Accent, Theme.Small));
            info.Add(new Paragraph(summon, Theme.Text, Theme.Body));
        }

        if (entry.Summons.Count > 0)
        {
            var summons = new Flow { Spacing = 6 };
            foreach (string key in entry.Summons)
            {
                int type = GuideCatalog.ItemType(key);
                if (type > 0)
                    summons.Add(new ItemSlotNode(type, 44));
            }
            info.Add(summons);
        }

        var buttons = new Flow { Spacing = 8 };
        buttons.Add(new Button(L("Home.OpenEntry"), () => Navigator.OpenEntry(entry.Id)) { Primary = true });
        buttons.Add(new Button(L("Home.OpenLoadout"), () => Navigator.Go(new LoadoutsPage())) { Icon = ClassIcon(CurrentClass()) });
        info.Add(buttons);
        row.AddFlex(info);

        return new Card(row)
        {
            Padding = 16,
            Fill = new Color(40, 36, 24),
            BorderColor = Theme.Gold * 0.7f,
            Accent = Theme.Gold,
        };
    }

    private static Node? UpgradeReminder()
    {
        if (Main.gameMenu)
            return null;
        var missing = new List<(UpgradeDefinition Def, UpgradeState State)>();
        foreach (UpgradeDefinition def in GuideData.Upgrades)
        {
            if (def.Mod != "Terraria" && !ModLoader.HasMod(def.Mod))
                continue;
            if (GuideCondition.Parse(def.Show).Evaluate() == false || GuideCondition.Parse(def.Requires).Evaluate() == false)
                continue;
            if (GuideCondition.Parse(def.Available).Evaluate() != true)
                continue;
            UpgradeState state = UpgradeService.Read(def, Main.LocalPlayer);
            if (state.Used is int used && used < def.Max)
                missing.Add((def, state));
        }
        if (missing.Count == 0)
            return null;

        VStack section = PageKit.Section(L("Home.Upgrades"), ItemID.LifeCrystal, L("Home.UpgradesHint"));
        var flow = new Flow { Spacing = 8 };
        foreach ((UpgradeDefinition def, UpgradeState state) in missing.Take(10))
        {
            int type = GuideCatalog.ItemType(def.Item);
            if (type <= 0)
                continue;
            var slot = new ItemSlotNode(type, 44) { Caption = def.Max > 1 ? $"{state.Used}/{def.Max}" : null };
            flow.Add(slot);
        }
        section.Add(flow);
        section.Add(new Button(L("Home.OpenUpgrades"), () => Navigator.OpenSection(Section.Upgrades)) { Icon = ItemID.LifeCrystal });
        return section;
    }

    private static Node? VersionNotice()
    {
        if (GuideConfig.Instance?.ShowVersionNotice == false)
            return null;
        var different = SupportedMods.All.Where(m => m.Match == VersionMatch.Different || m.Match == VersionMatch.Near).ToList();
        if (different.Count == 0)
            return null;
        string list = string.Join(", ", different.Select(m => $"{SupportedMods.DisplayName(m.Name)} {m.LoadedVersion} ({L("Home.Checked")} {m.VerifiedVersion})"));
        return Callout(L("Home.VersionNotice", list), Theme.Warning, L("Home.VersionNoticeTitle"));
    }
}
