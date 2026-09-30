using KhuMue.Common;
using KhuMue.Core;
using KhuMue.UI.Pages;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace KhuMue.UI;

/// <summary>The guide window: sidebar, header with history buttons, and the scrolling page area.</summary>
internal sealed class GuideWindow
{
    internal static float CurrentScroll => _instance?._scroll?.ScrollY ?? 0f;
    private static GuideWindow? _instance;

    private Rectangle _window;
    private Rectangle _sidebar;
    private Rectangle _header;
    private Rectangle _content;
    private bool _compactSidebar;

    private Container? _root;
    private ScrollView? _scroll;
    private Page? _builtPage;
    private Page? _drawFailedPage;
    private (int W, int H, float Text, float Scale, string Culture) _builtFor;
    private Node? _hovered;
    private Node? _pressed;

    internal Rectangle Bounds => _window;

    internal GuideWindow()
    {
        _instance = this;
    }

    private static readonly (Section Section, int Icon, string Key)[] SidebarItems =
    {
        (Section.Home, ItemID.Compass, "Home"),
        (Section.Progression, ItemID.SuspiciousLookingEye, "Progression"),
        (Section.Loadouts, ItemID.WarriorEmblem, "Loadouts"),
        (Section.Items, ItemID.Chest, "Items"),
        (Section.Upgrades, ItemID.LifeCrystal, "Upgrades"),
        (Section.Guides, ItemID.Book, "Guides"),
        (Section.Settings, ItemID.Cog, "Settings"),
    };

    internal static string T(string key) => Language.GetTextValue(Pages.PageKit.Root + "UI." + key);

    private void ComputeLayout()
    {
        Vector2 screen = GuideUISystem.ScreenUi;
        float uiW = screen.X;
        float uiH = screen.Y;
        float scale = Math.Clamp(GuideConfig.Instance?.WindowSize ?? 1f, 0.6f, 1.2f);
        int w = (int)Math.Min(uiW - 24, Math.Max(640, Math.Min(1280, uiW * 0.88f) * scale));
        int h = (int)Math.Min(uiH - 24, Math.Max(420, Math.Min(840, uiH * 0.88f) * scale));
        _window = new Rectangle((int)(uiW - w) / 2, (int)(uiH - h) / 2, w, h);
        _compactSidebar = w < 900;
        int sidebarW = _compactSidebar ? 60 : 184;
        _sidebar = new Rectangle(_window.X, _window.Y, sidebarW, _window.Height);
        _header = new Rectangle(_sidebar.Right, _window.Y, _window.Width - sidebarW, 54);
        _content = new Rectangle(_sidebar.Right + 16, _header.Bottom + 10, _window.Width - sidebarW - 26, _window.Height - _header.Height - 20);
    }

    private void RebuildIfNeeded()
    {
        ComputeLayout();
        Page? page = Navigator.Current;
        if (page is null)
        {
            Navigator.OpenSection(Section.Home);
            page = Navigator.Current!;
        }

        var key = (_window.Width, _window.Height, Theme.TextScale, Main.UIScale, Language.ActiveCulture.Name);
        if (!page.Dirty && ReferenceEquals(page, _builtPage) && key == _builtFor && _root is not null)
            return;

        bool samePage = ReferenceEquals(page, _builtPage);
        float keepScroll = page.ScrollToTop ? 0f : samePage ? (_scroll?.ScrollY ?? 0f) : page.SavedScroll;
        page.ScrollToTop = false;
        _builtFor = key;
        _builtPage = page;
        page.Dirty = false;

        var root = new Chrome();
        Node body;
        Node? pinned = null;
        try
        {
            pinned = page.BuildHeader(_content.Width - 14);
            body = page.Build(_content.Width - 14);
        }
        catch (Exception e)
        {
            KhuMue.Instance.Logger.Error($"Building page {page.GetType().Name} failed.", e);
            body = new Paragraph(T("PageError"), Theme.Danger);
        }
        Rectangle scrollArea = _content;
        if (pinned is not null)
        {
            int h = pinned.Measure(_content.Width - 14);
            pinned.Arrange(new Rectangle(_content.X, _content.Y, _content.Width - 14, h));
            root.Add(pinned);
            scrollArea = new Rectangle(_content.X, _content.Y + h + 10, _content.Width, _content.Height - h - 10);
        }
        _scroll = new ScrollView(body);
        _scroll.Arrange(scrollArea);
        _scroll.ScrollTo(keepScroll);
        if (body is ItemGrid grid)
            grid.Host = _scroll;
        else if (body is Container container)
            AttachGrids(container, _scroll);
        root.Add(_scroll);
        AddChrome(root, page);
        root.Arrange(_window);
        _root = root;
    }

