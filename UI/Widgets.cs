using KhuMue.Core;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace KhuMue.UI;

/// <summary>Vertical scroll container with clipping, wheel scrolling and a draggable bar.</summary>
internal sealed class ScrollView : Node
{
    public Node Content;
    public float ScrollY;
    private float _target;
    private int _contentHeight;
    private bool _dragging;
    private int _dragOffset;
    private const int BarWidth = 8;

    public ScrollView(Node content)
    {
        Content = content;
    }

    public int ContentHeight => _contentHeight;
    public int MaxScroll => Math.Max(0, _contentHeight - Bounds.Height);

    public override int Measure(int width) => Content.Measure(width - BarWidth - 4);

    public override void Arrange(Rectangle bounds)
    {
        Bounds = bounds;
        int width = bounds.Width - BarWidth - 6;
        _contentHeight = Content.Measure(width);
        Content.Arrange(new Rectangle(bounds.X, bounds.Y, width, _contentHeight));
        ScrollY = Math.Clamp(ScrollY, 0, MaxScroll);
        _target = Math.Clamp(_target, 0, MaxScroll);
    }

    public void ScrollTo(float y, bool instant = true)
    {
        _target = Math.Clamp(y, 0, MaxScroll);
        if (instant)
            ScrollY = _target;
    }

    private Rectangle Track => new(Bounds.Right - BarWidth, Bounds.Y + 2, BarWidth, Bounds.Height - 4);

    private Rectangle Thumb
    {
        get
        {
            Rectangle track = Track;
            if (MaxScroll <= 0)
                return Rectangle.Empty;
            int h = Math.Max(28, (int)(track.Height * (Bounds.Height / (float)_contentHeight)));
            int y = track.Y + (int)((track.Height - h) * (ScrollY / MaxScroll));
            return new Rectangle(track.X, y, track.Width, h);
        }
    }

    private Point ToContent(Point p) => new(p.X, p.Y + (int)ScrollY);

    public override Node? HitTest(Point p)
    {
        if (!Visible || !Bounds.Contains(p))
            return null;
        if (MaxScroll > 0 && Track.Contains(p))
            return this;
        return Content.HitTest(ToContent(p));
    }

    public override bool Interactive => true;

    public override bool Scroll(Point p, int delta)
    {
        if (!Visible || !Bounds.Contains(p))
            return false;
        if (Content.Scroll(ToContent(p), delta))
            return true;
        if (MaxScroll <= 0)
            return false;
        _target = Math.Clamp(_target - delta * 0.75f, 0, MaxScroll);
        return true;
    }

    public override void Update(Ctx ctx)
    {
        // Smooth scrolling toward the target.
        ScrollY += (_target - ScrollY) * 0.35f;
        if (Math.Abs(_target - ScrollY) < 0.5f)
            ScrollY = _target;

        if (_dragging)
        {
            if (!Main.mouseLeft)
            {
                _dragging = false;
            }
            else
            {
                Rectangle track = Track;
                Rectangle thumb = Thumb;
                float ratio = (ctx.Mouse.Y - _dragOffset - track.Y) / (float)Math.Max(1, track.Height - thumb.Height);
                ScrollTo(ratio * MaxScroll);
            }
        }
        else if (ctx.IsHovered(this) && ctx.LeftPressed && Track.Contains(ctx.Mouse))
        {
            Rectangle thumb = Thumb;
            if (thumb.Contains(ctx.Mouse))
            {
                _dragging = true;
                _dragOffset = ctx.Mouse.Y - thumb.Y;
            }
            else
            {
                ScrollTo(ScrollY + (ctx.Mouse.Y < thumb.Y ? -Bounds.Height : Bounds.Height) * 0.9f, instant: false);
            }
        }

        var inner = new Ctx { Sb = ctx.Sb, Hovered = ctx.Hovered, Mouse = ToContent(ctx.Mouse), MouseInView = Bounds.Contains(ctx.Mouse), LeftPressed = ctx.LeftPressed };
        Content.Update(inner);
    }

