namespace KhuMue.Core;

/// <summary>
/// Tiny boolean expression over <see cref="Flags"/> with three-valued logic.
/// Grammar:  expr := or ;  or := and ('|' and)* ;  and := unary ('&amp;' unary)* ;
///           unary := '!' unary | '(' expr ')' | flag ;  flag := [A-Za-z0-9._ ]+  or  mod:Name
/// "mod:Name" is true when that mod is loaded. Unknown flags evaluate to null (unknown).
/// </summary>
internal abstract class GuideCondition
{
    public abstract bool? Evaluate();

    /// <summary>All flag keys referenced, for data validation.</summary>
    public abstract IEnumerable<string> Keys();

    internal static readonly GuideCondition True = new Literal(true);

    private static readonly Dictionary<string, GuideCondition> Cache = new();

    internal static GuideCondition Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return True;
        lock (Cache)
        {
            if (Cache.TryGetValue(text, out GuideCondition? cached))
                return cached;
            var parser = new Parser(text);
            GuideCondition result = parser.ParseOr();
            parser.ExpectEnd();
            Cache[text] = result;
            return result;
        }
    }

    internal static void ClearCache()
    {
        lock (Cache)
            Cache.Clear();
    }

    private sealed class Literal(bool value) : GuideCondition
    {
        public override bool? Evaluate() => value;
        public override IEnumerable<string> Keys() => Array.Empty<string>();
    }

    private sealed class FlagRef(string key) : GuideCondition
    {
        public override bool? Evaluate()
        {
            if (key.StartsWith("mod:", StringComparison.OrdinalIgnoreCase))
                return ModLoader.HasMod(key[4..]);
            return Flags.Get(key);
        }

        public override IEnumerable<string> Keys() => new[] { key };
    }

    private sealed class Not(GuideCondition inner) : GuideCondition
    {
        public override bool? Evaluate() => !inner.Evaluate();
        public override IEnumerable<string> Keys() => inner.Keys();
    }

    private sealed class And(List<GuideCondition> parts) : GuideCondition
    {
        public override bool? Evaluate()
        {
            bool unknown = false;
            foreach (GuideCondition part in parts)
            {
                bool? value = part.Evaluate();
                if (value == false)
                    return false;
                if (value is null)
                    unknown = true;
            }
            return unknown ? null : true;
        }

        public override IEnumerable<string> Keys() => parts.SelectMany(p => p.Keys());
    }

    private sealed class Or(List<GuideCondition> parts) : GuideCondition
    {
        public override bool? Evaluate()
        {
            bool unknown = false;
            foreach (GuideCondition part in parts)
            {
                bool? value = part.Evaluate();
                if (value == true)
                    return true;
                if (value is null)
                    unknown = true;
            }
            return unknown ? null : false;
        }

        public override IEnumerable<string> Keys() => parts.SelectMany(p => p.Keys());
    }

    private sealed class Parser(string text)
    {
        private int _pos;

        public GuideCondition ParseOr()
        {
            var parts = new List<GuideCondition> { ParseAnd() };
            while (Peek() == '|')
            {
                _pos++;
                parts.Add(ParseAnd());
            }
            return parts.Count == 1 ? parts[0] : new Or(parts);
        }

        private GuideCondition ParseAnd()
        {
            var parts = new List<GuideCondition> { ParseUnary() };
            while (Peek() == '&')
            {
                _pos++;
                parts.Add(ParseUnary());
            }
            return parts.Count == 1 ? parts[0] : new And(parts);
        }

        private GuideCondition ParseUnary()
        {
            char c = Peek();
            if (c == '!')
            {
                _pos++;
                return new Not(ParseUnary());
            }
            if (c == '(')
            {
                _pos++;
                GuideCondition inner = ParseOr();
                if (Peek() != ')')
                    throw new FormatException($"Missing ')' in condition '{text}'.");
                _pos++;
                return inner;
            }

            int start = _pos;
            while (_pos < text.Length && (char.IsLetterOrDigit(text[_pos]) || text[_pos] is '.' or '_' or ':' or ' ' or '-'))
                _pos++;
            string key = text[start.._pos].Trim();
            if (key.Length == 0)
                throw new FormatException($"Expected a flag at position {start} in condition '{text}'.");
            if (key.Equals("true", StringComparison.OrdinalIgnoreCase) || key.Equals("always", StringComparison.OrdinalIgnoreCase))
                return True;
            if (key.Equals("false", StringComparison.OrdinalIgnoreCase) || key.Equals("never", StringComparison.OrdinalIgnoreCase))
                return new Literal(false);
            return new FlagRef(key);
        }

        private char Peek()
        {
            while (_pos < text.Length && char.IsWhiteSpace(text[_pos]))
                _pos++;
            return _pos < text.Length ? text[_pos] : '\0';
        }

        public void ExpectEnd()
        {
            if (Peek() != '\0')
                throw new FormatException($"Unexpected '{text[_pos]}' in condition '{text}'.");
        }
    }
}
