using KhuMue.Core;
using Terraria.Audio;

namespace KhuMue.UI;

/// <summary>Per-frame draw/input context. Mouse is in the node's coordinate space.</summary>
internal sealed class Ctx
{
    public SpriteBatch Sb = null!;
    public Point Mouse;
    public Node? Hovered;
    public bool MouseInView = true;

    /// <summary>The left button went down this frame (see GuideWindow.CaptureInput).</summary>
    public bool LeftPressed;

    public bool IsHovered(Node node) => ReferenceEquals(Hovered, node);
}

/// <summary>
/// Retained layout node. Pages build a tree once; the window measures/arranges it when the
/// size changes and draws it every frame. Bounds are in UI pixels, in the coordinate space of the
/// nearest scroll view.
/// </summary>
internal abstract class Node
{
    public Rectangle Bounds;
    public bool Visible = true;

    public Action? OnClick;
    public Action? OnRightClick;

    /// <summary>Plain tooltip shown on hover.</summary>
    public Func<string>? Tooltip;

    /// <summary>Item whose game tooltip is shown on hover.</summary>
    public int TooltipItem;

    public virtual bool Interactive => OnClick is not null || OnRightClick is not null || Tooltip is not null || TooltipItem > 0;

    /// <summary>Height needed for the given width.</summary>
    public abstract int Measure(int width);

    public virtual void Arrange(Rectangle bounds) => Bounds = bounds;

    public abstract void Draw(Ctx ctx);

    /// <summary>Deepest interactive node under the point, or null.</summary>
    public virtual Node? HitTest(Point p) => Visible && Interactive && Bounds.Contains(p) ? this : null;

    /// <summary>Returns true when the wheel was consumed.</summary>
    public virtual bool Scroll(Point p, int delta) => false;

    /// <summary>Called every update while visible (e.g. scroll-bar dragging).</summary>
    public virtual void Update(Ctx ctx)
    {
    }
}

/// <summary>A node with children.</summary>
internal abstract class Container : Node
{
    public readonly List<Node> Children = new();

    public T Add<T>(T child) where T : Node
    {
        Children.Add(child);
        return child;
    }

    public override void Draw(Ctx ctx)
    {
        foreach (Node child in Children)
        {
            if (child.Visible)
                child.Draw(ctx);
        }
    }

    public override Node? HitTest(Point p)
    {
        if (!Visible || !Bounds.Contains(p))
            return null;
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            Node? hit = Children[i].HitTest(p);
            if (hit is not null)
                return hit;
        }
        return Interactive ? this : null;
    }

    public override bool Scroll(Point p, int delta)
    {
        if (!Visible || !Bounds.Contains(p))
            return false;
        for (int i = Children.Count - 1; i >= 0; i--)
        {
            if (Children[i].Scroll(p, delta))
                return true;
        }
        return false;
    }

    public override void Update(Ctx ctx)
    {
        foreach (Node child in Children)
        {
            if (child.Visible)
                child.Update(ctx);
        }
    }
}

/// <summary>Vertical stack. Children use the full width.</summary>
internal sealed class VStack : Container
{
    public int Spacing = Theme.Gap;
    public int Padding;

    public override int Measure(int width)
    {
        int inner = width - Padding * 2;
        int h = 0, count = 0;
        foreach (Node child in Children)
        {
            if (!child.Visible)
                continue;
            h += child.Measure(inner);
            count++;
        }
        return h + Math.Max(0, count - 1) * Spacing + Padding * 2;
    }

    public override void Arrange(Rectangle bounds)
    {
        Bounds = bounds;
        int inner = bounds.Width - Padding * 2;
        int y = bounds.Y + Padding;
        foreach (Node child in Children)
        {
            if (!child.Visible)
                continue;
            int h = child.Measure(inner);
            child.Arrange(new Rectangle(bounds.X + Padding, y, inner, h));
            y += h + Spacing;
        }
    }
}

/// <summary>Horizontal row. Fixed-width children keep their width; the rest share the remainder.</summary>
internal sealed class HStack : Container
{
    public int Spacing = Theme.Gap;
    public readonly Dictionary<Node, int> Fixed = new();
    public readonly Dictionary<Node, float> Weights = new();

    /// <summary>Vertically center children in the row.</summary>
    public bool CenterY = true;

