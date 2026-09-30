using KhuMue.Common;
using KhuMue.Core;

namespace KhuMue.UI.Pages;

/// <summary>Shared building blocks for guide pages.</summary>
internal static class PageKit
{
    /// <summary>All guide texts live under this root so they never collide with older translation packs.</summary>
    internal const string Root = "Mods.KhuMue.Guide.";

    internal static string L(string key) => Language.GetTextValue(Root + key);

    internal static string L(string key, params object[] args) => Language.GetTextValue(Root + key, args);

    internal static bool Has(string key) => Language.Exists(Root + key);

    /// <summary>The key's "Cal" variant while Calamity is loaded and one exists (texts that Calamity changes), else the key.</summary>
    internal static string Variant(string key) => SupportedMods.CalamityLoaded && Has(key + "Cal") ? key + "Cal" : key;

    internal static string EraName(string era) => L("Eras." + era);

    internal static string KindName(string kind) => L("Kinds." + kind);

    internal static Color StateColor(EntryState state) => state switch
    {
        EntryState.Done => Theme.Success,
        EntryState.Next => Theme.Gold,
        EntryState.Available => Theme.Accent,
        EntryState.Locked => Theme.TextFaint,
        _ => Theme.Warning,
    };

    internal static string StateName(EntryState state) => L("States." + state);

    /// <summary>With spoilers hidden, entries after the next step show "???".</summary>
    internal static bool IsSpoiler(EntryStatus status)
    {
        if (GuideConfig.Instance?.Spoilers != SpoilerMode.HideFuture)
            return false;
        if (status.Done == true || status.State == EntryState.Next)
            return false;
        EntryStatus? next = ProgressService.Next;
        return next is not null && status.Entry.Order > next.Entry.Order + 0.001f;
    }

    internal static string DisplayName(EntryStatus status) => IsSpoiler(status) ? "???" : EntryNames.Name(status.Entry.Id);

    /// <summary>A short warning for items that are easy to misuse (Rod of Discord in the Abyss ...), if any.</summary>
    internal static string? ItemWarning(int type)
    {
        if (type <= 0)
            return null;
        string key = Variant("Items.Warning." + GuideCatalog.ItemKey(type).Replace('/', '_'));
        return Has(key) ? L(key) : null;
    }

    internal static int PortraitNpc(ProgressionEntry entry) => entry.Npcs.Count > 0 ? GuideCatalog.NpcType(entry.Npcs[0]) : 0;

    /// <summary>Picture of an entry: its NPC, or its icon item for custom-drawn bosses, else the first summon item.</summary>
    internal static NpcPortrait Portrait(ProgressionEntry entry, int size)
    {
        int icon = entry.Icon is null ? 0 : GuideCatalog.ItemType(entry.Icon);
        int summon = entry.Summons.Select(GuideCatalog.ItemType).FirstOrDefault(t => t > 0);
        return new NpcPortrait(icon > 0 ? 0 : PortraitNpc(entry), size) { Item = icon, FallbackItem = summon };
    }

    /// <summary>Small colored mod label ("Calamity", "Infernum" ...).</summary>
    internal static Badge ModBadge(string mod)
    {
        string name = mod == "Terraria" ? "Terraria" : Has("ModShort." + mod) ? L("ModShort." + mod) : SupportedMods.DisplayName(mod);
        Color color = mod switch
        {
            "Terraria" => new Color(140, 200, 120),
            "CalamityMod" => new Color(230, 90, 80),
            "InfernumMode" => new Color(255, 140, 50),
            "NoxusBoss" => new Color(200, 140, 255),
            "CalamityHunt" => new Color(120, 220, 200),
            "CatalystMod" => new Color(120, 160, 255),
            "CalamityFables" => new Color(255, 205, 110),
            "SpiritReforged" => new Color(110, 210, 255),
            _ => Theme.TextMuted,
        };
        return new Badge(name, color);
    }

    internal static Badge StateBadge(EntryStatus status)
    {
        return new Badge(StateName(status.State), StateColor(status.State), solid: status.State is EntryState.Next or EntryState.Done);
    }

    /// <summary>A compact row for a boss/event entry. Click opens its page.</summary>
    internal static Node EntryRow(EntryStatus status, bool showWhy = true)
    {
        ProgressionEntry entry = status.Entry;
        bool spoiler = IsSpoiler(status);
        var row = new HStack { Spacing = 10 };
        NpcPortrait portrait = Portrait(entry, 40);
        portrait.Hidden = spoiler;
        portrait.Dim = status.Done == true;
        row.AddFixed(portrait, 44);

        var text = new VStack { Spacing = 0 };
        var titleLine = new Flow { Spacing = 6 };
        titleLine.Add(new Label(DisplayName(status), status.Done == true ? Theme.TextMuted : Theme.Text, Theme.Body));
        if (entry.Kind != "boss")
            titleLine.Add(new Badge(KindName(entry.Kind), Theme.Info));
        if (entry.Mod != "Terraria")
            titleLine.Add(ModBadge(entry.Mod));
        if (entry.Gate)
            titleLine.Add(new Badge(L("Badges.Gate"), Theme.Danger));
        else if (!entry.IsMain)
            titleLine.Add(new Badge(L("Badges.Optional"), Theme.TextFaint));
        text.Add(titleLine);
        if (showWhy && !spoiler)
        {
            string summary = EntryNames.Text(entry, "Short");
            if (summary.Length == 0)
                summary = EntryNames.Text(entry, "Summary");
            if (summary.Length > 0)
                text.Add(new Label(summary, Theme.TextMuted, Theme.Small));
        }
        row.AddFlex(text);

        if (!spoiler && entry.Summons.Count > 0)
        {
            var summons = new Flow { Spacing = 4 };
            foreach (string key in entry.Summons.Take(3))
            {
                int type = GuideCatalog.ItemType(key);
                if (type > 0)
                    summons.Add(new ItemSlotNode(type, 34));
            }
            if (summons.Children.Count > 0)
                row.AddFixed(summons, summons.Children.Count * 38);
        }
        row.AddFixed(StateBadge(status), 118);

        var card = new Card(row)
        {
            Padding = 8,
            Fill = status.State == EntryState.Next ? new Color(52, 46, 26) : Theme.Surface,
            BorderColor = status.State == EntryState.Next ? Theme.Gold * 0.8f : Theme.Border,
            Accent = Theme.Era(entry.Era) * (status.Done == true ? 0.4f : 1f),
            OnClick = () => Navigator.OpenEntry(entry.Id),
        };
        return card;
    }

