using KhuMue.UI;
using Terraria.GameInput;
using Terraria.ModLoader.IO;

namespace KhuMue.Common;

/// <summary>Per-character guide state: preferred class, whether the guide was ever opened, key clash notice.</summary>
public sealed class GuidePlayer : ModPlayer
{
    /// <summary>melee, ranged, magic, summon or rogue; empty = detect from held weapon.</summary>
    public string PreferredClass = "";

    public bool HasOpenedGuide;

    /// <summary>The key clash this character was last told about ("" = none).</summary>
    public string KeyClashSeen = "";

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (KhuMue.ToggleGuideKeybind?.JustPressed == true && TextBox.Current is null)
            ModContent.GetInstance<GuideUISystem>().Toggle();
    }

    public override void OnEnterWorld()
    {
        if (Player.whoAmI != Main.myPlayer)
            return;
        if (!HasOpenedGuide && GuideConfig.Instance?.OpenForNewCharacters == true)
            ModContent.GetInstance<GuideUISystem>().ScheduleFirstOpen();

        // Say once per character when no key opens the guide, or when the key also triggers something else
        // (B = Quick Buff by default), and again only if that changes. The Settings page always shows it.
        try
        {
            KhuMue.MigrateOldKeybind();
            bool unbound = KhuMue.AssignedKeys().Count == 0;
            string clashes = unbound ? "" : UI.Pages.SettingsPage.KeyClashes();
            string notice = unbound ? "unbound" : clashes.Length > 0 ? KhuMue.ToggleKeyText() + " | " + clashes : "";
            if (notice.Length > 0 && notice != KeyClashSeen)
            {
                string text = unbound
                    ? UI.Pages.PageKit.L("Settings.KeyNoneChat")
                    : UI.Pages.PageKit.L("Settings.KeyClashChat", KhuMue.ToggleKeyText(), clashes);
                Main.NewText(text, new Color(255, 205, 110));
            }
            KeyClashSeen = notice;
        }
        catch (Exception e)
        {
            // A hint must never break entering a world.
            KhuMue.Instance.Logger.Warn("Key clash check failed: " + e.Message);
        }
    }

    public override void SaveData(TagCompound tag)
    {
        if (!string.IsNullOrEmpty(PreferredClass))
            tag["class"] = PreferredClass;
        if (HasOpenedGuide)
            tag["opened"] = true;
        if (!string.IsNullOrEmpty(KeyClashSeen))
            tag["keyClashSeen"] = KeyClashSeen;
    }

    public override void LoadData(TagCompound tag)
    {
        PreferredClass = tag.GetString("class");
        HasOpenedGuide = tag.GetBool("opened");
        KeyClashSeen = tag.GetString("keyClashSeen");
    }
}