    public override void Draw(Ctx ctx)
    {
        Clip.Push(ctx.Sb, Bounds, new Vector2(0, -(int)ScrollY));
        try
        {
            var inner = new Ctx
            {
                Sb = ctx.Sb,
                Hovered = Bounds.Contains(ctx.Mouse) ? ctx.Hovered : null,
                Mouse = ToContent(ctx.Mouse),
                MouseInView = Bounds.Contains(ctx.Mouse),
            };
            Content.Draw(inner);
        }
        finally
        {
            Clip.Pop(ctx.Sb);
        }

        if (MaxScroll > 0)
        {
            Rectangle track = Track;
            Gfx.Panel(ctx.Sb, track, Theme.Slot * 0.8f, null, 4);
            bool hot = _dragging || (ctx.IsHovered(this) && Track.Contains(ctx.Mouse));
            Gfx.Panel(ctx.Sb, Thumb, hot ? Theme.Accent : Theme.BorderStrong, null, 4);
        }
    }
}

/// <summary>Single-line text input. Click to focus; Esc or clicking elsewhere removes focus.</summary>
internal sealed class TextBox : Node
{
    public string Value = "";
    public string Placeholder;
    public Action<string>? OnChange;
    public Action? OnSubmit;
    public int MaxLength = 48;
    public bool Focused;

    public TextBox(string placeholder)
    {
        Placeholder = placeholder;
        OnClick = Focus;
    }

    public void Focus()
    {
        if (Focused)
            return;
        Focused = true;
        Main.clrInput();
        TextBox? previous = Current;
        Current = this;
        if (previous is not null && previous != this)
            previous.Focused = false;
    }

    public void Blur()
    {
        if (!Focused)
            return;
        Focused = false;
        if (Current == this)
            Current = null;
        PlayerInput.WritingText = false;
    }

    /// <summary>The focused text box, if any (at most one).</summary>
    internal static TextBox? Current { get; private set; }

    internal static void BlurAll()
    {
        Current?.Blur();
        Current = null;
    }

    public override int Measure(int width) => Theme.LineHeight(Theme.Body) + 14;

    public override void Update(Ctx ctx)
    {
        if (Focused && ctx.LeftPressed && !Bounds.Contains(ctx.Mouse))
            Blur();
    }

