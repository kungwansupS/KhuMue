using KhuMue.Core;
using static KhuMue.UI.Pages.PageKit;

namespace KhuMue.UI.Pages;

/// <summary>Short how-to articles, filtered to the mods that are loaded.</summary>
internal sealed class GuidesPage(string? article = null) : Page
{
    private readonly string? _article = article;

    public override string Title => _article is null ? L("Guides.Title") : L("Guides." + _article + ".Title");
    public override Section Section => Section.Guides;
    public override string Key => "Guides:" + (_article ?? "");

    private static bool Visible(GuideArticle a) => a.Mods.All(m => m == "Terraria" || ModLoader.HasMod(m));

    public override Node Build(int width)
    {
        if (_article is not null && GuideData.Articles.FirstOrDefault(a => a.Id == _article) is GuideArticle article)
            return ArticleView(article);

        var page = new VStack { Spacing = 14 };
        page.Add(Muted(L("Guides.Intro", KhuMue.ToggleKeyText())));
        foreach (IGrouping<string, GuideArticle> category in GuideData.Articles.Where(Visible).GroupBy(a => a.Category))
        {
            VStack section = PageKit.Section(L("Guides.Categories." + category.Key));
            var flow = new Flow { Spacing = 8, RowSpacing = 8, CellWidth = Math.Max(240, (width - 16) / 2) };
            foreach (GuideArticle a in category)
            {
                GuideArticle item = a;
                var row = new HStack { Spacing = 10 };
                int icon = GuideCatalog.ItemType(a.Icon);
                row.AddFixed(new ItemSlotNode(icon, 38) { OnClick = null, TooltipItem = 0, ShowOwned = false }, 40);
                var text = new VStack { Spacing = 0 };
                text.Add(new Label(L("Guides." + a.Id + ".Title"), Theme.Text, Theme.Body));
                if (Has("Guides." + a.Id + ".Summary"))
                    text.Add(new Label(L(Variant("Guides." + a.Id + ".Summary")), Theme.TextMuted, Theme.Small));
                row.AddFlex(text);
                flow.Add(new Card(row) { Padding = 8, OnClick = () => { Sounds.Click(); Navigator.Go(new GuidesPage(item.Id)); } });
            }
            section.Add(flow);
            page.Add(section);
        }
        return page;
    }

    private static Node ArticleView(GuideArticle article)
    {
        var page = new VStack { Spacing = 14 };
        if (Has("Guides." + article.Id + ".Summary"))
            page.Add(new Paragraph(L(Variant("Guides." + article.Id + ".Summary")), Theme.TextMuted, Theme.Body));
        foreach (string section in article.Sections)
        {
            string prefix = "Guides." + article.Id + "." + section;
            if (!Has(prefix + ".Body"))
                continue;
            VStack block = PageKit.Section(Has(prefix + ".Title") ? L(prefix + ".Title") : section);
            block.Add(new Paragraph(L(Variant(prefix + ".Body"), KhuMue.ToggleKeyText())));
            page.Add(block);
        }
        if (Has("Guides." + article.Id + ".Source"))
            page.Add(Muted(L("Guides." + article.Id + ".Source")));
        return page;
    }
}