    private static void AttachGrids(Container container, ScrollView host)
    {
        foreach (Node child in container.Children)
        {
            if (child is ItemGrid grid)
                grid.Host = host;
            else if (child is Container inner)
                AttachGrids(inner, host);
            else if (child is Card { Children.Count: > 0 } card && card.Children[0] is Container cardInner)
                AttachGrids(cardInner, host);
        }
    }

    /// <summary>Invisible container covering the window; also blocks clicks from reaching the world.</summary>
    private sealed class Chrome : Container
    {
        public override int Measure(int width) => 0;

        public override void Arrange(Rectangle bounds) => Bounds = bounds;
    }

    private void AddChrome(Container root, Page page)
    {
        // Sidebar navigation.
        int y = _sidebar.Y + 62;
        int itemH = 44;
        foreach ((Section section, int icon, string key) in SidebarItems)
        {
            var nav = new SidebarButton(T("Tabs." + key), icon, page.Section == section, _compactSidebar)
            {
                OnClick = () =>
                {
                    Sounds.Click();
                    Navigator.OpenSection(section);
                },
            };
            if (_compactSidebar)
                nav.Tooltip = () => T("Tabs." + key);
            nav.Arrange(new Rectangle(_sidebar.X + 8, y, _sidebar.Width - 16, itemH));
            root.Add(nav);
            y += itemH + 4;
        }

        // Header: back, forward, title, close.
        int hy = _header.Y + (_header.Height - 32) / 2;
        var back = new IconButton(IconButton.Glyph.Back, Navigator.Back) { Enabled = Navigator.CanGoBack, Tooltip = () => T("Back") };
        back.Arrange(new Rectangle(_header.X + 12, hy, 32, 32));
        root.Add(back);
        var forward = new IconButton(IconButton.Glyph.Forward, Navigator.Forward) { Enabled = Navigator.CanGoForward, Tooltip = () => T("Forward") };
        forward.Arrange(new Rectangle(_header.X + 48, hy, 32, 32));
        root.Add(forward);
        var close = new IconButton(IconButton.Glyph.Close, () => ModContent.GetInstance<GuideUISystem>().Close()) { Tooltip = () => T("Close") };
        close.Arrange(new Rectangle(_header.Right - 44, hy, 32, 32));
        root.Add(close);

        var title = new Label(page.Title, Theme.Text, Theme.Title);
        title.Arrange(new Rectangle(_header.X + 92, _header.Y + (_header.Height - Theme.LineHeight(Theme.Title)) / 2, _header.Width - 150, Theme.LineHeight(Theme.Title)));
        root.Add(title);
    }

    // ------------------------------------------------------------------ update & draw

    private bool _leftPressed;
    private bool _rightPressed;

    /// <summary>
    /// Called right after the game reads input (ModSystem.PostUpdateInput). UpdateUI runs before that step
    /// and only sees a press once Main.mouseLeftRelease is already cleared, so presses are recorded here and
    /// handled by the next <see cref="Update"/>. A press over the window is also consumed, so inventory
    /// slots or other interfaces drawn under the window do not react to it.
    /// </summary>
    internal void CaptureInput()
    {
        bool over = _window.Contains(GuideUISystem.MouseUi);
        if (over)
            Main.LocalPlayer.mouseInterface = true;
        if (Main.mouseLeft && Main.mouseLeftRelease)
        {
            _leftPressed = true;
            if (over)
                Main.mouseLeftRelease = false;
        }
        if (Main.mouseRight && Main.mouseRightRelease)
        {
            _rightPressed = true;
            if (over)
                Main.mouseRightRelease = false;
        }
    }