    public override void Draw(Ctx ctx)
    {
        bool hover = ctx.IsHovered(this);
        Gfx.Panel(ctx.Sb, Bounds, Focused ? Theme.SurfaceAlt : Theme.Slot, Focused ? Theme.Accent : hover ? Theme.BorderStrong : Theme.Border);

        if (Focused)
        {
            PlayerInput.WritingText = true;
            Main.instance.HandleIME();
            string input = Main.GetInputText(Value).Replace("\n", "").Replace("\r", "");
            if (input.Length > MaxLength)
                input = input[..MaxLength];
            if (input != Value)
            {
                Value = input;
                OnChange?.Invoke(Value);
            }
            if (Main.inputTextEnter)
                OnSubmit?.Invoke();
        }

        int lh = Theme.LineHeight(Theme.Body);
        int textX = Bounds.X + 30;
        // Magnifier glyph.
        Gfx.Circle(ctx.Sb, new Vector2(Bounds.X + 15, Bounds.Center.Y - 1), 5.5f, Theme.TextFaint);
        Gfx.Circle(ctx.Sb, new Vector2(Bounds.X + 15, Bounds.Center.Y - 1), 3.5f, Focused ? Theme.SurfaceAlt : Theme.Slot);
        Gfx.Line(ctx.Sb, new Vector2(Bounds.X + 19, Bounds.Center.Y + 3), new Vector2(Bounds.X + 23, Bounds.Center.Y + 7), Theme.TextFaint, 2f);

        int clearW = Value.Length > 0 ? 22 : 0;
        float maxW = Bounds.Width - 38 - clearW;
        string shown = Value.Length == 0 ? Placeholder : Value;
        Color color = Value.Length == 0 ? Theme.TextFaint : Theme.Text;
        // Keep the caret end visible for long input.
        if (Value.Length > 0 && Txt.Width(shown, Theme.Body) > maxW)
        {
            int cut = 0;
            while (cut < shown.Length && Txt.Width(shown[cut..], Theme.Body) > maxW)
                cut++;
            shown = shown[cut..];
        }
        else if (Value.Length == 0)
        {
            shown = Txt.Ellipsize(shown, maxW, Theme.Body);
        }
        float y = Bounds.Y + (Bounds.Height - lh) / 2f + 2;
        Txt.DrawString(ctx.Sb, shown, new Vector2(textX, y), color, Theme.Body);
        if (Focused && Main.GlobalTimeWrappedHourly % 1f < 0.55f)
        {
            float caretX = textX + (Value.Length == 0 ? 0 : Txt.Width(shown, Theme.Body)) + 1;
            Gfx.Fill(ctx.Sb, new Rectangle((int)caretX, (int)y, 2, lh - 4), Theme.Accent);
        }

        if (Value.Length > 0)
        {
            var clear = new Rectangle(Bounds.Right - 24, Bounds.Center.Y - 9, 18, 18);
            bool clearHover = hover && clear.Contains(ctx.Mouse);
            Gfx.Line(ctx.Sb, new Vector2(clear.X + 4, clear.Y + 4), new Vector2(clear.Right - 4, clear.Bottom - 4), clearHover ? Theme.Text : Theme.TextFaint, 2f);
            Gfx.Line(ctx.Sb, new Vector2(clear.Right - 4, clear.Y + 4), new Vector2(clear.X + 4, clear.Bottom - 4), clearHover ? Theme.Text : Theme.TextFaint, 2f);
            OnClick = () =>
            {
                if (clear.Contains(GuideUISystem.MouseUi))
                {
                    Value = "";
                    OnChange?.Invoke(Value);
                    Focus();
                }
                else
                {
                    Focus();
                }
            };
        }
        else
        {
            OnClick = Focus;
        }
    }
}

/// <summary>
/// Virtualized grid of item slots for large result sets: only visible rows are drawn and hit-tested.
/// Must be the direct content of a <see cref="ScrollView"/> (uses its scroll/viewport).
/// </summary>
internal sealed class ItemGrid : Node
{
    public IReadOnlyList<int> Items = Array.Empty<int>();
    public int Cell = 44;

    /// <summary>When wider than <see cref="Cell"/>, each cell also shows the item name next to the icon.</summary>
    public int CellWidth;
    public int Gap = 6;
    public ScrollView? Host;
    public Action<int>? OnPick;
    private int _columns = 1;

    private int Width => Math.Max(Cell, CellWidth);

    public override bool Interactive => true;

    public override int Measure(int width)
    {
        _columns = Math.Max(1, (width + Gap) / (Width + Gap));
        int rows = (Items.Count + _columns - 1) / _columns;
        return Math.Max(Cell, rows * (Cell + Gap) - Gap);
    }

    public override void Arrange(Rectangle bounds)
    {
        Bounds = bounds;
        _columns = Math.Max(1, (bounds.Width + Gap) / (Width + Gap));
    }

    private int IndexAt(Point p)
    {
        if (!Bounds.Contains(p))
            return -1;
        int col = (p.X - Bounds.X) / (Width + Gap);
        int row = (p.Y - Bounds.Y) / (Cell + Gap);
        if (col >= _columns || (p.X - Bounds.X) % (Width + Gap) >= Width || (p.Y - Bounds.Y) % (Cell + Gap) >= Cell)
            return -1;
        int index = row * _columns + col;
        return index < Items.Count ? index : -1;
    }

