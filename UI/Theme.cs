using ReLogic.Graphics;
using Terraria.GameContent;

namespace KhuMue.UI;

/// <summary>Colors, sizes and fonts shared by every guide screen.</summary>
internal static class Theme
{
    // Surfaces (dark navy, slightly translucent window so the game stays visible at the edges).
    internal static readonly Color Backdrop = new(4, 7, 12, 150);
    internal static readonly Color Window = new(14, 20, 31, 246);
    internal static readonly Color Sidebar = new(10, 15, 24, 250);
    internal static readonly Color Header = new(17, 25, 38, 250);
    internal static readonly Color Surface = new(24, 34, 50);
    internal static readonly Color SurfaceAlt = new(30, 42, 61);
    internal static readonly Color SurfaceHover = new(36, 51, 74);
    internal static readonly Color SurfacePressed = new(44, 62, 90);
    internal static readonly Color Border = new(49, 66, 92);
    internal static readonly Color BorderStrong = new(78, 101, 136);
    internal static readonly Color Slot = new(20, 29, 44);

    // Text.
    internal static readonly Color Text = new(236, 241, 247);
    internal static readonly Color TextMuted = new(160, 176, 196);
    internal static readonly Color TextFaint = new(112, 128, 150);

    // Accents. Meaning never depends on color alone: every status also has an icon or word.
    internal static readonly Color Accent = new(94, 214, 196);
    internal static readonly Color AccentDim = new(40, 110, 102);
    internal static readonly Color Gold = new(244, 199, 104);
    internal static readonly Color Success = new(126, 214, 146);
    internal static readonly Color Warning = new(255, 182, 77);
    internal static readonly Color Danger = new(255, 116, 104);
    internal static readonly Color Info = new(125, 175, 255);
    internal static readonly Color Link = new(126, 205, 255);

    // Era colors used for badges and era headers.
    internal static readonly Color EraPre = new(118, 196, 98);
    internal static readonly Color EraHard = new(178, 124, 255);
    internal static readonly Color EraPost = new(255, 164, 70);
    internal static readonly Color EraEnd = new(255, 92, 120);

    internal static Color Era(string era) => era switch
    {
        "hm" => EraHard,
        "post" => EraPost,
        "end" => EraEnd,
        _ => EraPre,
    };

    internal static Color ClassColor(string cls) => cls switch
    {
        "melee" => new Color(255, 140, 70),
        "ranged" => new Color(120, 220, 110),
        "magic" => new Color(110, 170, 255),
        "summon" => new Color(90, 225, 225),
        "rogue" => new Color(206, 132, 227),
        _ => Text,
    };

    internal static DynamicSpriteFont Font => FontAssets.MouseText.Value;

    /// <summary>Text size multiplier chosen by the player.</summary>
    internal static float TextScale => Math.Clamp(Common.GuideConfig.Instance?.TextScale ?? 1f, 0.8f, 1.4f);

    internal static float Body => 0.86f * TextScale;
    internal static float Small => 0.74f * TextScale;
    internal static float Title => 1.05f * TextScale;
    internal static float Hero => 1.25f * TextScale;

    /// <summary>Line height for a given text scale, generous enough for Thai marks above and below.</summary>
    internal static int LineHeight(float scale) => (int)Math.Ceiling(Font.LineSpacing * scale * 0.92f) + 2;

    internal const int Pad = 12;
    internal const int Gap = 8;
    internal const int Radius = 6;
}