    internal void Update()
    {
        RebuildIfNeeded();
        Navigator.Current?.Tick();
        bool leftPressed = _leftPressed;
        bool rightPressed = _rightPressed;
        _leftPressed = _rightPressed = false;
        if (_root is null)
            return;

        Point mouse = GuideUISystem.MouseUi;
        bool overWindow = _window.Contains(mouse);
        _hovered = Main.hasFocus && overWindow ? _root.HitTest(mouse) : null;

        var ctx = new Ctx { Hovered = _hovered, Mouse = mouse, LeftPressed = leftPressed };
        _root.Update(ctx);

        if (overWindow)
        {
            Main.LocalPlayer.mouseInterface = true;
            PlayerInput.LockVanillaMouseScroll("KhuMue/Window");
            int wheel = PlayerInput.ScrollWheelDeltaForUI;
            if (wheel != 0)
                _root.Scroll(mouse, wheel);
        }

        // Clicks fire on press, like vanilla buttons.
        if (leftPressed && overWindow && Main.hasFocus)
        {
            _pressed = _hovered;
            _hovered?.OnClick?.Invoke();
        }
        if (rightPressed && overWindow && _hovered?.OnRightClick is Action right)
            right();
        if (Main.mouseXButton1 && overWindow && !_mouseBackHeld)
            Navigator.Back();
        _mouseBackHeld = Main.mouseXButton1;

        HandleKeys();
    }

    private bool _mouseBackHeld;

    private static void HandleKeys()
    {
        if (Keys2.JustPressed(Keys.Escape))
        {
            Main.LocalPlayer.releaseInventory = false;
            if (TextBox.Current is not null)
            {
                TextBox.BlurAll();
            }
            else
            {
                ModContent.GetInstance<GuideUISystem>().Close();
            }
        }
    }

    internal void Draw(SpriteBatch sb)
    {
        RebuildIfNeeded();
        if (_root is null)
            return;

        Point mouse = GuideUISystem.MouseUi;
        if (_window.Contains(mouse))
            Main.LocalPlayer.mouseInterface = true;

        // Backdrop + window.
        Vector2 screen = GuideUISystem.ScreenUi;
        Gfx.Fill(sb, new Rectangle(0, 0, (int)screen.X + 2, (int)screen.Y + 2), Theme.Backdrop);
        Gfx.Shadow(sb, _window, 16, 0.5f);
        Gfx.Panel(sb, _window, Theme.Window, Theme.Border, 8);
        Gfx.Panel(sb, _sidebar, Theme.Sidebar, null, 8);
        Gfx.Fill(sb, new Rectangle(_sidebar.Right - 8, _sidebar.Y, 8, _sidebar.Height), Theme.Sidebar);
        Gfx.Fill(sb, new Rectangle(_sidebar.Right, _sidebar.Y + 8, 1, _sidebar.Height - 16), Theme.Border);
        Gfx.Fill(sb, new Rectangle(_header.X + 1, _header.Bottom, _header.Width - 10, 1), Theme.Border);

        // Brand.
        var brand = new Rectangle(_sidebar.X + 10, _sidebar.Y + 12, _sidebar.Width - 20, 36);
        ItemIcons.Draw(sb, ItemID.Book, new Vector2(brand.X + 16, brand.Center.Y), 28);
        if (!_compactSidebar)
        {
            Txt.DrawString(sb, T("Brand"), new Vector2(brand.X + 38, brand.Y + 2), Theme.Gold, Theme.Body * 1.05f);
            float taglineScale = Theme.Small * 0.9f;
            string tagline = Txt.Ellipsize(T("Tagline"), brand.Width - 40, taglineScale);
            Txt.DrawString(sb, tagline, new Vector2(brand.X + 38, brand.Y + 20), Theme.TextFaint, taglineScale);
        }

        var ctx = new Ctx { Sb = sb, Hovered = _hovered, Mouse = mouse };
        try
        {
            _root.Draw(ctx);
        }
        catch (Exception e)
        {
            Clip.Reset(sb);
            // Log once per built page: a broken node would otherwise log every frame.
            if (!ReferenceEquals(_drawFailedPage, _builtPage))
            {
                _drawFailedPage = _builtPage;
                KhuMue.Instance.Logger.Error("Guide draw failed.", e);
            }
            if (Navigator.Current is not null)
                Navigator.Current.Dirty = true;
        }

        // Footer version text in the sidebar.
        if (!_compactSidebar)
        {
            string version = "v" + KhuMue.Instance.Version;
            Txt.DrawString(sb, version, new Vector2(_sidebar.X + 16, _sidebar.Bottom - 28), Theme.TextFaint, Theme.Small);
        }

        DrawTooltip();
    }