    public override Node? HitTest(Point p)
    {
        int index = IndexAt(p);
        if (index < 0)
            return null;
        TooltipItem = Items[index];
        int type = Items[index];
        OnClick = () => (OnPick ?? Navigator.OpenItem)(type);
        return this;
    }

    public override void Draw(Ctx ctx)
    {
        int top = Host is null ? Bounds.Y : (int)Host.ScrollY + Host.Bounds.Y;
        int bottom = Host is null ? Bounds.Bottom : top + Host.Bounds.Height;
        int firstRow = Math.Max(0, (top - Bounds.Y) / (Cell + Gap));
        int lastRow = Math.Max(0, (bottom - Bounds.Y) / (Cell + Gap) + 1);
        int hoverIndex = ctx.IsHovered(this) ? IndexAt(ctx.Mouse) : -1;
        bool named = CellWidth > Cell;
        for (int row = firstRow; row <= lastRow; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                int index = row * _columns + col;
                if (index >= Items.Count)
                    return;
                var box = new Rectangle(Bounds.X + col * (Width + Gap), Bounds.Y + row * (Cell + Gap), Width, Cell);
                bool hover = index == hoverIndex;
                Gfx.Panel(ctx.Sb, box, hover ? Theme.SurfaceHover : Theme.Slot, hover ? Theme.Accent : Theme.Border, 5);
                var iconBox = new Rectangle(box.X, box.Y, Cell, Cell);
                ItemIcons.Draw(ctx.Sb, Items[index], iconBox.Center.ToVector2(), Cell - 12);
                if (named)
                {
                    float scale = Theme.Small;
                    string name = Txt.Ellipsize(GuideCatalog.ItemName(Items[index]), box.Width - Cell - 12, scale);
                    float y = box.Y + (box.Height - Theme.LineHeight(scale)) / 2f;
                    Txt.DrawString(ctx.Sb, name, new Vector2(box.X + Cell, y), RichText.ItemColor(Items[index]), scale);
                }
                if (Inventory.Owns(Items[index]))
                    Gfx.Circle(ctx.Sb, new Vector2(iconBox.Right - 6, iconBox.Y + 6), 4f, Theme.Success);
            }
        }
    }
}

/// <summary>Toggle chip used for filters.</summary>
internal sealed class Chip : Node, IFlowItem
{
    public string Text;
    public bool On;
    public Color Color = Theme.Accent;
    public int Icon;

    public Chip(string text, bool on, Action onClick, int icon = 0)
    {
        Text = text;
        On = on;
        OnClick = onClick;
        Icon = icon;
    }

    public int PreferredWidth => (int)Math.Ceiling(Txt.Width(Text, Theme.Small)) + 20 + (Icon > 0 ? 22 : 0);

    public override int Measure(int width) => Theme.LineHeight(Theme.Small) + 10;

    public override void Draw(Ctx ctx)
    {
        bool hover = ctx.IsHovered(this);
        Gfx.Panel(ctx.Sb, Bounds, On ? Color * 0.25f : hover ? Theme.SurfaceHover : Theme.Surface, On ? Color : hover ? Theme.BorderStrong : Theme.Border, 8);
        int x = Bounds.X + 10;
        if (Icon > 0)
        {
            ItemIcons.Draw(ctx.Sb, Icon, new Vector2(x + 9, Bounds.Center.Y), 18);
            x += 22;
        }
        Txt.DrawString(ctx.Sb, Txt.Ellipsize(Text, Bounds.Right - x - 6, Theme.Small), new Vector2(x, Bounds.Y + (Bounds.Height - Theme.LineHeight(Theme.Small)) / 2f + 2), On ? Theme.Text : Theme.TextMuted, Theme.Small);
    }
}

/// <summary>Keyboard helpers.</summary>
internal static class Keys2
{
    internal static bool JustPressed(Keys key) => Main.keyState.IsKeyDown(key) && !Main.oldKeyState.IsKeyDown(key);
}
