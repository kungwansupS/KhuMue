using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using KhuMue.Core;
using Terraria.GameContent.UI;

namespace KhuMue.UI;

internal enum LinkKind
{
    None,
    Item,
    Entry,
}

internal readonly record struct Link(LinkKind Kind, int Item = 0, string? Entry = null)
{
    public static readonly Link None = new(LinkKind.None);
}

/// <summary>
/// Paragraph text with inline item icons and links.
/// Markup: [i:Mod/Name] icon only · [item:Mod/Name] icon + name · [e:EntryId] entry link ·
/// [c/RRGGBB:text] color · **text** highlight · lines starting with "• " become bullets.
/// Item and NPC names are resolved at runtime, so they follow the player's language.
/// </summary>
internal sealed class RichLayout
{
    internal sealed class Run
    {
        public string Text = "";
        public int Item;
        public Rectangle Box;
        public Color Color;
        public Link Link;
        public bool Bullet;
        public bool IsIcon => Item > 0 && Text.Length == 0;
    }

    public readonly List<Run> Runs = new();
    public int Height;
    public int Width;

    public Run? RunAt(Point local)
    {
        foreach (Run run in Runs)
        {
            if (run.Link.Kind != LinkKind.None && run.Box.Contains(local))
                return run;
        }
        return null;
    }

    public void Draw(SpriteBatch sb, Vector2 origin, float scale, Run? hovered)
    {
        foreach (Run run in Runs)
        {
            var box = new Rectangle((int)origin.X + run.Box.X, (int)origin.Y + run.Box.Y, run.Box.Width, run.Box.Height);
            if (run.Bullet)
            {
                Gfx.Circle(sb, new Vector2(box.X + 5, box.Y + box.Height / 2f + 1), 3f, Theme.Accent);
                continue;
            }
            if (run.IsIcon)
            {
                ItemIcons.Draw(sb, run.Item, box.Center.ToVector2(), box.Height - 2);
                if (run == hovered)
                    Draw_Underline(sb, box, Theme.Link);
                continue;
            }
            Color color = run.Color;
            if (run.Link.Kind != LinkKind.None && run == hovered)
                color = Color.Lerp(color, Color.White, 0.45f);
            Txt.DrawString(sb, run.Text, new Vector2(box.X, box.Y + 2), color, scale);
            if (run.Link.Kind != LinkKind.None && run == hovered)
                Draw_Underline(sb, box, color);
        }
    }

    private static void Draw_Underline(SpriteBatch sb, Rectangle box, Color color)
    {
        Gfx.Fill(sb, new Rectangle(box.X, box.Bottom - 2, box.Width, 1), color * 0.8f);
    }
}

internal static class RichText
{
    private static readonly Dictionary<(string, int, float, Color), RichLayout> Cache = new();

    internal static void ClearCache() => Cache.Clear();

    private enum TokenKind
    {
        Text,
        Icon,
        Newline,
    }

    private readonly record struct Token(TokenKind Kind, string Text, Color Color, Link Link, int Item = 0);

    internal static RichLayout Layout(string markup, int width, float scale, Color baseColor)
    {
        var key = (markup ?? "", width, scale, baseColor);
        if (Cache.TryGetValue(key, out RichLayout? cached))
            return cached;
        if (Cache.Count > 1500)
            Cache.Clear();

        var layout = new RichLayout { Width = width };
        int lineHeight = Theme.LineHeight(scale);
        int iconSize = lineHeight;
        int y = 0;

        foreach (string rawLine in (markup ?? "").Replace("\r", "").Split('\n'))
        {
            string line = rawLine;
            int indent = 0;
            bool bullet = false;
            if (line.StartsWith("• ") || line.StartsWith("- "))
            {
                bullet = true;
                line = line[2..];
                indent = 16;
            }

            if (bullet)
            {
                layout.Runs.Add(new RichLayout.Run { Bullet = true, Box = new Rectangle(2, y, indent, lineHeight) });
            }

            List<Token> tokens = Tokenize(line, baseColor);
            int x = indent;
            bool lineHasContent = false;
            foreach (Token token in tokens)
            {
                if (token.Kind == TokenKind.Icon)
                {
                    int w = iconSize + 2;
                    if (x + w > width && lineHasContent)
                    {
                        x = indent;
                        y += lineHeight;
                    }
                    layout.Runs.Add(new RichLayout.Run
                    {
                        Item = token.Item,
                        Box = new Rectangle(x + 1, y, iconSize, iconSize),
                        Link = token.Link,
                    });
                    x += w;
                    lineHasContent = true;
                    continue;
                }

                // Text: place piece by piece using the shared break rules.
                string text = token.Text;
                int start = 0;
                while (start < text.Length)
                {
                    int available = width - x;
                    string rest = text[start..];
                    float restWidth = Txt.Width(rest, scale);
                    if (restWidth <= available)
                    {
                        AddText(layout, rest, x, y, lineHeight, scale, token);
                        x += (int)Math.Ceiling(restWidth);
                        lineHasContent = true;
                        break;
                    }

                    int end = FitEndWithin(text, start, available, scale);
                    if (end <= start)
                    {
                        if (lineHasContent)
                        {
                            // Nothing fits on this line; wrap and retry.
                            x = indent;
                            y += lineHeight;
                            lineHasContent = false;
                            while (start < text.Length && text[start] == ' ')
                                start++;
                            continue;
                        }
                        end = Txt.FitEnd(text, start, available, scale);
                    }

                    string piece = text[start..end].TrimEnd();
                    AddText(layout, piece, x, y, lineHeight, scale, token);
                    start = end;
                    while (start < text.Length && text[start] == ' ')
                        start++;
                    x = indent;
                    y += lineHeight;
                    lineHasContent = false;
                }
            }
            y += lineHeight;
        }

        layout.Height = Math.Max(lineHeight, y);
        Cache[key] = layout;
        return layout;
    }