    /// <summary>Current class of the player: explicit preference, else the held weapon's class.</summary>
    internal static string CurrentClass()
    {
        if (Main.gameMenu)
            return "melee";
        GuidePlayer? gp = Main.LocalPlayer.TryGetModPlayer(out GuidePlayer p) ? p : null;
        if (!string.IsNullOrEmpty(gp?.PreferredClass))
            return gp!.PreferredClass;
        Item held = Main.LocalPlayer.HeldItem;
        string cls = ItemSearch.ClassOf(held);
        return cls.Length > 0 ? cls : "melee";
    }

    internal static string ClassName(string cls) => L("Classes." + cls);

    internal static int ClassIcon(string cls) => cls switch
    {
        "ranged" => ItemID.RangerEmblem,
        "magic" => ItemID.SorcererEmblem,
        "summon" => ItemID.SummonerEmblem,
        "rogue" => GuideCatalog.ItemType("CalamityMod/RogueEmblem") is int t && t > 0 ? t : ItemID.WarriorEmblem,
        _ => ItemID.WarriorEmblem,
    };

    internal static IEnumerable<string> Classes()
    {
        yield return "melee";
        yield return "ranged";
        yield return "magic";
        yield return "summon";
        if (SupportedMods.CalamityLoaded)
            yield return "rogue";
    }

    /// <summary>Coin amount as "1 platinum 2 gold" style text in the current language.</summary>
    internal static string Coins(int copper)
    {
        if (copper <= 0)
            return L("Items.Free");
        int plat = copper / 1000000;
        int gold = copper / 10000 % 100;
        int silver = copper / 100 % 100;
        int cu = copper % 100;
        var parts = new List<string>();
        if (plat > 0) parts.Add(plat + " " + Lang.inter[15].Value);
        if (gold > 0) parts.Add(gold + " " + Lang.inter[16].Value);
        if (silver > 0) parts.Add(silver + " " + Lang.inter[17].Value);
        if (cu > 0) parts.Add(cu + " " + Lang.inter[18].Value);
        return string.Join(" ", parts);
    }

    internal static string Percent(float chance)
    {
        if (chance >= 0.9995f)
            return "100%";
        float p = chance * 100f;
        if (p >= 10f)
            return p.ToString("0") + "%";
        if (p >= 1f)
            return p.ToString("0.#") + "%";
        return p.ToString("0.##") + "%";
    }

    /// <summary>A titled section: heading + content, with vertical spacing.</summary>
    internal static VStack Section(string title, int icon = 0, string? right = null)
    {
        var stack = new VStack { Spacing = 8 };
        stack.Add(new Heading(title, right, icon));
        return stack;
    }

    /// <summary>Callout box (info/warning).</summary>
    internal static Card Callout(string markup, Color color, string? title = null)
    {
        var inner = new VStack { Spacing = 4 };
        if (title is not null)
            inner.Add(new Label(title, color, Theme.Body));
        inner.Add(new Paragraph(markup, Theme.Text, Theme.Body));
        return new Card(inner) { Fill = Color.Lerp(Theme.Surface, color, 0.10f), BorderColor = color * 0.7f, Accent = color, Padding = 10 };
    }

    internal static Paragraph Muted(string text) => new(text, Theme.TextMuted, Theme.Small);
}

/// <summary>NPC portrait in a rounded frame.</summary>
internal sealed class NpcPortrait : Node
{
    public int Npc;
    public int Size;
    public bool Hidden;
    public bool Dim;
    public bool Frame = true;

    /// <summary>Item drawn instead of the NPC.</summary>
    public int Item;

    /// <summary>Item drawn when the NPC has no usable picture.</summary>
    public int FallbackItem;

    public NpcPortrait(int npc, int size)
    {
        Npc = npc;
        Size = size;
    }

    public override int Measure(int width) => Size;

    public override void Draw(Ctx ctx)
    {
        var box = new Rectangle(Bounds.X, Bounds.Y, Math.Min(Bounds.Width, Size), Size);
        if (Frame)
            Gfx.Panel(ctx.Sb, box, Theme.Slot, Theme.Border, 6);
        if (Hidden)
        {
            Txt.DrawCentered(ctx.Sb, "?", box, Theme.TextFaint, Theme.Title);
            return;
        }
        var inner = new Rectangle(box.X + 3, box.Y + 3, box.Width - 6, box.Height - 6);
        Color tint = Dim ? Color.White * 0.55f : Color.White;
        if (Item > 0 || !ItemIcons.DrawNpc(ctx.Sb, Npc, inner, tint, preferHead: Size <= 64))
        {
            int item = Item > 0 ? Item : FallbackItem;
            if (item > 0)
                ItemIcons.Draw(ctx.Sb, item, inner.Center.ToVector2(), inner.Width * 0.8f, tint);
        }
    }
}
