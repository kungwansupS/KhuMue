using System.Reflection;
using KhuMue.Common;
using KhuMue.Core;
using KhuMue.UI.Pages;
using Terraria.GameInput;
using Terraria.UI;

namespace KhuMue.UI;

/// <summary>Owns the guide window and plugs it into the game's interface layers.</summary>
public sealed class GuideUISystem : ModSystem
{
    private GuideWindow? _window;
    private bool _visible;
    private int _autoOpenDelay = -1;

    internal bool Visible => _visible;

    /// <summary>
    /// Mouse position in UI coordinates. Read from the raw input instead of Main.mouseX, whose space depends
    /// on the zoom context the caller runs in (UI, world or unscaled).
    /// </summary>
    internal static Point MouseUi => new((int)(PlayerInput.MouseX / Main.UIScale), (int)(PlayerInput.MouseY / Main.UIScale));

    /// <summary>Screen size in UI coordinates, whatever the current zoom context.</summary>
    internal static Vector2 ScreenUi => new(PlayerInput.RealScreenWidth / Main.UIScale, PlayerInput.RealScreenHeight / Main.UIScale);

    private const string LayerName = "KhuMue: Guide";

    private delegate void OrigModifyInterfaceLayers(List<GameInterfaceLayer> layers);

    public override void Load()
    {
        if (Main.dedServ)
            return;
        _window = new GuideWindow();

        // Other mods add HUD layers (Calamity's Rage/Adrenaline bars, stealth meter ...) after ours, and those
        // would be drawn over the open window. After every mod has edited the list, move the guide to just
        // before the vanilla mouse text, so only tooltips and the cursor are drawn above it.
        MethodInfo? target = typeof(SystemLoader).GetMethod(nameof(SystemLoader.ModifyInterfaceLayers), BindingFlags.Public | BindingFlags.Static);
        if (target is not null)
        {
            MonoModHooks.Add(target, (OrigModifyInterfaceLayers orig, List<GameInterfaceLayer> layers) =>
            {
                orig(layers);
                KeepGuideOnTop(layers);
            });
        }
    }

    private static void KeepGuideOnTop(List<GameInterfaceLayer> layers)
    {
        int ours = layers.FindIndex(layer => layer.Name == LayerName);
        if (ours < 0)
            return;
        GameInterfaceLayer guide = layers[ours];
        layers.RemoveAt(ours);
        int mouseText = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        layers.Insert(mouseText >= 0 ? mouseText : layers.Count, guide);

        // Vanilla draws chat after the mouse text, so long chat lines (mods post notices when a world is entered)
        // covered the window's buttons. While the chat box is closed, draw the chat under the guide instead.
        if (!Main.drawingPlayerChat)
        {
            int chat = layers.FindIndex(layer => layer.Name == "Vanilla: Player Chat");
            int at = layers.IndexOf(guide);
            if (chat > at)
            {
                GameInterfaceLayer chatLayer = layers[chat];
                layers.RemoveAt(chat);
                layers.Insert(at, chatLayer);
            }
        }
    }

    public override void Unload()
    {
        _window = null;
        _visible = false;
        Navigator.Reset();
        TextBox.BlurAll();
        Gfx.Unload();
        Txt.ClearCaches();
        RichText.ClearCache();
    }

    public override void PostSetupRecipes()
    {
        // Recipes are final here; the item index is built lazily the first time it is needed.
        ProgressService.Invalidate();
    }

    public override void OnLocalizationsLoaded()
    {
        Txt.ClearCaches();
        RichText.ClearCache();
        GuideCatalog.OnLanguageChanged();
        ItemSearch.Invalidate();
        if (Navigator.Current is Page page)
            page.Dirty = true;
    }

    public override void OnWorldLoad()
    {
        ProgressService.Invalidate();
        Inventory.Invalidate();
    }

    public override void OnWorldUnload()
    {
        Close();
        Navigator.Reset();
        ProgressService.Invalidate();
    }

    internal void Toggle()
    {
        if (_visible)
            Close();
        else
            Open();
    }

    internal void Open()
    {
        if (Main.dedServ || Main.gameMenu || _window is null)
            return;
        try
        {
            GuideCatalog.EnsureBuilt();
            ProgressService.Refresh(force: true);
            Inventory.Invalidate();
            if (Navigator.Current is null)
                Navigator.OpenSection(Section.Home);
            else
                Navigator.Current.Dirty = true;
        }
        catch (Exception e)
        {
            // The guide must never take the game down; the page area shows an error message instead.
            KhuMue.Instance.Logger.Error("Opening the guide failed.", e);
        }
        _visible = true;
        Sounds.Open();
        if (Main.LocalPlayer.TryGetModPlayer(out GuidePlayer guidePlayer))
            guidePlayer.HasOpenedGuide = true;
    }

    private int _drawFailures;

    /// <summary>Draws the window; repeated failures close the guide instead of crashing the game.</summary>
    private void DrawSafely()
    {
        try
        {
            _window!.Draw(Main.spriteBatch);
            _drawFailures = 0;
        }
        catch (Exception e)
        {
            if (_drawFailures++ == 0)
                KhuMue.Instance.Logger.Error("Drawing the guide failed.", e);
            try
            {
                Clip.Reset(Main.spriteBatch);
            }
            catch
            {
            }
            if (_drawFailures > 3)
            {
                Close();
                Navigator.Reset();
            }
        }
    }

    public override void PostUpdateInput()
    {
        KhuMue.MigrateOldKeybind();
        // This frame's input was just read. Presses are taken here, before the game draws its interface:
        // later in the frame vanilla clears Main.mouseLeftRelease, and slots under the window would react to
        // the same click (see GuideWindow.CaptureInput).
        if (_visible && _window is not null && !Main.gameMenu)
            _window.CaptureInput();
    }

    internal void Close()
    {
        if (!_visible)
            return;
        _visible = false;
        TextBox.BlurAll();
        PlayerInput.WritingText = false;
        Sounds.Close();
    }

    internal void OpenItem(int type)
    {
        Open();
        if (_visible)
            Navigator.OpenItem(type);
    }

    internal void OpenEntry(string id)
    {
        Open();
        if (_visible)
            Navigator.OpenEntry(id);
    }

    /// <summary>Opens the guide once for a brand-new character, a few seconds after spawning.</summary>
    internal void ScheduleFirstOpen() => _autoOpenDelay = 180;

    public override void UpdateUI(GameTime gameTime)
    {
        if (Main.gameMenu)
        {
            if (_visible)
                Close();
            return;
        }

        if (_autoOpenDelay > 0 && --_autoOpenDelay == 0 && !_visible)
            Open();

        if (!_visible || _window is null)
            return;
        try
        {
            _window.Update();
        }
        catch (Exception e)
        {
            KhuMue.Instance.Logger.Error("Guide update failed.", e);
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        if (!_visible || _window is null)
            return;
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        if (index < 0)
            index = layers.Count;
        layers.Insert(index, new LegacyGameInterfaceLayer(
            LayerName,
            () =>
            {
                if (_visible && _window is not null)
                    DrawSafely();
                return true;
            },
            InterfaceScaleType.UI));
    }
}
