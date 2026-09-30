using KhuMue.Common;
using KhuMue.Core;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Detected mods, data versions, options and credits.</summary>
internal sealed class SettingsPage : Page
{
    public override string Title => L("Settings.Title");
    public override Section Section => Section.Settings;

    public override Node Build(int width)
    {
        var page = new VStack { Spacing = 14 };
        GuideConfig? config = GuideConfig.Instance;

        VStack options = PageKit.Section(L("Settings.Options"), ItemID.Cog);
        if (KhuMue.AssignedKeys().Count == 0)
            options.Add(Callout(L("Settings.KeyNone"), Theme.Warning));
        else
            options.Add(new Paragraph(L("Settings.Key", KhuMue.ToggleKeyText())));
        string clashes = KeyClashes();
        if (clashes.Length > 0)
            options.Add(Callout(L("Settings.KeyClash", KhuMue.ToggleKeyText(), clashes), Theme.Warning));
        if (config is not null)
        {
            var sizes = new Flow { Spacing = 6 };
            sizes.Add(new Label(L("Settings.TextSize"), Theme.TextMuted, Theme.Small));
            foreach (float s in new[] { 0.9f, 1f, 1.1f, 1.2f, 1.3f })
            {
                float value = s;
                sizes.Add(new Chip((int)(s * 100) + "%", Math.Abs(config.TextScale - s) < 0.01f, () =>
                {
                    config.TextScale = value;
                    Save(config);
                    Txt.ClearCaches();
                    RichText.ClearCache();
                    Dirty = true;
                }));
            }
            options.Add(sizes);

            var window = new Flow { Spacing = 6 };
            window.Add(new Label(L("Settings.WindowSize"), Theme.TextMuted, Theme.Small));
            foreach (float s in new[] { 0.8f, 0.9f, 1f, 1.1f })
            {
                float value = s;
                window.Add(new Chip((int)(s * 100) + "%", Math.Abs(config.WindowSize - s) < 0.01f, () =>
                {
                    config.WindowSize = value;
                    Save(config);
                    Dirty = true;
                }));
            }
            options.Add(window);

            var toggles = new Flow { Spacing = 6 };
            toggles.Add(new Chip(L("Settings.HideSpoilers"), config.Spoilers == SpoilerMode.HideFuture, () =>
            {
                config.Spoilers = config.Spoilers == SpoilerMode.HideFuture ? SpoilerMode.ShowAll : SpoilerMode.HideFuture;
                Save(config);
                Dirty = true;
            }));
            toggles.Add(new Chip(L("Settings.OpenForNew"), config.OpenForNewCharacters, () =>
            {
                config.OpenForNewCharacters = !config.OpenForNewCharacters;
                Save(config);
                Dirty = true;
            }));
            toggles.Add(new Chip(L("Settings.VersionNotice"), config.ShowVersionNotice, () =>
            {
                config.ShowVersionNotice = !config.ShowVersionNotice;
                Save(config);
                Dirty = true;
            }));
            options.Add(toggles);
        }
        page.Add(options);

        VStack mods = PageKit.Section(L("Settings.Mods"), ItemID.Book);
        mods.Add(Muted(L("Settings.ModsIntro")));
        foreach (SupportedMod mod in SupportedMods.All)
        {
            var row = new HStack { Spacing = 10 };
            row.AddFlex(new Label(SupportedMods.DisplayName(mod.Name), Theme.Text, Theme.Body));
            string loaded = mod.LoadedVersion?.ToString() ?? L("Settings.NotLoaded");
            row.AddFixed(new Label(loaded, Theme.TextMuted, Theme.Small), 110);
            row.AddFixed(new Label(L("Settings.CheckedFor", mod.VerifiedVersion), Theme.TextMuted, Theme.Small), 150);
            (string text, Color color) = mod.Match switch
            {
                VersionMatch.Exact => (L("Settings.Match.Exact"), Theme.Success),
                VersionMatch.Near => (L("Settings.Match.Near"), Theme.Warning),
                VersionMatch.Different => (L("Settings.Match.Different"), Theme.Danger),
                _ => (L("Settings.Match.NotLoaded"), Theme.TextFaint),
            };
            row.AddFixed(new Badge(text, color), 130);
            mods.Add(new Card(row) { Padding = 8 });
        }
        if (GuideData.LoadErrors.Count > 0)
            mods.Add(Callout(string.Join("\n", GuideData.LoadErrors), Theme.Danger, L("Settings.DataErrors")));
        page.Add(mods);

        VStack about = PageKit.Section(L("Settings.About"));
        about.Add(new Paragraph(L("Settings.AboutBody")));
        about.Add(Muted(L("Settings.Credits")));
        page.Add(about);
        return page;
    }

    private static void Save(GuideConfig config)
    {
        try
        {
            // Persist the client config the same way the in-game config menu does.
            typeof(Terraria.ModLoader.Config.ConfigManager)
                .GetMethod("Save", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                ?.Invoke(null, new object[] { config });
        }
        catch
        {
            // Changes still apply for this session.
        }
        Sounds.Click();
    }

    /// <summary>
    /// Other controls bound to the same key as the guide: vanilla triggers ("QuickBuff") and other mods'
    /// keybinds ("ImproveGame/MasterControl") share one table in the input profile.
    /// </summary>
    internal static string KeyClashes()
    {
        List<string> keys = KhuMue.AssignedKeys();
        if (keys.Count == 0)
            return "";
        var names = new List<string>();
        try
        {
            foreach (KeyValuePair<string, List<string>> binding in Terraria.GameInput.PlayerInput.CurrentProfile.InputModes[Terraria.GameInput.InputMode.Keyboard].KeyStatus)
            {
                if (binding.Key == KhuMue.Instance.Name + "/" + KhuMue.ToggleKeybindName || !binding.Value.Any(keys.Contains))
                    continue;
                string name = binding.Key.Replace("/", ": ");
                names.Add(System.Text.RegularExpressions.Regex.Replace(name, "(?<=[a-z])(?=[A-Z])", " "));
            }
        }
        catch (Exception e)
        {
            KhuMue.Instance.Logger.Warn("Could not read key bindings: " + e.Message);
            return "";
        }
        return string.Join(", ", names);
    }
}
