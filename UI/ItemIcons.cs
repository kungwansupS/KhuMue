using KhuMue.Core;
using Terraria.GameContent;
using Terraria.UI;

namespace KhuMue.UI;

/// <summary>Draws item and NPC artwork from the game's own textures.</summary>
internal static class ItemIcons
{
    /// <summary>Item icon centered on a point, scaled down to fit <paramref name="size"/> pixels.</summary>
    internal static void Draw(SpriteBatch sb, int itemType, Vector2 center, float size, Color? tint = null)
    {
        if (itemType <= 0 || itemType >= ItemLoader.ItemCount)
            return;
        try
        {
            Item item = GuideCatalog.SampleItem(itemType);
            ItemSlot.DrawItemIcon(item, ItemSlot.Context.InventoryItem, sb, center, 1f, size, tint ?? Color.White);
        }
        catch
        {
            // A third-party inventory draw hook failing must not break the guide window.
        }
    }

    /// <summary>
    /// NPC portrait: the boss map head icon when one exists (clean and small), otherwise the first
    /// animation frame of the NPC texture.
    /// </summary>
    /// <summary>Draws an NPC (boss head icon if wanted). False when there is nothing usable to draw.</summary>
    internal static bool DrawNpc(SpriteBatch sb, int npcType, Rectangle box, Color? tint = null, bool preferHead = true)
    {
        if (npcType == 0)
            return false;
        try
        {
            Color color = tint ?? Color.White;
            if (preferHead && npcType > 0 && npcType < NPCLoader.NPCCount)
            {
                int head = NPCID.Sets.BossHeadTextures[npcType];
                if (head >= 0 && head < TextureAssets.NpcHeadBoss.Length && TextureAssets.NpcHeadBoss[head]?.Value is Texture2D headTex)
                {
                    DrawFitted(sb, headTex, headTex.Bounds, box, color, allowUpscale: true);
                    return true;
                }
            }

            int type = npcType;
            if (type < 0)
                type = NPCID.FromNetId(type);
            Main.instance.LoadNPC(type);
            Texture2D tex = TextureAssets.Npc[type].Value;
            int frames = Math.Max(1, Main.npcFrameCount[type]);
            int frameHeight = tex.Height / frames;
            // Custom-drawn NPCs often ship a tiny placeholder texture; treat those as "no picture".
            if (tex.Width < 8 || frameHeight < 8)
                return false;
            DrawFitted(sb, tex, new Rectangle(0, 0, tex.Width, frameHeight), box, color, allowUpscale: false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static void DrawFitted(SpriteBatch sb, Texture2D tex, Rectangle source, Rectangle box, Color color, bool allowUpscale)
    {
        float scale = Math.Min(box.Width / (float)source.Width, box.Height / (float)source.Height);
        if (!allowUpscale)
            scale = Math.Min(scale, 1f);
        else if (scale > 1f)
            scale = MathF.Floor(scale); // keep pixel art crisp: whole-number upscales only
        scale = Math.Max(scale, 0.05f);
        sb.Draw(tex, box.Center.ToVector2(), source, color, 0f, source.Size() / 2f, scale, SpriteEffects.None, 0f);
    }
}

internal static class EntryNames
{
    /// <summary>Display name of a progression entry in the current language.</summary>
    internal static string Name(string id)
    {
        string key = Pages.PageKit.Root + "Entries." + id + ".Name";
        if (Language.Exists(key))
            return Language.GetTextValue(key);
        if (GuideData.EntriesById.TryGetValue(id, out ProgressionEntry? entry) && entry.Npcs.Count > 0)
        {
            int type = GuideCatalog.NpcType(entry.Npcs[0]);
            if (type != 0)
                return Lang.GetNPCNameValue(type);
        }
        return id;
    }

    /// <summary>
    /// Entry text field. A "{field}Cal" variant replaces the base text while Calamity is loaded
    /// (vanilla entries whose rules Calamity changes).
    /// </summary>
    internal static string Text(ProgressionEntry entry, string field)
    {
        string key = entry.LocalizationPrefix + "." + field;
        if (SupportedMods.CalamityLoaded && Language.Exists(key + "Cal"))
            return Language.GetTextValue(key + "Cal");
        return Language.Exists(key) ? Language.GetTextValue(key) : "";
    }
}
