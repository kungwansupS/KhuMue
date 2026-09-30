using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using KhuMue.Common;
using KhuMue.Core;
using KhuMue.UI;
using KhuMue.UI.Pages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace GuideQaProbe;

public sealed class GuideQaProbe : Mod
{
}

/// <summary>
/// Test-only driver for Better Guide (never shipped). It runs only when the environment variable
/// BG_QA_PLAN names a plan file inside the current save folder, i.e. only in the isolated QA profile made
/// by tools/RunClientQa.ps1. It builds every page offscreen to catch exceptions and missing texts, then
/// opens pages in the real window and saves screenshots of the back buffer.
/// </summary>
public sealed class QaSystem : ModSystem
{
    private const BindingFlags Any = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private JsonElement _plan;
    private string _out = "";
    private bool _active;
    private bool _done;
    private int _ticks;
    private int _shot = -1;
    private int _wait;
    private bool _capture;
    private readonly List<string> _log = new();
    private readonly List<string> _problems = new();
    private readonly SortedDictionary<string, string> _progress = new();

    public override void OnWorldLoad()
    {
        string? path = Environment.GetEnvironmentVariable("BG_QA_PLAN");
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return;
        string full = Path.GetFullPath(path);
        if (!full.StartsWith(Path.GetFullPath(Main.SavePath), StringComparison.OrdinalIgnoreCase))
            return;
        _plan = JsonDocument.Parse(File.ReadAllText(full)).RootElement.Clone();
        _out = Path.GetDirectoryName(full)!;
        _active = true;
        Mod.Logger.Info("GUIDE_QA active, plan " + full);
    }