    private void DrawTooltip()
    {
        if (_hovered is null)
            return;
        if (_hovered.TooltipItem > 0)
        {
            Item item = GuideCatalog.SampleItem(_hovered.TooltipItem).Clone();
            Main.HoverItem = item;
            Main.hoverItemName = item.Name;
            return;
        }
        string? text = _hovered.Tooltip?.Invoke();
        if (!string.IsNullOrWhiteSpace(text))
            Main.instance.MouseText(text);
    }
}

/// <summary>Sidebar navigation entry.</summary>
internal sealed class SidebarButton(string text, int icon, bool selected, bool compact) : Node
{
    public override int Measure(int width) => 44;

    public override void Draw(Ctx ctx)
    {
        bool hover = ctx.IsHovered(this);
        if (selected || hover)
            Gfx.Panel(ctx.Sb, Bounds, selected ? Theme.SurfaceAlt : Theme.Surface, selected ? Theme.Border : null, 6);
        if (selected)
            Gfx.Panel(ctx.Sb, new Rectangle(Bounds.X, Bounds.Y + 8, 4, Bounds.Height - 16), Theme.Accent, null, 2);
        int iconX = compact ? Bounds.Center.X : Bounds.X + 24;
        ItemIcons.Draw(ctx.Sb, icon, new Vector2(iconX, Bounds.Center.Y), 28);
        if (!compact)
        {
            int lh = Theme.LineHeight(Theme.Body);
            Txt.DrawString(ctx.Sb, Txt.Ellipsize(text, Bounds.Width - 52, Theme.Body), new Vector2(Bounds.X + 46, Bounds.Y + (Bounds.Height - lh) / 2f + 2), selected ? Theme.Text : Theme.TextMuted, Theme.Body);
        }
    }
}

/// <summary>Square header button drawn with simple vector glyphs.</summary>
internal sealed class IconButton : Node
{
    internal enum Glyph
    {
        Back,
        Forward,
        Close,
    }

    private readonly Glyph _glyph;
    public bool Enabled = true;

    public IconButton(Glyph glyph, Action onClick)
    {
        _glyph = glyph;
        OnClick = onClick;
    }

    public override bool Interactive => Enabled || Tooltip is not null;

    public override Node? HitTest(Point p) => Visible && Bounds.Contains(p) ? this : null;

    public override int Measure(int width) => 32;

    public override void Draw(Ctx ctx)
    {
        bool hover = Enabled && ctx.IsHovered(this);
        Gfx.Panel(ctx.Sb, Bounds, hover ? (_glyph == Glyph.Close ? Theme.Danger * 0.5f : Theme.SurfaceHover) : Theme.Surface, Theme.Border, 6);
        Color c = Enabled ? (hover ? Theme.Text : Theme.TextMuted) : Theme.TextFaint * 0.6f;
        Vector2 m = Bounds.Center.ToVector2();
        switch (_glyph)
        {
            case Glyph.Back:
                Gfx.Line(ctx.Sb, m + new Vector2(4, -7), m + new Vector2(-4, 0), c, 2.5f);
                Gfx.Line(ctx.Sb, m + new Vector2(-4, 0), m + new Vector2(4, 7), c, 2.5f);
                break;
            case Glyph.Forward:
                Gfx.Line(ctx.Sb, m + new Vector2(-4, -7), m + new Vector2(4, 0), c, 2.5f);
                Gfx.Line(ctx.Sb, m + new Vector2(4, 0), m + new Vector2(-4, 7), c, 2.5f);
                break;
            case Glyph.Close:
                Gfx.Line(ctx.Sb, m + new Vector2(-6, -6), m + new Vector2(6, 6), c, 2.5f);
                Gfx.Line(ctx.Sb, m + new Vector2(6, -6), m + new Vector2(-6, 6), c, 2.5f);
                break;
        }
    }
}
