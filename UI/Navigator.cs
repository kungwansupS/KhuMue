using KhuMue.Core;
using KhuMue.UI.Pages;

namespace KhuMue.UI;

/// <summary>A screen of the guide. Pages build a node tree; the window hosts and scrolls it.</summary>
internal abstract class Page
{
    /// <summary>Set to rebuild the node tree on the next frame (filters changed, world changed...).</summary>
    public bool Dirty = true;

    /// <summary>Scroll position restored when returning through history.</summary>
    public float SavedScroll;

    /// <summary>Set together with <see cref="Dirty"/> to start the rebuilt page at the top.</summary>
    public bool ScrollToTop;

    public abstract string Title { get; }

    /// <summary>Sidebar section this page belongs to.</summary>
    public abstract Section Section { get; }

    public abstract Node Build(int width);

    /// <summary>Optional node pinned above the scrolling area (search bars, filters).</summary>
    public virtual Node? BuildHeader(int width) => null;

    /// <summary>Called every update while visible.</summary>
    public virtual void Tick()
    {
    }

    public virtual void OnShow()
    {
    }

    /// <summary>Two pages with the same key are the same destination (history de-duplication).</summary>
    public virtual string Key => GetType().Name;
}

internal enum Section
{
    Home,
    Progression,
    Items,
    Loadouts,
    Upgrades,
    Guides,
    Settings,
}

/// <summary>Back/forward history of pages. Everything that links (items, bosses) goes through here.</summary>
internal static class Navigator
{
    private static readonly List<Page> History = new();
    private static int _index = -1;

    internal static Page? Current => _index >= 0 && _index < History.Count ? History[_index] : null;
    internal static bool CanGoBack => _index > 0;
    internal static bool CanGoForward => _index >= 0 && _index < History.Count - 1;

    /// <summary>Raised when the current page changes.</summary>
    internal static event Action? Changed;

    internal static void Reset()
    {
        History.Clear();
        _index = -1;
    }

    internal static void Go(Page page)
    {
        Page? current = Current;
        if (current is not null && current.Key == page.Key)
        {
            current.Dirty = true;
            return;
        }
        if (current is not null)
            current.SavedScroll = GuideWindow.CurrentScroll;
        if (_index < History.Count - 1)
            History.RemoveRange(_index + 1, History.Count - _index - 1);
        History.Add(page);
        if (History.Count > 60)
            History.RemoveAt(0);
        _index = History.Count - 1;
        page.Dirty = true;
        page.SavedScroll = 0;
        page.OnShow();
        TextBox.BlurAll();
        Changed?.Invoke();
    }

    internal static void Back()
    {
        if (!CanGoBack)
            return;
        Current!.SavedScroll = GuideWindow.CurrentScroll;
        _index--;
        Current!.Dirty = true;
        Current.OnShow();
        TextBox.BlurAll();
        Sounds.Click();
        Changed?.Invoke();
    }

    internal static void Forward()
    {
        if (!CanGoForward)
            return;
        Current!.SavedScroll = GuideWindow.CurrentScroll;
        _index++;
        Current!.Dirty = true;
        Current.OnShow();
        TextBox.BlurAll();
        Sounds.Click();
        Changed?.Invoke();
    }

    internal static void OpenSection(Section section)
    {
        Go(section switch
        {
            Section.Progression => new ProgressionPage(),
            Section.Items => new ItemsPage(),
            Section.Loadouts => new LoadoutsPage(),
            Section.Upgrades => new UpgradesPage(),
            Section.Guides => new GuidesPage(),
            Section.Settings => new SettingsPage(),
            _ => new HomePage(),
        });
    }

    internal static void OpenItem(int type)
    {
        if (type <= 0 || type >= ItemLoader.ItemCount)
            return;
        Sounds.Click();
        Go(new ItemPage(type));
    }

    internal static void OpenEntry(string id)
    {
        if (!GuideData.EntriesById.ContainsKey(id))
            return;
        Sounds.Click();
        Go(new EntryPage(id));
    }

    internal static void Follow(Link link)
    {
        switch (link.Kind)
        {
            case LinkKind.Item:
                OpenItem(link.Item);
                break;
            case LinkKind.Entry when link.Entry is not null:
                OpenEntry(link.Entry);
                break;
        }
    }
}