    public T AddFixed<T>(T child, int width) where T : Node
    {
        Add(child);
        Fixed[child] = width;
        return child;
    }

    public T AddFlex<T>(T child, float weight = 1f) where T : Node
    {
        Add(child);
        Weights[child] = weight;
        return child;
    }

    private IEnumerable<(Node Node, int Width)> Widths(int width)
    {
        var visible = Children.Where(c => c.Visible).ToList();
        int fixedTotal = visible.Where(Fixed.ContainsKey).Sum(c => Fixed[c]);
        int gaps = Math.Max(0, visible.Count - 1) * Spacing;
        float weightTotal = visible.Where(c => !Fixed.ContainsKey(c)).Sum(c => Weights.TryGetValue(c, out float w) ? w : 1f);
        int flexSpace = Math.Max(0, width - fixedTotal - gaps);
        foreach (Node child in visible)
        {
            if (Fixed.TryGetValue(child, out int fw))
                yield return (child, fw);
            else
                yield return (child, weightTotal <= 0 ? 0 : (int)(flexSpace * (Weights.TryGetValue(child, out float w) ? w : 1f) / weightTotal));
        }
    }

    public override int Measure(int width)
    {
        int h = 0;
        foreach ((Node node, int w) in Widths(width))
            h = Math.Max(h, node.Measure(w));
        return h;
    }

    public override void Arrange(Rectangle bounds)
    {
        Bounds = bounds;
        int x = bounds.X;
        foreach ((Node node, int w) in Widths(bounds.Width))
        {
            int h = node.Measure(w);
            int y = CenterY ? bounds.Y + (bounds.Height - h) / 2 : bounds.Y;
            node.Arrange(new Rectangle(x, y, w, h));
            x += w + Spacing;
        }
    }
}

/// <summary>Wrapping flow of equally sized cells (icons, chips).</summary>
internal sealed class Flow : Container
{
    public int Spacing = 6;
    public int RowSpacing = 6;

    /// <summary>When > 0 every child gets this width; otherwise children report their own width via <see cref="IFlowItem"/>.</summary>
    public int CellWidth;
    public int CellHeight;

    private int Width(Node child, int available) => CellWidth > 0 ? Math.Min(CellWidth, available) : child is IFlowItem f ? Math.Min(f.PreferredWidth, available) : available;

    public override int Measure(int width)
    {
        int x = 0, y = 0, rowH = 0;
        foreach (Node child in Children)
        {
            if (!child.Visible)
                continue;
            int w = Width(child, width);
            int h = CellHeight > 0 ? CellHeight : child.Measure(w);
            if (x > 0 && x + w > width)
            {
                x = 0;
                y += rowH + RowSpacing;
                rowH = 0;
            }
            x += w + Spacing;
            rowH = Math.Max(rowH, h);
        }
        return y + rowH;
    }

    public override void Arrange(Rectangle bounds)
    {
        Bounds = bounds;
        int x = 0, y = 0, rowH = 0;
        foreach (Node child in Children)
        {
            if (!child.Visible)
                continue;
            int w = Width(child, bounds.Width);
            int h = CellHeight > 0 ? CellHeight : child.Measure(w);
            if (x > 0 && x + w > bounds.Width)
            {
                x = 0;
                y += rowH + RowSpacing;
                rowH = 0;
            }
            child.Arrange(new Rectangle(bounds.X + x, bounds.Y + y, w, h));
            x += w + Spacing;
            rowH = Math.Max(rowH, h);
        }
    }
}

internal interface IFlowItem
{
    int PreferredWidth { get; }
}

/// <summary>Background panel around a single child.</summary>
internal sealed class Card : Container
{
    public int Padding = Theme.Pad;
    public Color Fill = Theme.Surface;
    public Color? BorderColor = Theme.Border;
    public Color? HoverFill;

    /// <summary>Optional colored strip on the left edge.</summary>
    public Color? Accent;

    public Card(Node? child = null)
    {
        if (child is not null)
            Add(child);
    }

    public override int Measure(int width) => Children.Count == 0 ? Padding * 2 : Children[0].Measure(width - Padding * 2 - (Accent is null ? 0 : 4)) + Padding * 2;

    public override void Arrange(Rectangle bounds)
    {
        Bounds = bounds;
        int left = Accent is null ? 0 : 4;
        if (Children.Count > 0)
            Children[0].Arrange(new Rectangle(bounds.X + Padding + left, bounds.Y + Padding, bounds.Width - Padding * 2 - left, bounds.Height - Padding * 2));
    }

