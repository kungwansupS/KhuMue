namespace KhuMue.Core;

internal enum EntryState
{
    /// <summary>Completed in this world.</summary>
    Done,

    /// <summary>The next step on the recommended route.</summary>
    Next,

    /// <summary>Can be done now (requirements met), not completed yet.</summary>
    Available,

    /// <summary>Requirements not met yet.</summary>
    Locked,

    /// <summary>The world state could not be read (mod internals changed).</summary>
    Unknown,
}

internal sealed class EntryStatus(ProgressionEntry entry)
{
    public ProgressionEntry Entry { get; } = entry;
    public bool? Done { get; set; }
    public bool? Available { get; set; }
    public EntryState State { get; set; }
}

/// <summary>
/// Evaluates the curated progression entries against the current world, a few times per second
/// at most. All results are read-only views of world/player state; nothing is changed.
/// </summary>
internal static class ProgressService
{
    private static readonly List<EntryStatus> Visible = new();
    private static readonly Dictionary<string, EntryStatus> ById = new();
    private static uint _lastUpdate = uint.MaxValue;
    private static StageDefinition? _stage;
    private static int _stageIndex;
    private static EntryStatus? _next;

    internal static IReadOnlyList<EntryStatus> Entries
    {
        get
        {
            Refresh();
            return Visible;
        }
    }

    internal static EntryStatus? Next
    {
        get
        {
            Refresh();
            return _next;
        }
    }

    /// <summary>Active stage ladder: Calamity's when Calamity is loaded, otherwise vanilla.</summary>
    internal static List<StageDefinition> Ladder => SupportedMods.CalamityLoaded ? GuideData.Stages.Calamity : GuideData.Stages.Vanilla;

    internal static StageDefinition? Stage
    {
        get
        {
            Refresh();
            return _stage;
        }
    }

    internal static int StageIndex
    {
        get
        {
            Refresh();
            return _stageIndex;
        }
    }

    internal static EntryStatus? Get(string id)
    {
        Refresh();
        return ById.TryGetValue(id, out EntryStatus? status) ? status : null;
    }

    internal static void Invalidate() => _lastUpdate = uint.MaxValue;

    internal static void Refresh(bool force = false)
    {
        uint now = Main.GameUpdateCount;
        if (!force && _lastUpdate != uint.MaxValue && now - _lastUpdate < 30)
            return;
        _lastUpdate = now;

        Visible.Clear();
        ById.Clear();
        _next = null;

        var replaced = new HashSet<string>();
        var candidates = new List<ProgressionEntry>();
        foreach (ProgressionEntry entry in GuideData.Entries)
        {
            if (entry.Mod != "Terraria" && !ModLoader.HasMod(entry.Mod))
                continue;
            if (GuideCondition.Parse(entry.Show).Evaluate() == false)
                continue;
            candidates.Add(entry);
            foreach (string id in entry.Replaces)
                replaced.Add(id);
        }

        foreach (ProgressionEntry entry in candidates)
        {
            if (replaced.Contains(entry.Id))
                continue;
            var status = new EntryStatus(entry)
            {
                Done = GuideCondition.Parse(entry.Done).Evaluate(),
                Available = GuideCondition.Parse(entry.Available).Evaluate(),
            };
            status.State = status.Done switch
            {
                true => EntryState.Done,
                null when !entry.Repeatable => EntryState.Unknown,
                _ => status.Available == false ? EntryState.Locked : EntryState.Available,
            };
            Visible.Add(status);
            ById[entry.Id] = status;
        }

        // Current stage: the last stage in the ladder whose condition holds.
        _stage = null;
        _stageIndex = 0;
        List<StageDefinition> ladder = Ladder;
        for (int i = 0; i < ladder.Count; i++)
        {
            if (GuideCondition.Parse(ladder[i].When).Evaluate() == true)
            {
                _stage = ladder[i];
                _stageIndex = i;
            }
        }

        // The recommended next step: the first main-route entry that is not done, looking at the world's
        // current era first. Main steps of earlier eras that were skipped (a pre-Hardmode boss in a Hardmode
        // world) are not pushed as "next"; they stay in the list of things that can also be done.
        int era = EraRank(_stage?.Era);
        EntryStatus? fallback = null;
        foreach (EntryStatus status in Visible)
        {
            if (!status.Entry.IsMain || status.Entry.Parent is not null || status.Entry.Repeatable || status.Done != false)
                continue;
            fallback ??= status;
            if (EraRank(status.Entry.Era) < era)
                continue;
            _next = status;
            break;
        }
        _next ??= fallback;
        if (_next is { State: EntryState.Available })
            _next.State = EntryState.Next;
    }

    private static int EraRank(string? era) => era switch
    {
        "hm" => 1,
        "post" => 2,
        "end" => 3,
        _ => 0,
    };

    /// <summary>Main-route entries that are done / total, optionally for one era.</summary>
    internal static (int Done, int Total) MainProgress(string? era = null)
    {
        Refresh();
        int done = 0, total = 0;
        foreach (EntryStatus status in Visible)
        {
            if (!status.Entry.IsMain || status.Entry.Parent is not null || status.Entry.Repeatable)
                continue;
            if (era is not null && status.Entry.Era != era)
                continue;
            total++;
            if (status.Done == true)
                done++;
        }
        return (done, total);
    }

    /// <summary>
    /// Other content that can be done right now: skipped main-route steps first, then side content
    /// closest to the current frontier.
    /// </summary>
    internal static List<EntryStatus> AvailableNow(int max)
    {
        Refresh();
        float frontier = _next?.Entry.Order ?? float.MaxValue;
        return Visible
            .Where(s => s.Entry.Parent is null && s != _next && s.State == EntryState.Available
                        && s.Entry.Order <= frontier + 2.5f)
            .OrderBy(s => s.Entry.IsMain && !s.Entry.Repeatable ? 0 : 1)
            .ThenBy(s => Math.Abs(s.Entry.Order - frontier))
            .Take(max)
            .ToList();
    }
}
