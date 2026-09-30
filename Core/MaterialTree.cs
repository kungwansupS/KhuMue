namespace KhuMue.Core;

/// <summary>One base material of a crafting tree: an item, or any item of a recipe group.</summary>
internal readonly record struct Material(int Type, int Count, RecipeGroup? Group);

/// <summary>The raw materials and stations behind one recipe, with every craftable part crafted as well.</summary>
internal sealed class MaterialTotals
{
    public List<Material> Materials { get; } = new();
    public List<int> Stations { get; } = new();

    /// <summary>True when at least one ingredient was broken down (otherwise the recipe itself says it all).</summary>
    public bool Expanded { get; set; }
}

internal static class MaterialTree
{
    private const int MaxDepth = 8;

    /// <summary>
    /// Follows the recipe down to items that have no recipe: every craftable ingredient is replaced by the
    /// ingredients of its first recipe (the one the item page lists first). Recipe groups ("any Iron Bar") and
    /// loops such as Wood and Wood Platform are kept as they are. Amounts are summed exactly and rounded up once
    /// at the end, so recipes that make stacks (e.g. 70 Musket Balls) are not counted once per use.
    /// </summary>
    internal static MaterialTotals Expand(Recipe recipe)
    {
        var totals = new MaterialTotals();
        var amounts = new Dictionary<(int Type, int Group), double>();
        var order = new List<(int Type, int Group)>();
        var groups = new Dictionary<int, RecipeGroup>();
        var path = new HashSet<int> { recipe.createItem.type };

        void AddStations(Recipe r)
        {
            foreach (int tile in r.requiredTile)
            {
                if (!totals.Stations.Contains(tile))
                    totals.Stations.Add(tile);
            }
        }

        void Walk(Recipe r, double crafts, int depth)
        {
            AddStations(r);
            foreach (Item ingredient in r.requiredItem)
            {
                if (ingredient.IsAir)
                    continue;
                double need = ingredient.stack * crafts;
                RecipeGroup? group = GuideCatalog.GroupFor(r, ingredient);
                if (group is null && depth < MaxDepth && !path.Contains(ingredient.type) && FirstRecipe(ingredient.type) is Recipe sub)
                {
                    totals.Expanded = true;
                    path.Add(ingredient.type);
                    Walk(sub, need / Math.Max(1, sub.createItem.stack), depth + 1);
                    path.Remove(ingredient.type);
                    continue;
                }
                var key = (ingredient.type, group?.RegisteredId ?? -1);
                if (group is not null)
                    groups[group.RegisteredId] = group;
                if (!amounts.ContainsKey(key))
                {
                    amounts[key] = 0;
                    order.Add(key);
                }
                amounts[key] += need;
            }
        }

        Walk(recipe, 1, 0);
        foreach (var key in order)
        {
            int count = (int)Math.Ceiling(amounts[key] - 1e-9);
            totals.Materials.Add(new Material(key.Type, Math.Max(1, count), key.Group >= 0 ? groups[key.Group] : null));
        }
        return totals;
    }

    /// <summary>The recipe an ingredient is broken down with, or null when it counts as a base material.</summary>
    private static Recipe? FirstRecipe(int type)
    {
        IReadOnlyList<Recipe> recipes = GuideCatalog.RecipesFor(type);
        if (recipes.Count == 0)
            return null;
        Recipe first = recipes[0];
        // Two-way recipes (1 Wood -> 2 Wood Platforms, 2 Wood Platforms -> 1 Wood): only the direction that
        // multiplies is a real crafting step, so a platform breaks down into wood but wood stays a base material.
        foreach (Item part in first.requiredItem)
        {
            if (part.type == type)
                return null;
            IReadOnlyList<Recipe> back = GuideCatalog.RecipesFor(part.type);
            if (back.Count > 0 && back[0].requiredItem.Any(i => i.type == type) && first.createItem.stack <= part.stack)
                return null;
        }
        return first;
    }
}