    /// <summary>Like <see cref="Text.FitEnd"/> but returns <paramref name="start"/> when not even one break point fits.</summary>
    private static int FitEndWithin(string text, int start, int available, float scale)
    {
        if (available <= 0)
            return start;
        int lastSpace = -1, lastAny = -1;
        foreach (int pos in Txt.BreakPositions(text, start))
        {
            if (Txt.Width(text[start..pos], scale) > available)
                break;
            lastAny = pos;
            char before = text[pos - 1];
            if (pos == text.Length || before == ' ' || before == '/' || before == '-')
                lastSpace = pos;
        }
        if (lastSpace > start && lastSpace - start >= (lastAny - start) / 3)
            return lastSpace;
        return lastAny > start ? lastAny : start;
    }

    private static void AddText(RichLayout layout, string text, int x, int y, int lineHeight, float scale, Token token)
    {
        if (text.Length == 0)
            return;
        int w = (int)Math.Ceiling(Txt.Width(text, scale));
        layout.Runs.Add(new RichLayout.Run
        {
            Text = text,
            Box = new Rectangle(x, y, w, lineHeight),
            Color = token.Color,
            Link = token.Link,
            Item = token.Link.Kind == LinkKind.Item ? token.Link.Item : 0,
        });
    }

    private static List<Token> Tokenize(string line, Color baseColor)
    {
        var tokens = new List<Token>();
        var buffer = new StringBuilder();
        bool bold = false;

        void Flush()
        {
            if (buffer.Length == 0)
                return;
            tokens.Add(new Token(TokenKind.Text, buffer.ToString(), bold ? Theme.Gold : baseColor, Link.None));
            buffer.Clear();
        }

        int i = 0;
        while (i < line.Length)
        {
            if (line[i] == '*' && i + 1 < line.Length && line[i + 1] == '*')
            {
                Flush();
                bold = !bold;
                i += 2;
                continue;
            }

            if (line[i] == '[')
            {
                int close = line.IndexOf(']', i);
                int colon = line.IndexOf(':', i);
                if (close > i && colon > i && colon < close)
                {
                    string tag = line[(i + 1)..colon];
                    string arg = line[(colon + 1)..close];
                    if (TryTag(tag, arg, tokens, bold ? Theme.Gold : baseColor, Flush))
                    {
                        i = close + 1;
                        continue;
                    }
                }
            }

            buffer.Append(line[i]);
            i++;
        }
        Flush();
        return tokens;
    }

    private static bool TryTag(string tag, string arg, List<Token> tokens, Color color, Action flush)
    {
        if (tag.StartsWith("i/", StringComparison.Ordinal))
        {
            // Chat item tag with options, as mods write them in tooltips: [i/s10:225] = 10 Silk, [i/p57:...] = prefix.
            Match stack = Regex.Match(tag, @"s(\d+)");
            bool known = TryTag("i", arg, tokens, color, flush);
            if (known && stack.Success && stack.Groups[1].Value != "1")
                tokens.Add(new Token(TokenKind.Text, "x" + stack.Groups[1].Value + " ", color, Link.None));
            return known;
        }

        switch (tag)
        {
            case "i":
            case "item":
            {
                int type = int.TryParse(arg, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id) ? id : GuideCatalog.ItemType(arg);
                flush();
                if (type <= 0 || type >= ItemLoader.ItemCount)
                {
                    // Unknown in this mod pack: show the raw name rather than a broken icon.
                    string fallback = arg.Contains('/') ? arg[(arg.IndexOf('/') + 1)..] : arg;
                    tokens.Add(new Token(TokenKind.Text, fallback, Theme.TextFaint, Link.None));
                    return true;
                }
                var link = new Link(LinkKind.Item, type);
                tokens.Add(new Token(TokenKind.Icon, "", color, link, type));
                if (tag == "item")
                    tokens.Add(new Token(TokenKind.Text, " " + GuideCatalog.ItemName(type), ItemColor(type), link));
                return true;
            }
            case "e":
            {
                flush();
                string name = EntryNames.Name(arg);
                bool known = GuideData.EntriesById.ContainsKey(arg);
                tokens.Add(new Token(TokenKind.Text, name, known ? Theme.Link : color, known ? new Link(LinkKind.Entry, 0, arg) : Link.None));
                return true;
            }
            case "npc":
            {
                flush();
                int type = GuideCatalog.NpcType(arg);
                string name = type != 0 ? Lang.GetNPCNameValue(type) : arg;
                tokens.Add(new Token(TokenKind.Text, name, Theme.Gold, Link.None));
                return true;
            }
            default:
                if (tag.StartsWith("c/") && tag.Length == 8 && uint.TryParse(tag[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint rgb))
                {
                    flush();
                    var c = new Color((int)(rgb >> 16) & 255, (int)(rgb >> 8) & 255, (int)rgb & 255);
                    tokens.Add(new Token(TokenKind.Text, arg, c, Link.None));
                    return true;
                }
                return false;
        }
    }

    internal static Color ItemColor(int type)
    {
        Item item = GuideCatalog.SampleItem(type);
        try
        {
            return ItemRarity.GetColor(item.rare);
        }
        catch
        {
            return Theme.Text;
        }
    }
}
