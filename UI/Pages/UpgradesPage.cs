using KhuMue.Core;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Permanent character upgrades and which ones this character has used.</summary>
internal sealed class UpgradesPage : Page
{
    private static readonly string[] Groups = { "life", "mana", "slot", "stats", "qol", "rage", "adrenaline", "souls", "world" };

    public override string Title => L("Upgrades.Title");
    public override Section Section => Section.Upgrades;

    public override void Tick()
    {
        if (Main.GameUpdateCount % 90 == 0)
            Dirty = true;
    }

    public override Node Build(int width)
    {
        var page = new VStack { Spacing = 14 };
        page.Add(Muted(L("Upgrades.Intro")));

        var shown = GuideData.Upgrades
            .Where(d => (d.Mod == "Terraria" || ModLoader.HasMod(d.Mod)) && GuideCatalog.ItemType(d.Item) > 0)
            .Where(d => GuideCondition.Parse(d.Show).Evaluate() != false)
            .ToList();
        var possible = shown.Where(d => GuideCondition.Parse(d.Requires).Evaluate() != false).ToList();
        var impossible = shown.Except(possible).ToList();

        int used = 0;
        foreach (UpgradeDefinition def in possible)
        {
            if (UpgradeService.Read(def, Main.LocalPlayer).Complete)
                used++;
        }
        page.Add(new ProgressBar { Value = possible.Count == 0 ? 0 : used / (float)possible.Count, Caption = L("Upgrades.Summary", used, possible.Count), Color = Theme.Success });

        // Things that can be used right now but have not been.
        var ready = possible.Where(d => !UpgradeService.Read(d, Main.LocalPlayer).Complete && GuideCondition.Parse(d.Available).Evaluate() == true).ToList();
        if (ready.Count > 0)
        {
            var chips = new Flow { Spacing = 6, RowSpacing = 6 };
            foreach (UpgradeDefinition def in ready)
            {
                int type = GuideCatalog.ItemType(def.Item);
                chips.Add(new ItemSlotNode(type, 36) { ShowOwned = true });
            }
            VStack section = PageKit.Section(L("Upgrades.ReadyTitle"), ItemID.LifeCrystal, L("Upgrades.ReadyHint"));
            section.Add(chips);
            page.Add(section);
        }

        foreach (string group in Groups)
        {
            var items = possible.Where(d => d.Group == group).ToList();
            if (items.Count == 0)
                continue;
            VStack section = PageKit.Section(L("Upgrades.Groups." + group));
            if (Has("Upgrades.GroupNotes." + group))
                section.Add(Muted(L("Upgrades.GroupNotes." + group)));
            foreach (UpgradeDefinition def in items)
                section.Add(UpgradeRow(def, possibleHere: true));
            page.Add(section);
        }

        if (impossible.Count > 0)
        {
            VStack section = PageKit.Section(L("Upgrades.NotInWorld"));
            section.Add(Muted(L("Upgrades.NotInWorldHint")));
            foreach (UpgradeDefinition def in impossible)
                section.Add(UpgradeRow(def, possibleHere: false));
            page.Add(section);
        }
        return page;
    }

    private static Node UpgradeRow(UpgradeDefinition def, bool possibleHere)
    {
        int type = GuideCatalog.ItemType(def.Item);
        UpgradeState state = UpgradeService.Read(def, Main.LocalPlayer);
        var row = new HStack { Spacing = 10 };
        row.AddFixed(new ItemSlotNode(type, 40) { ShowOwned = true }, 42);
        var text = new VStack { Spacing = 2 };
        text.Add(new Label(GuideCatalog.ItemName(type), RichText.ItemColor(type), Theme.Body));
        text.Add(new Paragraph(L(Variant("Upgrades.Items." + def.Id + ".Effect")), Theme.Text, Theme.Small));
        text.Add(new Paragraph(L(Variant("Upgrades.Items." + def.Id + ".Where")), Theme.TextMuted, Theme.Small));
        row.AddFlex(text);

        string status;
        Color color;
        if (!possibleHere)
        {
            status = L("Upgrades.NotPossible");
            color = Theme.TextFaint;
        }
        else if (state.Used is null)
        {
            status = L("Upgrades.Unknown");
            color = Theme.Warning;
        }
        else if (state.Complete)
        {
            status = def.Max > 1 ? L("Upgrades.UsedCount", state.Used, def.Max) : L("Upgrades.Used");
            color = Theme.Success;
        }
        else
        {
            status = def.Max > 1 ? L("Upgrades.UsedCount", state.Used, def.Max) : L("Upgrades.NotUsed");
            color = GuideCondition.Parse(def.Available).Evaluate() == true ? Theme.Gold : Theme.TextFaint;
        }
        row.AddFixed(new Badge(status, color, solid: possibleHere && state.Complete), 130);
        return new Card(row)
        {
            Padding = 8,
            Fill = possibleHere && state.Complete ? new Color(26, 44, 34) : Theme.Surface,
            OnClick = () => Navigator.OpenItem(type),
            Tooltip = () => L("Upgrades.ClickForSources"),
        };
    }
}
