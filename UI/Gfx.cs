using Terraria.GameContent;

namespace KhuMue.UI;

/// <summary>Drawing primitives. Rounded shapes use small textures generated once on the main thread.</summary>
internal static class Gfx
{
    private const int CornerSize = 8;
    private static Texture2D? _rounded;
    private static Texture2D? _roundedBorder;
    private static Texture2D? _circle;

    internal static Texture2D Pixel => TextureAssets.MagicPixel.Value;

    internal static void Unload()
    {
        Texture2D? a = _rounded, b = _roundedBorder, c = _circle;
        _rounded = _roundedBorder = _circle = null;
        if (a is not null || b is not null || c is not null)
        {
            Main.QueueMainThreadAction(() =>
            {
                a?.Dispose();
                b?.Dispose();
                c?.Dispose();
            });
        }
    }

    private static void EnsureTextures()
    {
        if (_rounded is not null && !_rounded.IsDisposed)
            return;
        GraphicsDevice device = Main.instance.GraphicsDevice;
        int size = CornerSize * 2 + 1;
        _rounded = new Texture2D(device, size, size);
        _roundedBorder = new Texture2D(device, size, size);
        var fill = new Color[size * size];
        var border = new Color[size * size];
        float r = CornerSize;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float cx = x < CornerSize ? CornerSize - x - 0.5f : (x > CornerSize ? x - CornerSize - 0.5f : 0);
                float cy = y < CornerSize ? CornerSize - y - 0.5f : (y > CornerSize ? y - CornerSize - 0.5f : 0);
                float d = MathF.Sqrt(cx * cx + cy * cy);
                float a = Math.Clamp(r - d + 0.5f, 0f, 1f);
                fill[y * size + x] = Color.White * a;
                float inner = Math.Clamp((r - 1.2f) - d + 0.5f, 0f, 1f);
                border[y * size + x] = Color.White * Math.Clamp(a - inner, 0f, 1f);
            }
        }
        _rounded.SetData(fill);
        _roundedBorder.SetData(border);

        int cs = 32;
        _circle = new Texture2D(device, cs, cs);
        var circle = new Color[cs * cs];
        float cr = cs / 2f;
        for (int y = 0; y < cs; y++)
        {
            for (int x = 0; x < cs; x++)
            {
                float dx = x + 0.5f - cr, dy = y + 0.5f - cr;
                float a = Math.Clamp(cr - MathF.Sqrt(dx * dx + dy * dy), 0f, 1f);
                circle[y * cs + x] = Color.White * a;
            }
        }
        _circle.SetData(circle);
    }

    internal static void Fill(SpriteBatch sb, Rectangle r, Color color)
    {
        if (r.Width > 0 && r.Height > 0 && color.A > 0)
            sb.Draw(Pixel, r, color);
    }

    internal static void Outline(SpriteBatch sb, Rectangle r, Color color, int thickness = 1)
    {
        Fill(sb, new Rectangle(r.X, r.Y, r.Width, thickness), color);
        Fill(sb, new Rectangle(r.X, r.Bottom - thickness, r.Width, thickness), color);
        Fill(sb, new Rectangle(r.X, r.Y + thickness, thickness, r.Height - thickness * 2), color);
        Fill(sb, new Rectangle(r.Right - thickness, r.Y + thickness, thickness, r.Height - thickness * 2), color);
    }

    /// <summary>Rounded rectangle, optional 1px border. Radius is capped at 8.</summary>
    internal static void Panel(SpriteBatch sb, Rectangle r, Color fill, Color? border = null, int radius = Theme.Radius)
    {
        if (r.Width <= 0 || r.Height <= 0)
            return;
        EnsureTextures();
        radius = Math.Clamp(Math.Min(radius, Math.Min(r.Width, r.Height) / 2), 0, CornerSize);
        if (radius <= 1)
        {
            Fill(sb, r, fill);
            if (border is Color b0)
                Outline(sb, r, b0);
            return;
        }
        NineSlice(sb, _rounded!, r, radius, fill);
        if (border is Color b && b.A > 0)
            NineSlice(sb, _roundedBorder!, r, radius, b);
    }

    private static void NineSlice(SpriteBatch sb, Texture2D tex, Rectangle r, int radius, Color color)
    {
        int c = CornerSize;
        int w = r.Width, h = r.Height;
        // Corners (scaled from the 8px source corner down to `radius`).
        sb.Draw(tex, new Rectangle(r.X, r.Y, radius, radius), new Rectangle(0, 0, c, c), color);
        sb.Draw(tex, new Rectangle(r.Right - radius, r.Y, radius, radius), new Rectangle(c + 1, 0, c, c), color);
        sb.Draw(tex, new Rectangle(r.X, r.Bottom - radius, radius, radius), new Rectangle(0, c + 1, c, c), color);
        sb.Draw(tex, new Rectangle(r.Right - radius, r.Bottom - radius, radius, radius), new Rectangle(c + 1, c + 1, c, c), color);
        // Edges.
        sb.Draw(tex, new Rectangle(r.X + radius, r.Y, w - radius * 2, radius), new Rectangle(c, 0, 1, c), color);
        sb.Draw(tex, new Rectangle(r.X + radius, r.Bottom - radius, w - radius * 2, radius), new Rectangle(c, c + 1, 1, c), color);
        sb.Draw(tex, new Rectangle(r.X, r.Y + radius, radius, h - radius * 2), new Rectangle(0, c, c, 1), color);
        sb.Draw(tex, new Rectangle(r.Right - radius, r.Y + radius, radius, h - radius * 2), new Rectangle(c + 1, c, c, 1), color);
        // Center.
        sb.Draw(tex, new Rectangle(r.X + radius, r.Y + radius, w - radius * 2, h - radius * 2), new Rectangle(c, c, 1, 1), color);
    }

    internal static void Circle(SpriteBatch sb, Vector2 center, float radius, Color color)
    {
        EnsureTextures();
        sb.Draw(_circle!, center, null, color, 0f, new Vector2(16f), radius / 16f, SpriteEffects.None, 0f);
    }

    /// <summary>Soft drop shadow below a panel.</summary>
    internal static void Shadow(SpriteBatch sb, Rectangle r, int spread = 10, float strength = 0.45f)
    {
        for (int i = spread; i > 0; i -= 2)
        {
            var box = new Rectangle(r.X - i / 2, r.Y - i / 3, r.Width + i, r.Height + i);
            Panel(sb, box, Color.Black * (strength / spread * 2f), null, 8);
        }
    }

    internal static void Line(SpriteBatch sb, Vector2 a, Vector2 b, Color color, float thickness = 2f)
    {
        Vector2 d = b - a;
        float length = d.Length();
        if (length < 0.5f)
            return;
        sb.Draw(Pixel, a, new Rectangle(0, 0, 1, 1), color, MathF.Atan2(d.Y, d.X), new Vector2(0f, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 0f);
    }
}
