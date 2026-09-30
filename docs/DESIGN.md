# Design notes (Khu Mue [Calamity] 2.x)

## Goals

1. **Never wrong about the game.** Anything the running game can answer (recipes, drops, shops, Shimmer,
   stats, names) is read live. Only knowledge the game cannot report is curated: the route order, how to
   start each fight, unlocks, tips, gear recommendations. Each curated fact is traceable to code
   (`docs/DATA-VERIFICATION.md`).
2. **Answer "what now?" in one glance.** The Home page shows the world's stage, the next main step with
   its summon items and how to get them, and a short list of other things available now.
3. **Every item is one click away.** Item icons anywhere in the guide open the item page; entry names in
   text are links; the mouse back button returns.
4. **Readable in Thai and English.** Plain words, short sentences, English proper names inside Thai text,
   Thai-aware line breaking, text size and window size options.

## Structure

- `GuideData` loads the curated JSON; `GuideCatalog` builds live indexes once per load (recipes by result
  and ingredient, NPC and bag drops, shops, Shimmer, bug-net catches, banners).
- `Flags` maps short names (`v.Plantera`, `cal.Yharon`, `wotg.Stargaze` ...) to world state. Unreadable
  flags return "unknown" instead of guessing. `GuideCondition` evaluates `&`, `|`, `!`, parentheses and
  `mod:Name` over them with three-valued logic.
- `ProgressService` turns entries into Done / Next / Available / Locked / Unknown and picks the stage from
  the stage ladder (Calamity's ladder when Calamity is loaded, Terraria's otherwise).
- The UI is a small retained node tree (`UI/Nodes.cs`, `UI/Widgets.cs`) drawn in the UI layer. Pages are
  rebuilt only when dirty or when size, text scale, UI scale or language changes.

## Texts

All texts are under `Mods.KhuMue.Guide.*` so an older translation pack cannot override them. A key
with the suffix `Cal` (for example `Entries.BloodMoon.SummonCal`) replaces the base text while Calamity is
loaded, for vanilla content whose rules Calamity changes. The tables in `tools/data/loc/` hold both
languages side by side; `tools/data/localization.py` writes the `.hjson` files.
