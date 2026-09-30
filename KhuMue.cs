using KhuMue.Core;
using KhuMue.UI;
using Terraria.GameInput;

namespace KhuMue;

public sealed class KhuMue : Mod
{
    internal static KhuMue Instance { get; private set; } = null!;

    /// <summary>
    /// "KhuMue/ToggleGuide". Its default (B; vanilla also uses B for Quick Buff) is only applied when the player
    /// resets their controls: tModLoader leaves new mod keys unbound. Releases before 2.1 were called
    /// "BetterGuide"; <see cref="MigrateOldKeybind"/> carries their binding over.
    /// </summary>
    internal static ModKeybind? ToggleGuideKeybind { get; private set; }

    internal const string ToggleKeybindName = "ToggleGuide";

    public override void Load()
    {
        Instance = this;
        if (!Main.dedServ)
            ToggleGuideKeybind = KeybindLoader.RegisterKeybind(this, ToggleKeybindName, "B");
    }

    /// <summary>
    /// Keys bound to the guide. tModLoader throws when the input profile has no entry for the keybind yet
    /// (a fresh profile before its first save), so this never throws and returns an empty list then.
    /// </summary>
    internal static List<string> AssignedKeys()
    {
        // ModKeybind.GetAssignedKeys throws while the input profile has no entry for the key yet
        // (a fresh profile), so read the profile directly.
        try
        {
            string id = Instance.Name + "/" + ToggleKeybindName;
            return PlayerInput.CurrentProfile?.InputModes[InputMode.Keyboard].KeyStatus.TryGetValue(id, out List<string>? keys) == true
                ? new List<string>(keys)
                : new List<string>();
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }

    /// <summary>The key(s) bound to the guide for help texts, or how to open it while no key is bound.</summary>
    internal static string ToggleKeyText()
    {
        List<string> keys = AssignedKeys();
        return keys.Count > 0 ? string.Join(" / ", keys) : UI.Pages.PageKit.L("UI.NoKey");
    }

    private static bool _keyMigrationDone;

    /// <summary>
    /// Once, as soon as tModLoader has loaded the input profile with this mod's keybind: while the guide key is
    /// unbound and the profile still holds the binding of the old internal name "BetterGuide", use that binding.
    /// </summary>
    internal static void MigrateOldKeybind()
    {
        if (_keyMigrationDone || Main.dedServ || Instance is null)
            return;
        try
        {
            KeyConfiguration? keyboard = PlayerInput.CurrentProfile?.InputModes[InputMode.Keyboard];
            string id = Instance.Name + "/" + ToggleKeybindName;
            if (keyboard is null || !keyboard.KeyStatus.TryGetValue(id, out List<string>? keys))
                return; // Profile not reloaded with this keybind yet: try again next frame.
            _keyMigrationDone = true;
            if (keys.Count > 0)
                return;
            if (!keyboard.UnloadedModKeyStatus.TryGetValue("BetterGuide/" + ToggleKeybindName, out List<string>? old) || old.Count == 0)
                return;
            keys.AddRange(old);
            PlayerInput.Save();
            Instance.Logger.Info("Guide key taken over from the old internal name BetterGuide: " + string.Join(", ", old));
        }
        catch (Exception e)
        {
            _keyMigrationDone = true;
            Instance.Logger.Warn("Taking over the old guide key failed: " + e.Message);
        }
    }

    public override void PostSetupContent()
    {
        GuideData.Load(this);
        Flags.Initialize();
    }

    public override void Unload()
    {
        GuideCatalog.Unload();
        Flags.Unload();
        GuideData.Unload();
        UpgradeService.Unload();
        ToggleGuideKeybind = null;
        Instance = null!;
    }

    /// <summary>
    /// Small cross-mod API.
    /// "OpenGuide" opens the guide, "OpenItem" (int itemType) opens an item page,
    /// "GetApiVersion" returns 1.
    /// </summary>
    public override object Call(params object[] args)
    {
        if (args.Length == 0 || args[0] is not string command)
            return new ArgumentException("KhuMue.Call needs a command string.");

        switch (command)
        {
            case "GetApiVersion":
                return 1;
            case "OpenGuide":
                if (Main.dedServ)
                    return false;
                ModContent.GetInstance<GuideUISystem>().Open();
                return true;
            case "OpenItem":
                if (Main.dedServ || args.Length < 2 || args[1] is not int itemType)
                    return false;
                ModContent.GetInstance<GuideUISystem>().OpenItem(itemType);
                return true;
            default:
                return new ArgumentException($"Unknown KhuMue.Call command '{command}'.");
        }
    }
}
