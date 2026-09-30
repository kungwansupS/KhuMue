using System.Globalization;
using ReLogic.Graphics;

namespace KhuMue.UI;

/// <summary>Plain text measuring, drawing and wrapping with Thai-aware break points.</summary>
internal static class Txt
{
    private static readonly Dictionary<(string, float), Vector2> MeasureCache = new();

    internal static void ClearCaches()
    {
        MeasureCache.Clear();
        WrapCache.Clear();
    }

    internal static Vector2 Measure(string text, float scale)
    {
        if (string.IsNullOrEmpty(text))
            return new Vector2(0f, Theme.LineHeight(scale));
        var key = (text, scale);
        if (MeasureCache.TryGetValue(key, out Vector2 size))
            return size;
        if (MeasureCache.Count > 4000)
            MeasureCache.Clear();
        size = Theme.Font.MeasureString(text) * scale;
        MeasureCache[key] = size;
        return size;
    }

    internal static float Width(string text, float scale) => Measure(text, scale).X;

    /// <summary>Draws text with a soft 1px shadow; y is the top of the line box.</summary>
    internal static void DrawString(SpriteBatch sb, string text, Vector2 position, Color color, float scale, bool shadow = true)
    {
        if (string.IsNullOrEmpty(text))
            return;
        DynamicSpriteFont font = Theme.Font;
        position = new Vector2(MathF.Round(position.X), MathF.Round(position.Y));
        if (shadow)
        {
            Color shadowColor = Color.Black * (color.A / 255f * 0.65f);
            sb.DrawString(font, text, position + new Vector2(1.5f, 1.5f), shadowColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        sb.DrawString(font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }

    /// <summary>Vertically centers a single line inside a box of the given height.</summary>
    internal static void DrawCentered(SpriteBatch sb, string text, Rectangle box, Color color, float scale, bool horizontal = true)
    {
        Vector2 size = Measure(text, scale);
        float x = horizontal ? box.X + (box.Width - size.X) / 2f : box.X;
        float y = box.Y + (box.Height - Theme.LineHeight(scale)) / 2f + 2f;
        DrawString(sb, text, new Vector2(x, y), color, scale);
    }

    /// <summary>Cuts text to fit a width, appending an ellipsis, without splitting Thai clusters.</summary>
    internal static string Ellipsize(string text, float maxWidth, float scale)
    {
        if (string.IsNullOrEmpty(text) || Width(text, scale) <= maxWidth)
            return text;
        const string ellipsis = "...";
        int[] starts = StringInfo.ParseCombiningCharacters(text);
        int low = 0, high = starts.Length - 1, best = 0;
        while (low <= high)
        {
            int mid = (low + high) / 2;
            string candidate = text[..starts[mid]] + ellipsis;
            if (Width(candidate, scale) <= maxWidth)
            {
                best = mid;
                low = mid + 1;
            }
            else
            {
                high = mid - 1;
            }
        }
        return best <= 0 ? ellipsis : text[..starts[best]].TrimEnd() + ellipsis;
    }

    // ------------------------------------------------------------------ wrapping

    private static readonly Dictionary<(string, int, float), List<string>> WrapCache = new();

    /// <summary>Wraps plain text into lines no wider than maxWidth. Explicit newlines are kept.</summary>
    internal static List<string> Wrap(string text, int maxWidth, float scale)
    {
        var key = (text ?? "", maxWidth, scale);
        if (WrapCache.TryGetValue(key, out List<string>? cached))
            return cached;
        if (WrapCache.Count > 2000)
            WrapCache.Clear();

        var lines = new List<string>();
        foreach (string paragraph in (text ?? "").Replace("\r", "").Split('\n'))
        {
            if (paragraph.Length == 0)
            {
                lines.Add("");
                continue;
            }
            WrapParagraph(paragraph, maxWidth, scale, lines);
        }
        WrapCache[key] = lines;
        return lines;
    }

    private static void WrapParagraph(string paragraph, int maxWidth, float scale, List<string> lines)
    {
        int start = 0;
        while (start < paragraph.Length)
        {
            int end = FitEnd(paragraph, start, maxWidth, scale);
            lines.Add(paragraph[start..end].TrimEnd());
            start = end;
            while (start < paragraph.Length && paragraph[start] == ' ')
                start++;
        }
    }

    /// <summary>Largest break position after <paramref name="start"/> whose text fits.</summary>
    internal static int FitEnd(string text, int start, float maxWidth, float scale)
    {
        if (Width(text[start..], scale) <= maxWidth)
            return text.Length;

        // Prefer breaking at spaces; fall back to Thai syllable boundaries only when a
        // space-free run is wider than the line.
        int lastSpace = -1, lastAny = -1;
        foreach (int pos in BreakPositions(text, start))
        {
            if (Width(text[start..pos], scale) > maxWidth)
                break;
            lastAny = pos;
            char before = text[pos - 1];
            if (pos == text.Length || before == ' ' || before == '/' || before == '-')
                lastSpace = pos;
        }
        if (lastSpace > start && lastSpace - start >= (lastAny - start) / 3)
            return lastSpace;
        if (lastAny > start)
            return lastAny;

        // No break point fits: cut at the last grapheme cluster that fits.
        int[] clusters = StringInfo.ParseCombiningCharacters(text[start..]);
        int fit = start + (clusters.Length > 1 ? clusters[1] : text.Length - start);
        for (int i = 1; i < clusters.Length; i++)
        {
            int pos = start + clusters[i];
            if (Width(text[start..pos], scale) <= maxWidth)
                fit = pos;
            else
                break;
        }
        return Math.Max(fit, start + 1);
    }

    /// <summary>
    /// Break opportunities: after spaces, and between Thai syllables. A Thai break is not allowed
    /// before a combining mark or a following vowel, and not after a leading vowel.
    /// This is a heuristic, not dictionary segmentation, so guide text uses spaces between phrases.
    /// </summary>
    internal static IEnumerable<int> BreakPositions(string text, int start)
    {
        int[] clusters = StringInfo.ParseCombiningCharacters(text[start..]);
        for (int i = 1; i < clusters.Length; i++)
        {
            int pos = start + clusters[i];
            char prev = text[pos - 1];
            char cur = text[pos];
            if (prev == ' ' || prev == '/' || prev == '-')
            {
                yield return pos;
                continue;
            }
            if (cur == ' ')
                continue;
            if (IsThai(cur) && IsThai(prev) && CanStartThai(cur) && CanEndThai(text, pos))
                yield return pos;
            else if (IsThai(cur) != IsThai(prev) && char.IsLetter(cur) && char.IsLetter(prev))
                yield return pos; // Thai <-> Latin boundary
        }
        yield return text.Length;
    }

    private static bool IsThai(char c) => c >= '฀' && c <= '๿';

    /// <summary>Characters that may not begin a line: following vowels, marks, repetition signs.</summary>
    private static bool CanStartThai(char c) => c switch
    {
        'ะ' or 'า' or 'ำ' or 'ๅ' or 'ๆ' or 'ฯ' => false, // ะ า ำ ๅ ๆ ฯ
        >= 'ั' and <= 'ฺ' => false, // above/below vowels and marks
        >= '็' and <= '๎' => false, // tone marks
        _ => true,
    };

    /// <summary>A line may not end right after a leading vowel (เ แ โ ใ ไ) or in the middle of "รร".</summary>
    private static bool CanEndThai(string text, int pos)
    {
        char prev = text[pos - 1];
        if (prev is 'เ' or 'แ' or 'โ' or 'ใ' or 'ไ')
            return false;
        // Keep a consonant together with a following "ร" cluster start like "กร", "ปร" is not
        // detectable without a dictionary; accept the heuristic.
        return true;
    }
}
