namespace KhuMue.UI;

/// <summary>
/// Nested scissor clipping plus content translation (for scrolling), in UI coordinates.
/// Follows tModLoader's UIElement.OverflowHidden contract: convert through Main.UIScaleMatrix,
/// intersect with the parent scissor, and always restore the previous batch state.
/// </summary>
internal static class Clip
{
    private static readonly RasterizerState ScissorOn = new() { CullMode = CullMode.None, ScissorTestEnable = true };

    private readonly record struct Frame(Rectangle Scissor, RasterizerState Rasterizer, Vector2 Translation);

    private static readonly Stack<Frame> Stack = new();

    /// <summary>Total translation currently applied to drawing.</summary>
    internal static Vector2 Translation { get; private set; }

    internal static Rectangle ToScreen(Rectangle ui)
    {
        Vector2 a = Vector2.Transform(new Vector2(ui.X, ui.Y), Main.UIScaleMatrix);
        Vector2 b = Vector2.Transform(new Vector2(ui.Right, ui.Bottom), Main.UIScaleMatrix);
        var rect = new Rectangle((int)a.X, (int)a.Y, (int)Math.Ceiling(b.X - a.X), (int)Math.Ceiling(b.Y - a.Y));
        int width = Main.instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
        int height = Main.instance.GraphicsDevice.PresentationParameters.BackBufferHeight;
        return Rectangle.Intersect(rect, new Rectangle(0, 0, width, height));
    }

    /// <summary>
    /// Clip to <paramref name="ui"/> (given in the current, already translated space) and add
    /// <paramref name="scroll"/> to the translation for everything drawn until <see cref="Pop"/>.
    /// </summary>
    internal static void Push(SpriteBatch sb, Rectangle ui, Vector2 scroll = default)
    {
        GraphicsDevice device = sb.GraphicsDevice;
        Rectangle previous = device.ScissorRectangle;
        Stack.Push(new Frame(previous, device.RasterizerState, Translation));
        sb.End();

        var translatedUi = new Rectangle(ui.X + (int)Translation.X, ui.Y + (int)Translation.Y, ui.Width, ui.Height);
        Rectangle next = ToScreen(translatedUi);
        if (Stack.Count > 1)
            next = Rectangle.Intersect(next, previous);
        device.ScissorRectangle = next;
        Translation += scroll;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, ScissorOn, null, Matrix);
    }

    internal static void Pop(SpriteBatch sb)
    {
        if (Stack.Count == 0)
            return;
        Frame frame = Stack.Pop();
        sb.End();
        sb.GraphicsDevice.ScissorRectangle = frame.Scissor;
        Translation = frame.Translation;
        RasterizerState state = Stack.Count > 0 ? ScissorOn : frame.Rasterizer;
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, state, null, Matrix);
    }

    private static Matrix Matrix => Translation == Vector2.Zero
        ? Main.UIScaleMatrix
        : Microsoft.Xna.Framework.Matrix.CreateTranslation(Translation.X, Translation.Y, 0f) * Main.UIScaleMatrix;

    /// <summary>Safety net after an exception mid-draw.</summary>
    internal static void Reset(SpriteBatch sb)
    {
        while (Stack.Count > 0)
            Pop(sb);
        Translation = Vector2.Zero;
    }
}
