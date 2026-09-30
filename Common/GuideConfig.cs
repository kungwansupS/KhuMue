using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace KhuMue.Common;

/// <summary>Client-only display options. Nothing here changes the world or other mods.</summary>
public sealed class GuideConfig : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    internal static GuideConfig? Instance => ModContent.GetInstance<GuideConfig>();

    [DefaultValue(1f)]
    [Range(0.6f, 1.2f)]
    [Increment(0.05f)]
    [Slider]
    public float WindowSize { get; set; } = 1f;

    [DefaultValue(1f)]
    [Range(0.8f, 1.4f)]
    [Increment(0.05f)]
    [Slider]
    public float TextScale { get; set; } = 1f;

    [DefaultValue(SpoilerMode.ShowAll)]
    public SpoilerMode Spoilers { get; set; } = SpoilerMode.ShowAll;

    [DefaultValue(true)]
    public bool OpenForNewCharacters { get; set; } = true;

    [DefaultValue(true)]
    public bool ShowVersionNotice { get; set; } = true;
}

public enum SpoilerMode
{
    /// <summary>Everything is visible.</summary>
    ShowAll,

    /// <summary>Names and pictures of bosses after the next step on the route are hidden until reached.</summary>
    HideFuture,
}
