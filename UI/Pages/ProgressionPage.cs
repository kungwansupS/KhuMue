using KhuMue.Core;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Every boss, event and quest step for the loaded mods, grouped by era.</summary>
internal sealed class ProgressionPage : Page
{
    private static string _era = "";
    private static bool _mainOnly;
    private static bool _showEvents = true;
    private static bool _showMinibosses = true;
    private static bool _hideDone;
    private int _doneCount = -1;

    public override string Title => L("Progression.Title");
    public override Section Section => Section.Progression;

    public override void Tick()
    {
        if (Main.GameUpdateCount % 60 == 0)
        {
            int done = ProgressService.Entries.Count(e => e.Done == true);
            if (_doneCount >= 0 && done != _doneCount)
                Dirty = true;
        }
    }

    public override Node Build(int width)
    {
        ProgressService.Refresh(force: true);
        IReadOnlyList<EntryStatus> all = ProgressService.Entries;
        _doneCount = all.Count(e => e.Done == true);

        var page = new VStack { Spacing = 12 };
        page.Add(Muted(L("Progression.Intro")));

        // Filters.
        var eras = new Flow { Spacing = 6 };
        foreach (string era in new[] { "", "pre", "hm", "post", "end" })
        {
            if (era.Length > 0 && !all.Any(e => e.Entry.Era == era))
                continue;
            string e = era;
            eras.Add(new Chip(era.Length == 0 ? L("Progression.AllEras") : EraName(era), _era == era, () =>
            {
                _era = e;
                Dirty = true;
                Sounds.Click();
            })
            { Color = era.Length == 0 ? Theme.Accent : Theme.Era(era) });
        }
        page.Add(eras);

        var toggles = new Flow { Spacing = 6 };
        toggles.Add(new Chip(L("Progression.MainOnly"), _mainOnly, () => { _mainOnly = !_mainOnly; Dirty = true; Sounds.Click(); }));
        toggles.Add(new Chip(L("Progression.Events"), _showEvents, () => { _showEvents = !_showEvents; Dirty = true; Sounds.Click(); }));
        toggles.Add(new Chip(L("Progression.Minibosses"), _showMinibosses, () => { _showMinibosses = !_showMinibosses; Dirty = true; Sounds.Click(); }));
        toggles.Add(new Chip(L("Progression.HideDone"), _hideDone, () => { _hideDone = !_hideDone; Dirty = true; Sounds.Click(); }));
        page.Add(toggles);

        IEnumerable<EntryStatus> shown = all.Where(Include);
        foreach (IGrouping<string, EntryStatus> group in shown.GroupBy(e => e.Entry.Era))
        {
            (int done, int total) = ProgressService.MainProgress(group.Key);
            string right = total > 0 ? L("Progression.EraProgress", done, total) : "";
            var header = new Heading(EraName(group.Key), right) { Color = Theme.Era(group.Key) };
            page.Add(header);
            foreach (EntryStatus status in group)
                page.Add(EntryRow(status));
        }

        if (!shown.Any())
            page.Add(Callout(L("Progression.Empty"), Theme.Info));
        return page;
    }

    private static bool Include(EntryStatus status)
    {
        ProgressionEntry entry = status.Entry;
        if (_era.Length > 0 && entry.Era != _era)
            return false;
        if (_mainOnly && !entry.IsMain)
            return false;
        if (!_showEvents && entry.Kind is "event")
            return false;
        if (!_showMinibosses && entry.Kind is "miniboss")
            return false;
        if (_hideDone && status.Done == true)
            return false;
        return true;
    }
}