    public override void Draw(Ctx ctx)
    {
        bool hover = Interactive && ctx.IsHovered(this) && (OnClick is not null);
        Draw_Panel(ctx.Sb, hover);
        base.Draw(ctx);
    }

    private void Draw_Panel(SpriteBatch sb, bool hover)
    {
        Color fill = hover ? (HoverFill ?? Theme.SurfaceHover) : Fill;
        Gfx.Panel(sb, Bounds, fill, hover ? Theme.BorderStrong : BorderColor);
        if (Accent is Color accent)
            Gfx.Panel(sb, new Rectangle(Bounds.X, Bounds.Y, 4, Bounds.Height), accent, null, 3);
    }
}

/// <summary>Empty space.</summary>
internal sealed class Spacer(int height) : Node
{
    public override int Measure(int width) => height;
    public override void Draw(Ctx ctx)
    {
    }
}

/// <summary>Thin horizontal rule.</summary>
internal sealed class Divider : Node
{
    public override int Measure(int width) => 9;
    public override void Draw(Ctx ctx) => Gfx.Fill(ctx.Sb, new Rectangle(Bounds.X, Bounds.Y + 4, Bounds.Width, 1), Theme.Border);
}

/// <summary>Single line of text, cut with an ellipsis when too long.</summary>
internal sealed class Label : Node, IFlowItem
{
    public string Text;
    public Color Color;
    public float Scale;
    public bool Center;
    public bool Right;

    public Label(string text, Color? color = null, float? scale = null)
    {
        Text = text;
        Color = color ?? Theme.Text;
        Scale = scale ?? Theme.Body;
    }

    public int PreferredWidth => (int)Math.Ceiling(Txt.Width(Text, Scale)) + 2;

    public override int Measure(int width) => Theme.LineHeight(Scale);

    public override void Draw(Ctx ctx)
    {
        string shown = Txt.Ellipsize(Text, Bounds.Width, Scale);
        float w = Txt.Width(shown, Scale);
        float x = Center ? Bounds.X + (Bounds.Width - w) / 2f : Right ? Bounds.Right - w : Bounds.X;
        Txt.DrawString(ctx.Sb, shown, new Vector2(x, Bounds.Y + 2), Color, Scale);
        if (shown != Text && Tooltip is null)
            Tooltip = () => Text;
    }
}

/// <summary>Wrapped paragraph with inline icons and links (see <see cref="RichText"/>).</summary>
internal sealed class Paragraph : Node
{
    public string Markup;
    public Color Color;
    public float Scale;
    private RichLayout? _layout;
    private RichLayout.Run? _hoverRun;

    public Paragraph(string markup, Color? color = null, float? scale = null)
    {
        Markup = markup;
        Color = color ?? Theme.Text;
        Scale = scale ?? Theme.Body;
    }

    public override bool Interactive => true;

    public override int Measure(int width)
    {
        if (string.IsNullOrWhiteSpace(Markup))
            return 0;
        return RichText.Layout(Markup, Math.Max(40, width), Scale, Color).Height;
    }

    public override void Arrange(Rectangle bounds)
    {
        Bounds = bounds;
        _layout = RichText.Layout(Markup, Math.Max(40, bounds.Width), Scale, Color);
    }

    public override Node? HitTest(Point p)
    {
        if (!Visible || !Bounds.Contains(p) || _layout is null)
            return null;
        RichLayout.Run? run = _layout.RunAt(new Point(p.X - Bounds.X, p.Y - Bounds.Y));
        return run is null ? null : this;
    }

    public override void Draw(Ctx ctx)
    {
        if (_layout is null)
            return;
        _hoverRun = ctx.IsHovered(this) ? _layout.RunAt(new Point(ctx.Mouse.X - Bounds.X, ctx.Mouse.Y - Bounds.Y)) : null;
        _layout.Draw(ctx.Sb, new Vector2(Bounds.X, Bounds.Y), Scale, _hoverRun);
        if (_hoverRun is not null)
        {
            TooltipItem = _hoverRun.Link.Kind == LinkKind.Item ? _hoverRun.Link.Item : 0;
            Link link = _hoverRun.Link;
            OnClick = () => Navigator.Follow(link);
        }
        else
        {
            TooltipItem = 0;
            OnClick = null;
        }
    }
}

