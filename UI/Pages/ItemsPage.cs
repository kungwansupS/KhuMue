using KhuMue.Core;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Search every loaded item by name, with category/class/mod filters.</summary>
internal sealed class ItemsPage : Page
{
    private static string _query = "";
    private static string _category = "";
    private static string _class = "";
    private static string _mod = "";

    private readonly TextBox _search;

    public ItemsPage()
    {
        _search = new TextBox(L("Items.SearchHint")) { Value = _query };
        _search.OnChange = value =>
        {
            _query = value;
            Dirty = true;
            ScrollToTop = true;
        };
        _search.OnSubmit = () =>
        {
            List<int> results = ItemSearch.Search(_query, _category, _class, _mod);
            if (results.Count > 0)
                Navigator.OpenItem(results[0]);
        };
    }

    public override string Title => L("Items.Title");
    public override Section Section => Section.Items;

    public override Node? BuildHeader(int width)
    {
        var header = new VStack { Spacing = 8 };
        header.Add(_search);

        var categories = new Flow { Spacing = 6 };
        foreach (string category in new[] { "", "weapon", "armor", "accessory", "ammo", "potion", "tool", "material", "placeable", "other" })
        {
            string c = category;
            categories.Add(new Chip(L("Items.Categories." + (c.Length == 0 ? "all" : c)), _category == c, () =>
            {
                _category = c;
                if (c != "weapon" && c != "armor" && c != "accessory" && c != "ammo")
                    _class = "";
                Dirty = true;
                ScrollToTop = true;
                Sounds.Click();
            }));
        }
        header.Add(categories);

        var filters = new Flow { Spacing = 6 };
        if (_category is "weapon" or "armor" or "accessory" or "ammo")
        {
            filters.Add(new Chip(L("Items.AnyClass"), _class.Length == 0, () => { _class = ""; Dirty = true; ScrollToTop = true; Sounds.Click(); }));
            foreach (string cls in Classes())
            {
                string c = cls;
                filters.Add(new Chip(ClassName(c), _class == c, () => { _class = c; Dirty = true; ScrollToTop = true; Sounds.Click(); }) { Color = Theme.ClassColor(c), Icon = ClassIcon(c) });
            }
        }
        // Only mods that add a meaningful number of items get a filter chip.
        var mods = ItemSearch.All.GroupBy(e => e.Mod).Where(g => g.Count() >= 40 && g.Key != "ModLoader")
            .OrderBy(g => g.Key == "Terraria" ? 0 : 1).ThenByDescending(g => g.Count()).Take(9).Select(g => g.Key).ToList();
        if (_mod.Length > 0 && !mods.Contains(_mod))
            mods.Add(_mod);
        if (mods.Count > 1)
        {
            filters.Add(new Chip(L("Items.AnyMod"), _mod.Length == 0, () => { _mod = ""; Dirty = true; ScrollToTop = true; Sounds.Click(); }));
            foreach (string mod in mods)
            {
                string m = mod;
                string label = m == "Terraria" ? "Terraria" : Has("ModShort." + m) ? L("ModShort." + m) : SupportedMods.DisplayName(m);
                filters.Add(new Chip(label, _mod == m, () => { _mod = m; Dirty = true; ScrollToTop = true; Sounds.Click(); }));
            }
        }
        header.Add(filters);
        return header;
    }

    public override Node Build(int width)
    {
        List<int> results = ItemSearch.Search(_query, _category, _class, _mod);
        var page = new VStack { Spacing = 8 };
        page.Add(new Label(L("Items.ResultCount", results.Count), Theme.TextMuted, Theme.Small));
        if (results.Count == 0)
        {
            page.Add(Callout(L("Items.NoResults"), Theme.Info));
            return page;
        }
        page.Add(new ItemGrid { Items = results, Cell = 42, CellWidth = 230 });
        return page;
    }
}