    public override void PostUpdateEverything()
    {
        if (!_active || _done || Main.gameMenu)
            return;
        Main.LocalPlayer.immune = true;
        Main.LocalPlayer.immuneTime = 60;
        _ticks++;
        _updateNo++;
        // A multiplayer client keeps receiving world data and mod sync packets for a while after joining.
        if (_ticks < (Main.netMode == NetmodeID.MultiplayerClient ? 300 : 60))
            return;

        try
        {
            if (_shot == -1)
            {
                PrepareWorld();
                RecordProgress();
                Sweep("en-US");
                _shot = 0;
                Begin(0);
                return;
            }
            Trace("update");
            if (_trace > 0)
                _trace--;
            if (_wait > 0 && --_wait == 0)
                _capture = true;
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    // ------------------------------------------------------------------ click tracing

    // Updates left to trace after a simulated press: window state at each step of the update, and every
    // Open()/Close() of the guide with its caller, to tell a lost press from a window that closed and reopened.
    private int _trace;
    private int _updateNo;
    private static QaSystem? _instance;
    private static readonly FieldInfo? LeftPressedField = typeof(GuideWindow).GetField("_leftPressed", Any);

    private delegate void OrigUiCall(GuideUISystem self);

    private void Trace(string where)
    {
        if (_trace <= 0)
            return;
        var ui = ModContent.GetInstance<GuideUISystem>();
        object? window = typeof(GuideUISystem).GetField("_window", Any)?.GetValue(ui);
        object? pressed = window is null ? null : LeftPressedField?.GetValue(window);
        _log.Add($"  u{_updateNo} {where}: vis={ui.Visible} pressed={pressed} left={Main.mouseLeft} release={Main.mouseLeftRelease} focus={Main.hasFocus} page={Navigator.Current?.GetType().Name}");
    }

    private static void TraceCall(string name)
    {
        if (_instance is not { _trace: > 0 } qa)
            return;
        var frames = new StackTrace(2, false).GetFrames();
        string callers = string.Join(" < ", frames.Take(4).Select(f => f.GetMethod()?.DeclaringType?.Name + "." + f.GetMethod()?.Name));
        qa._log.Add($"  u{qa._updateNo} {name}() from {callers}");
    }

    public override void PostUpdateInput() => Trace("postinput");

    // Terraria pauses single-player whenever its window loses focus (Main.DoUpdate), which would stall the
    // plan while the tester works in another window. Keep the game "active" and ignore the real mouse instead.
    private static readonly MethodInfo? SetIsActive = typeof(Game).GetProperty(nameof(Game.IsActive))?.GetSetMethod(nonPublic: true);

    // Simulated click of the current shot: 0 none, 1 waiting for the page, 2 button down, 3 released.
    private int _clickStage;
    private int _clickHeld;
    private JsonElement _clickSpec;
    private Point _clickUi;

    /// <summary>
    /// Runs right after the game reads input and before any mod's PostUpdateInput, so the guide receives the
    /// parked cursor and simulated clicks exactly like real mouse input.
    /// </summary>
    private void AfterInput(Terraria.GameInput.On_PlayerInput.orig_UpdateInput orig)
    {
        orig();
        if (!_active || _done || Main.gameMenu)
            return;

        bool down = false;
        if (_clickStage == 1 && _wait <= 12 && TryResolveClick(out _clickUi))
        {
            _clickStage = 2;
            _clickHeld = 0;
            down = true;
            _log.Add($"click: focus={Main.hasFocus} active={Main.instance.IsActive} release={Main.mouseLeftRelease} net={Main.netMode}");
            _trace = 8;
        }
        else if (_clickStage == 2 && ++_clickHeld < 3)
        {
            // Hold the button for a few updates like a real click; a one-update press can fall between the
            // input step and the interface update when the game runs several updates per frame.
            down = true;
        }
        else if (_clickStage == 2)
        {
            _clickStage = 3;
        }

        // The real cursor may sit over the hidden window: its buttons and wheel never reach the game.
        Main.mouseLeft = down;
        Main.mouseRight = Main.mouseMiddle = false;
        Terraria.GameInput.PlayerInput.Triggers.Current.MouseLeft = down;
        Terraria.GameInput.PlayerInput.Triggers.Current.MouseRight = false;
        Terraria.GameInput.PlayerInput.ScrollWheelDelta = 0;
        Terraria.GameInput.PlayerInput.ScrollWheelDeltaForUI = 0;

        // Park the cursor in the bottom-right corner, outside the guide window, so hover effects and tooltip
        // positions do not depend on where the real cursor happens to be; during a click it sits on the target.
        int x = Terraria.GameInput.PlayerInput.RealScreenWidth - 40;
        int y = Terraria.GameInput.PlayerInput.RealScreenHeight - 40;
        if (_clickStage >= 2)
        {
            x = (int)((_clickUi.X + 0.5f) * Main.UIScale);
            y = (int)((_clickUi.Y + 0.5f) * Main.UIScale);
        }
        Terraria.GameInput.PlayerInput.MouseX = x;
        Terraria.GameInput.PlayerInput.MouseY = y;
        Main.mouseX = x;
        Main.mouseY = y;
        Terraria.GameInput.PlayerInput.CacheMousePositionForZoom();
        Trace("input");
    }

    /// <summary>
    /// Click target in UI coordinates: "click": [x, y] relative to the window's top-left corner (negative x
    /// counts from the right edge), or "clickNode": "TypeName" for the first visible node of that type.
    /// </summary>
    private bool TryResolveClick(out Point target)
    {
        target = default;
        object? window = typeof(GuideUISystem).GetField("_window", Any)?.GetValue(ModContent.GetInstance<GuideUISystem>());
        if (window is null || window.GetType().GetField("_window", Any)?.GetValue(window) is not Rectangle rect)
            return false;
        if (_clickSpec.ValueKind == JsonValueKind.Array)
        {
            int dx = _clickSpec[0].GetInt32();
            int dy = _clickSpec[1].GetInt32();
            target = new Point(dx < 0 ? rect.Right + dx : rect.X + dx, rect.Y + dy);
            return true;
        }
        string type = _clickSpec.GetString()!;
        if (window.GetType().GetField("_root", Any)?.GetValue(window) is not Node root)
            return false;
        if (FindNode(root, type, 0, rect) is not Rectangle found)
        {
            _problems.Add($"shot {_plan.GetProperty("shots")[_shot].GetProperty("name").GetString()}: no visible {type} to click");
            _clickStage = 3;
            return false;
        }
        // Near the top-left corner: a button's label area, or the first cell of a grid.
        target = new Point(found.X + Math.Min(20, found.Width / 2), found.Y + Math.Min(20, found.Height / 2));
        return true;
    }

    /// <summary>First node of the type, in window coordinates (scroll views shift their content).</summary>
    private static Rectangle? FindNode(Node node, string type, int shiftY, Rectangle viewport)
    {
        var bounds = new Rectangle(node.Bounds.X, node.Bounds.Y - shiftY, node.Bounds.Width, node.Bounds.Height);
        if (node.GetType().Name == type && viewport.Contains(bounds.Center))
            return bounds;
        if (node is ScrollView scroll)
            return FindNode(scroll.Content, type, shiftY + (int)scroll.ScrollY, Rectangle.Intersect(viewport, scroll.Bounds));
        if (node is Container container)
        {
            foreach (Node child in container.Children)
            {
                if (FindNode(child, type, shiftY, viewport) is Rectangle hit)
                    return hit;
            }
        }
        return null;
    }

    public override void Load()
    {
        if (Main.dedServ)
            return;
        Main.OnPostDraw += AfterDraw;
        Terraria.GameInput.On_PlayerInput.UpdateInput += AfterInput;
        _instance = this;
        foreach (string name in new[] { "Open", "Close" })
        {
            MethodInfo? method = typeof(GuideUISystem).GetMethod(name, Any, Type.EmptyTypes);
            if (method is not null)
                MonoModHooks.Add(method, (OrigUiCall orig, GuideUISystem self) =>
                {
                    TraceCall(name);
                    orig(self);
                });
        }
    }

    public override void Unload()
    {
        Main.OnPostDraw -= AfterDraw;
        Terraria.GameInput.On_PlayerInput.UpdateInput -= AfterInput;
    }

    /// <summary>
    /// Runs once the whole frame is drawn. Screenshots are taken here and not in PostDrawInterface, which
    /// vanilla calls before it draws item tooltips and the cursor.
    /// </summary>
    private void AfterDraw(GameTime gameTime)
    {
        if (!_active || _done)
            return;
        if (!Main.instance.IsActive)
            SetIsActive?.Invoke(Main.instance, new object[] { true });
        if (!_capture)
            return;
        _capture = false;
        try
        {
            JsonElement shots = _plan.GetProperty("shots");
            JsonElement shot = shots[_shot];
            string shotName = shot.GetProperty("name").GetString()!;
            Save(shotName);
            CheckExpectation(shot, shotName);
            SweepThaiOnce();
            _shot++;
            if (_shot >= shots.GetArrayLength())
            {
                Finish();
                return;
            }
            Begin(_shot);
        }
        catch (Exception e)
        {
            Fail(e);
        }
    }

    // ------------------------------------------------------------------ world fixture

    private void PrepareWorld()
    {
        if (!_plan.TryGetProperty("world", out JsonElement world))
            return;
        if (world.TryGetProperty("vanilla", out JsonElement vanilla))
        {
            foreach (JsonProperty p in vanilla.EnumerateObject())
            {
                FieldInfo? f = typeof(NPC).GetField(p.Name, Any) ?? typeof(Main).GetField(p.Name, Any);
                if (f is null)
                    _problems.Add("fixture: unknown vanilla flag " + p.Name);
                else
                    f.SetValue(null, p.Value.GetBoolean());
            }
        }
        if (world.TryGetProperty("calamity", out JsonElement calamity) && ModLoader.TryGetMod("CalamityMod", out Mod cal))
        {
            Type? system = cal.Code.GetType("CalamityMod.DownedBossSystem");
            foreach (JsonProperty p in calamity.EnumerateObject())
            {
                PropertyInfo? prop = system?.GetProperty(p.Name, Any);
                if (prop is null)
                    _problems.Add("fixture: unknown Calamity flag " + p.Name);
                else
                    prop.SetValue(null, p.Value.GetBoolean());
            }
            if (world.TryGetProperty("revengeance", out JsonElement rev))
                cal.Call("SetDifficulty", "revengeancemode", rev.GetBoolean());
        }
        ProgressService.Refresh(force: true);
    }

    /// <summary>
    /// "progress": true records the route state of every entry, so a multiplayer run can be compared with a
    /// single-player run of the same world (flags that are not synced to clients show up as differences).
    /// </summary>
    private void RecordProgress()
    {
        _log.Add("net mode " + Main.netMode);
        if (!_plan.TryGetProperty("progress", out JsonElement want) || !want.GetBoolean())
            return;
        ProgressService.Refresh(force: true);
        foreach (EntryStatus status in ProgressService.Entries)
            _progress[status.Entry.Id] = status.State.ToString();
    }

    // ------------------------------------------------------------------ offscreen sweep

    private void Sweep(string label)
    {
        var watch = Stopwatch.StartNew();
        int pages = 0;
        int[] widths = { 980, 520 };

        void Try(string name, Func<Page> make)
        {
            foreach (int width in widths)
            {
                pages++;
                try
                {
                    Page page = make();
                    Node? header = page.BuildHeader(width);
                    Node body = page.Build(width);
                    foreach (Node node in new[] { header, body })
                    {
                        if (node is null)
                            continue;
                        int h = node.Measure(width);
                        node.Arrange(new Rectangle(0, 0, width, Math.Max(h, 1)));
                        Scan(node, $"{name}@{width}");
                    }
                    _ = page.Title;
                }
                catch (Exception e)
                {
                    _problems.Add($"{label} {name}@{width}: {e.GetType().Name}: {e.Message}");
                    Mod.Logger.Error($"GUIDE_QA page {name} failed", e);
                }
            }
        }

        Try("home", () => new HomePage());
        Try("progression", () => new ProgressionPage());
        Try("items", () => new ItemsPage());
        Try("upgrades", () => new UpgradesPage());
        Try("guides", () => new GuidesPage());
        Try("settings", () => new SettingsPage());
        foreach (ProgressionEntry entry in GuideData.Entries)
            Try("entry:" + entry.Id, () => new EntryPage(entry.Id));
        foreach (GuideArticle article in GuideData.Articles)
            Try("guide:" + article.Id, () => new GuidesPage(article.Id));

        FieldInfo classField = typeof(LoadoutsPage).GetField("_class", Any)!;
        LoadoutSet? set = LoadoutsPage.Set;
        if (set is not null)
        {
            foreach (string cls in PageKit.Classes())
            {
                classField.SetValue(null, cls);
                foreach (string stage in set.Stages)
                    Try($"loadouts:{stage}:{cls}", () => new LoadoutsPage(stage));
            }
            classField.SetValue(null, null);
        }

        // Every item page of the loaded mods (the item pages are the largest surface).
        int step = _plan.TryGetProperty("itemStep", out JsonElement s) ? Math.Max(1, s.GetInt32()) : 1;
        for (int type = 1; type < ItemLoader.ItemCount; type += step)
        {
            int t = type;
            if (Terraria.ID.ContentSamples.ItemsByType.TryGetValue(t, out Item? sample) && sample.type > 0)
                Try("item:" + GuideCatalog.ItemKey(t), () => new ItemPage(t));
        }

        // Every entry needs something to show as its picture.
        foreach (ProgressionEntry entry in GuideData.Entries)
        {
            int npc = PageKit.PortraitNpc(entry);
            int icon = entry.Icon is null ? 0 : GuideCatalog.ItemType(entry.Icon);
            int summon = entry.Summons.Select(GuideCatalog.ItemType).FirstOrDefault(t => t > 0);
            bool head = npc > 0 && npc < Terraria.ModLoader.NPCLoader.NPCCount && Terraria.ID.NPCID.Sets.BossHeadTextures[npc] >= 0;
            bool sprite = false;
            if (npc > 0 && npc < Terraria.ModLoader.NPCLoader.NPCCount)
            {
                Main.instance.LoadNPC(npc);
                var tex = Terraria.GameContent.TextureAssets.Npc[npc].Value;
                sprite = tex.Width >= 8 && tex.Height / Math.Max(1, Main.npcFrameCount[npc]) >= 8;
            }
            if (icon == 0 && !head && !sprite && summon == 0 && (entry.Mod == "Terraria" || ModLoader.HasMod(entry.Mod)))
                _problems.Add($"portrait: {entry.Id} has no usable picture");
        }
        _log.Add("key clashes: " + SettingsPage.KeyClashes());

        _log.Add($"{label}: swept {pages} page builds in {watch.Elapsed.TotalSeconds:0.0}s, problems so far {_problems.Count}");
        Mod.Logger.Info("GUIDE_QA " + _log[^1]);
    }

    /// <summary>Walks a node tree and flags texts that show missing localization or unformatted placeholders.</summary>
    private void Scan(Node node, string where)
    {
        var seen = new HashSet<Node>(ReferenceEqualityComparer.Instance);
        var stack = new Stack<Node>();
        stack.Push(node);
        while (stack.Count > 0)
        {
            Node n = stack.Pop();
            if (!seen.Add(n))
                continue;
            for (Type? t = n.GetType(); t is not null && t != typeof(object); t = t.BaseType)
            {
                foreach (FieldInfo f in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    object? value = f.GetValue(n);
                    switch (value)
                    {
                        case string text:
                            CheckText(text, where, n.GetType().Name + "." + f.Name);
                            break;
                        case Node child:
                            stack.Push(child);
                            break;
                        case IEnumerable<Node> children:
                            foreach (Node c in children)
                                stack.Push(c);
                            break;
                    }
                }
            }
        }
    }

    private void CheckText(string text, string where, string field)
    {
        if (text.Contains("Mods.KhuMue", StringComparison.Ordinal))
            _problems.Add($"missing text at {where} ({field}): {text}");
        else if (System.Text.RegularExpressions.Regex.IsMatch(text, @"\{\d\}"))
            _problems.Add($"unformatted placeholder at {where} ({field}): {text}");
    }

    // ------------------------------------------------------------------ screenshots

    private void Begin(int index)
    {
        JsonElement shot = _plan.GetProperty("shots")[index];
        if (shot.TryGetProperty("lang", out JsonElement lang))
            SetLanguage(lang.GetString()!);
        if (shot.TryGetProperty("textScale", out JsonElement ts) && GuideConfig.Instance is GuideConfig cfg)
            cfg.TextScale = ts.GetSingle();

        var ui = ModContent.GetInstance<GuideUISystem>();
        if (!ui.Visible)
            ui.Open();
        string page = shot.GetProperty("page").GetString()!;
        Navigate(page);
        _wait = shot.TryGetProperty("wait", out JsonElement w) ? w.GetInt32() : 20;
        if (shot.TryGetProperty("scroll", out JsonElement scroll))
            _pendingScroll = scroll.GetSingle();
        _hoverItem = shot.TryGetProperty("hover", out JsonElement hover) ? GuideCatalog.ItemType(hover.GetString()!) : 0;
        _clickStage = 0;
        if (shot.TryGetProperty("click", out JsonElement click) || shot.TryGetProperty("clickNode", out click))
        {
            _clickSpec = click.Clone();
            _clickStage = 1;
        }
    }

    /// <summary>"expect": the page type the guide shows after the shot's click, or "closed".</summary>
    private void CheckExpectation(JsonElement shot, string name)
    {
        if (!shot.TryGetProperty("expect", out JsonElement expect))
            return;
        string want = expect.GetString()!;
        string got = !ModContent.GetInstance<GuideUISystem>().Visible ? "closed" : Navigator.Current?.GetType().Name ?? "none";
        _log.Add($"shot {name}: expected {want}, got {got}");
        if (got != want)
            _problems.Add($"shot {name}: expected {want}, got {got}");
    }

    private float _pendingScroll = -1f;
    private int _hoverItem;

    public override void ModifyInterfaceLayers(List<Terraria.UI.GameInterfaceLayer> layers)
    {
        if (!_active || _hoverItem <= 0)
            return;
        // Show an item tooltip the way hovering an item slot does, to test tooltip drawing with other mods.
        int index = layers.FindIndex(l => l.Name == "Vanilla: Mouse Text");
        layers.Insert(index < 0 ? layers.Count : index, new Terraria.UI.LegacyGameInterfaceLayer("GuideQaProbe: Hover", () =>
        {
            Item item = GuideCatalog.SampleItem(_hoverItem).Clone();
            Main.HoverItem = item;
            Main.hoverItemName = item.Name;
            return true;
        }, Terraria.UI.InterfaceScaleType.UI));
    }

    public override void UpdateUI(GameTime gameTime)
    {
        Trace("ui");
        // Wait until the window has rebuilt for the new page before scrolling it.
        if (!_active || _pendingScroll < 0f || _wait > 12)
            return;
        // Scroll once the page has been built by the window.
        object? window = typeof(GuideUISystem).GetField("_window", Any)?.GetValue(ModContent.GetInstance<GuideUISystem>());
        object? view = window?.GetType().GetField("_scroll", Any)?.GetValue(window);
        if (view is ScrollView sv)
        {
            sv.ScrollTo(_pendingScroll);
            _pendingScroll = -1f;
        }
    }

    private static void Navigate(string page)
    {
        string[] parts = page.Split(':', 2);
        switch (parts[0])
        {
            case "home": Navigator.OpenSection(Section.Home); break;
            case "progression": Navigator.OpenSection(Section.Progression); break;
            case "upgrades": Navigator.OpenSection(Section.Upgrades); break;
            case "guides": Navigator.OpenSection(Section.Guides); break;
            case "settings": Navigator.OpenSection(Section.Settings); break;
            case "guide": Navigator.Go(new GuidesPage(parts[1])); break;
            case "entry": Navigator.OpenEntry(parts[1]); break;
            case "item": Navigator.OpenItem(GuideCatalog.ItemType(parts[1])); break;
            case "items":
                typeof(ItemsPage).GetField("_query", Any)!.SetValue(null, parts.Length > 1 ? parts[1] : "");
                Navigator.Go(new ItemsPage());
                break;
            case "loadouts":
            {
                // loadouts[:stage[:class]]
                string[] a = page.Split(':');
                typeof(LoadoutsPage).GetField("_class", Any)!.SetValue(null, a.Length > 2 ? a[2] : null);
                Navigator.Go(new LoadoutsPage(a.Length > 1 && a[1].Length > 0 ? a[1] : null));
                break;
            }
            default: throw new ArgumentException("unknown page " + page);
        }
    }

    private void SetLanguage(string culture)
    {
        if (Language.ActiveCulture.Name == culture)
            return;
        if (culture == "th-TH")
        {
            Mod thai = ModLoader.GetMod("ThaiOfflineAllMods");
            object runtime = thai.Code.GetType("ThaiOfflineAllMods.ThaiOfflineAllMods")!.GetProperty("Runtime", Any)!.GetValue(null)!;
            runtime.GetType().GetMethod("SetEnabled", Any)!.Invoke(runtime, new object[] { true });
        }
        else if (ModLoader.TryGetMod("ThaiOfflineAllMods", out Mod thai))
        {
            object? runtime = thai.Code.GetType("ThaiOfflineAllMods.ThaiOfflineAllMods")!.GetProperty("Runtime", Any)!.GetValue(null);
            runtime?.GetType().GetMethod("SetEnabled", Any)!.Invoke(runtime, new object[] { false });
        }
        if (Language.ActiveCulture.Name != culture)
            LanguageManager.Instance.SetLanguage(GameCulture.FromName(culture));
        _log.Add("language -> " + Language.ActiveCulture.Name);
    }

    private void Save(string name)
    {
        var device = Main.instance.GraphicsDevice;
        int w = device.PresentationParameters.BackBufferWidth;
        int h = device.PresentationParameters.BackBufferHeight;
        var pixels = new Color[w * h];
        device.GetBackBufferData(pixels);
        using var texture = new Texture2D(device, w, h);
        texture.SetData(pixels);
        using FileStream stream = File.Create(Path.Combine(_out, "shot-" + name + ".png"));
        texture.SaveAsPng(stream, w, h);
        _log.Add($"shot {name}: {Navigator.Current?.GetType().Name} '{Navigator.Current?.Title}' lang={Language.ActiveCulture.Name}");
    }

    private bool _thaiSwept;

    /// <summary>Sweeps every page in Thai once, after the first screenshot taken in Thai (the language has settled).</summary>
    private void SweepThaiOnce()
    {
        if (_thaiSwept || Language.ActiveCulture.Name != "th-TH")
            return;
        if (!_plan.TryGetProperty("sweepThai", out JsonElement thai) || !thai.GetBoolean())
            return;
        _thaiSwept = true;
        Sweep("th-TH");
    }

    private void Finish()
    {
        _done = true;
        SweepThaiOnce();
        var report = new { status = _problems.Count == 0 ? "PASS" : "PROBLEMS", netMode = Main.netMode, problems = _problems, log = _log, progress = _progress };
        File.WriteAllText(Path.Combine(_out, "qa-report.json"), JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        Mod.Logger.Info("GUIDE_QA done, problems " + _problems.Count);
        if (_plan.TryGetProperty("exitWhenDone", out JsonElement exit) && exit.GetBoolean())
            Main.instance.Exit();
    }

    private void Fail(Exception e)
    {
        _done = true;
        Mod.Logger.Error("GUIDE_QA failed", e);
        _problems.Add("QA driver failed: " + e);
        File.WriteAllText(Path.Combine(_out, "qa-report.json"), JsonSerializer.Serialize(new { status = "FAILED", problems = _problems, log = _log }, new JsonSerializerOptions { WriteIndented = true }));
        if (_plan.TryGetProperty("exitWhenDone", out JsonElement exit) && exit.GetBoolean())
            Main.instance.Exit();
    }
}