/// <summary>Clickable button with optional item icon.</summary>
internal sealed class Button : Node, IFlowItem
{
    public string Text;
    public int Icon;
    public bool Primary;
    public bool Selected;
    public bool Enabled = true;
    public float Scale = Theme.Body;
    public Color? TextColor;
    public int MinWidth;

    public Button(string text, Action? onClick = null, int icon = 0)
    {
        Text = text;
        OnClick = onClick;
        Icon = icon;
    }

    public int PreferredWidth => Math.Max(MinWidth, (int)Math.Ceiling(Txt.Width(Text, Scale)) + 22 + (Icon > 0 ? Theme.LineHeight(Scale) + 4 : 0));

    public override bool Interactive => Enabled && base.Interactive;

    public override int Measure(int width) => Theme.LineHeight(Scale) + 12;

    public override void Draw(Ctx ctx)
    {
        bool hover = Enabled && ctx.IsHovered(this);
        Color fill = Primary ? (hover ? Color.Lerp(Theme.Accent, Color.White, 0.15f) : Theme.Accent)
            : Selected ? Theme.SurfacePressed
            : hover ? Theme.SurfaceHover : Theme.SurfaceAlt;
        Color border = Selected ? Theme.Accent : hover ? Theme.BorderStrong : Theme.Border;
        Gfx.Panel(ctx.Sb, Bounds, Enabled ? fill : Theme.Surface, Primary ? null : border);
        Color textColor = !Enabled ? Theme.TextFaint : Primary ? new Color(10, 24, 28) : TextColor ?? (Selected ? Theme.Text : Theme.TextMuted);
        int iconSpace = 0;
        float textW = Txt.Width(Text, Scale);
        int lh = Theme.LineHeight(Scale);
        if (Icon > 0)
            iconSpace = lh + 4;
        float contentW = textW + iconSpace;
        float x = Bounds.X + Math.Max(8, (Bounds.Width - contentW) / 2f);
        if (Icon > 0)
        {
            ItemIcons.Draw(ctx.Sb, Icon, new Vector2(x + lh / 2f, Bounds.Center.Y), lh - 2);
            x += iconSpace;
        }
        string shown = Txt.Ellipsize(Text, Bounds.Width - iconSpace - 12, Scale);
        Txt.DrawString(ctx.Sb, shown, new Vector2(x, Bounds.Y + (Bounds.Height - lh) / 2f + 2), textColor, Scale, shadow: !Primary);
    }
}

/// <summary>Small colored pill with text (status, era, mod).</summary>
internal sealed class Badge : Node, IFlowItem
{
    public string Text;
    public Color Color;
    public bool Solid;
    public float Scale = Theme.Small;

    public Badge(string text, Color color, bool solid = false)
    {
        Text = text;
        Color = color;
        Solid = solid;
    }

    public int PreferredWidth => (int)Math.Ceiling(Txt.Width(Text, Scale)) + 14;

    public override int Measure(int width) => Theme.LineHeight(Scale) + 4;

    public override void Draw(Ctx ctx)
    {
        Rectangle box = Bounds;
        box.Width = Math.Min(Bounds.Width, PreferredWidth);
        Gfx.Panel(ctx.Sb, box, Solid ? Color : Color * 0.18f, Color * (Solid ? 0f : 0.7f), 4);
        Txt.DrawCentered(ctx.Sb, Txt.Ellipsize(Text, box.Width - 8, Scale), box, Solid ? new Color(12, 18, 26) : Color, Scale);
    }
}

/// <summary>Item slot: icon on a slot background; hover shows the game tooltip; click opens the item page.</summary>
internal sealed class ItemSlotNode : Node, IFlowItem
{
    public int Item;
    public int Size;
    public int Stack;
    public string? Caption;

    /// <summary>Show a small check when the player owns it.</summary>
    public bool ShowOwned = true;

    public ItemSlotNode(int item, int size = 40, int stack = 0)
    {
        Item = item;
        Size = size;
        Stack = stack;
        TooltipItem = item;
        OnClick = () => Navigator.OpenItem(item);
    }

    public int PreferredWidth => Size;

    public override int Measure(int width) => Size + (Caption is null ? 0 : Theme.LineHeight(Theme.Small));

    public override void Draw(Ctx ctx)
    {
        var box = new Rectangle(Bounds.X, Bounds.Y, Size, Size);
        bool hover = ctx.IsHovered(this);
        Gfx.Panel(ctx.Sb, box, hover ? Theme.SurfaceHover : Theme.Slot, hover ? Theme.Accent : Theme.Border, 5);
        ItemIcons.Draw(ctx.Sb, Item, box.Center.ToVector2(), Size - 10);
        if (Stack > 1)
            Txt.DrawString(ctx.Sb, Stack.ToString(), new Vector2(box.X + 4, box.Bottom - Theme.LineHeight(Theme.Small) - 1), Theme.Text, Theme.Small);
        if (ShowOwned && Inventory.Owns(Item))
        {
            Gfx.Circle(ctx.Sb, new Vector2(box.Right - 6, box.Y + 6), 5f, Theme.Success);
        }
        if (Caption is not null)
        {
            var captionBox = new Rectangle(Bounds.X - 12, box.Bottom, Size + 24, Theme.LineHeight(Theme.Small));
            Txt.DrawCentered(ctx.Sb, Txt.Ellipsize(Caption, captionBox.Width, Theme.Small), captionBox, Theme.TextMuted, Theme.Small);
        }
    }
}

/// <summary>Row: item icon + name (+ optional detail line). Click opens the item.</summary>
internal sealed class ItemRow : Node
{
    public int Item;
    public string? Detail;
    public Color? DetailColor;
    public string? Right;
    public bool Compact;

    public ItemRow(int item, string? detail = null)
    {
        Item = item;
        Detail = detail;
        TooltipItem = item;
        OnClick = () => Navigator.OpenItem(item);
    }

    public override int Measure(int width)
    {
        int lh = Theme.LineHeight(Theme.Body);
        int text = lh + (Detail is null ? 0 : Theme.LineHeight(Theme.Small));
        return Math.Max(Compact ? 30 : 38, text + 6);
    }

    public override void Draw(Ctx ctx)
    {
        bool hover = ctx.IsHovered(this);
        if (hover)
            Gfx.Panel(ctx.Sb, Bounds, Theme.SurfaceHover, null, 5);
        int icon = Math.Min(Bounds.Height - 4, 34);
        ItemIcons.Draw(ctx.Sb, Item, new Vector2(Bounds.X + 4 + icon / 2f, Bounds.Center.Y), icon - 2);
        int x = Bounds.X + icon + 12;
        int rightW = Right is null ? 0 : (int)Txt.Width(Right, Theme.Small) + 8;
        int w = Bounds.Right - x - rightW;
        int lh = Theme.LineHeight(Theme.Body);
        int total = lh + (Detail is null ? 0 : Theme.LineHeight(Theme.Small));
        int y = Bounds.Y + (Bounds.Height - total) / 2;
        string name = GuideCatalog.ItemName(Item);
        Txt.DrawString(ctx.Sb, Txt.Ellipsize(name, w, Theme.Body), new Vector2(x, y + 2), RichText.ItemColor(Item), Theme.Body);
        if (Detail is not null)
            Txt.DrawString(ctx.Sb, Txt.Ellipsize(Detail, w, Theme.Small), new Vector2(x, y + lh + 1), DetailColor ?? Theme.TextMuted, Theme.Small);
        if (Right is not null)
            Txt.DrawString(ctx.Sb, Right, new Vector2(Bounds.Right - rightW + 4, Bounds.Center.Y - Theme.LineHeight(Theme.Small) / 2f + 2), Theme.TextMuted, Theme.Small);
        if (Inventory.Owns(Item))
            Gfx.Circle(ctx.Sb, new Vector2(Bounds.X + icon + 4, Bounds.Y + 7), 4f, Theme.Success);
    }
}

/// <summary>Section heading with optional right-side text.</summary>
internal sealed class Heading : Node
{
    public string Text;
    public string? RightText;
    public Color Color = Theme.Text;
    public float Scale = Theme.Title;
    public int Icon;

    public Heading(string text, string? right = null, int icon = 0)
    {
        Text = text;
        RightText = right;
        Icon = icon;
    }

    public override int Measure(int width) => Theme.LineHeight(Scale) + 6;

    public override void Draw(Ctx ctx)
    {
        int x = Bounds.X;
        int lh = Theme.LineHeight(Scale);
        if (Icon > 0)
        {
            ItemIcons.Draw(ctx.Sb, Icon, new Vector2(x + lh / 2f, Bounds.Y + lh / 2f + 1), lh);
            x += lh + 6;
        }
        Txt.DrawString(ctx.Sb, Txt.Ellipsize(Text, Bounds.Right - x, Scale), new Vector2(x, Bounds.Y + 2), Color, Scale);
        if (RightText is not null)
        {
            float w = Txt.Width(RightText, Theme.Small);
            Txt.DrawString(ctx.Sb, RightText, new Vector2(Bounds.Right - w, Bounds.Y + (lh - Theme.LineHeight(Theme.Small)) / 2f + 3), Theme.TextMuted, Theme.Small);
        }
        Gfx.Fill(ctx.Sb, new Rectangle(Bounds.X, Bounds.Bottom - 2, Bounds.Width, 1), Theme.Border);
    }
}

/// <summary>Horizontal progress bar with a caption.</summary>
internal sealed class ProgressBar : Node
{
    public float Value;
    public Color Color = Theme.Accent;
    public string? Caption;

    public override int Measure(int width) => Caption is null ? 10 : Theme.LineHeight(Theme.Small) + 12;

    public override void Draw(Ctx ctx)
    {
        int y = Bounds.Y;
        if (Caption is not null)
        {
            Txt.DrawString(ctx.Sb, Caption, new Vector2(Bounds.X, y + 1), Theme.TextMuted, Theme.Small);
            y += Theme.LineHeight(Theme.Small) + 2;
        }
        var track = new Rectangle(Bounds.X, y, Bounds.Width, 8);
        Gfx.Panel(ctx.Sb, track, Theme.Slot, null, 4);
        int fill = (int)(track.Width * Math.Clamp(Value, 0f, 1f));
        if (fill > 0)
            Gfx.Panel(ctx.Sb, new Rectangle(track.X, track.Y, Math.Max(fill, 8), track.Height), Color, null, 4);
    }
}

/// <summary>Custom-drawn node for one-off visuals.</summary>
internal sealed class Canvas : Node
{
    public Func<int, int> Height;
    public Action<Ctx, Rectangle> Paint;

    public Canvas(Func<int, int> height, Action<Ctx, Rectangle> paint)
    {
        Height = height;
        Paint = paint;
    }

    public override int Measure(int width) => Height(width);
    public override void Draw(Ctx ctx) => Paint(ctx, Bounds);
}

/// <summary>Plays the standard menu tick for clicks.</summary>
internal static class Sounds
{
    internal static void Click() => SoundEngine.PlaySound(SoundID.MenuTick);
    internal static void Open() => SoundEngine.PlaySound(SoundID.MenuOpen);
    internal static void Close() => SoundEngine.PlaySound(SoundID.MenuClose);
}

/// <summary>Cached "does the player have this item" checks (inventory, piggy bank, safe, forge, void vault, equipment).</summary>
internal static class Inventory
{
    private static readonly HashSet<int> Owned = new();
    private static uint _last = uint.MaxValue;

    internal static bool Owns(int type)
    {
        if (Main.gameMenu || type <= 0)
            return false;
        uint now = Main.GameUpdateCount;
        if (_last == uint.MaxValue || now - _last > 30)
        {
            _last = now;
            Owned.Clear();
            Player p = Main.LocalPlayer;
            void AddAll(Item[] items)
            {
                foreach (Item item in items)
                {
                    if (item is not null && !item.IsAir)
                        Owned.Add(item.type);
                }
            }
            AddAll(p.inventory);
            AddAll(p.armor);
            AddAll(p.miscEquips);
            AddAll(p.bank.item);
            AddAll(p.bank2.item);
            AddAll(p.bank3.item);
            AddAll(p.bank4.item);
            if (!p.HeldItem.IsAir)
                Owned.Add(p.HeldItem.type);
        }
        return Owned.Contains(type);
    }

    internal static int Count(int type)
    {
        if (Main.gameMenu)
            return 0;
        int count = 0;
        foreach (Item item in Main.LocalPlayer.inventory)
        {
            if (item is not null && item.type == type)
                count += item.stack;
        }
        return count;
    }

    internal static void Invalidate() => _last = uint.MaxValue;
}
