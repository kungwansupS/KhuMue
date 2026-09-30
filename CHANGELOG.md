# Changelog

## 2.2.0 (2026-09-25)

### Added
- Item pages show sources the game keeps no registry for. `tools/data/obtain_sources.py` reads them from the
  game code into `Data/obtain.json`, and stops if an id is not where it is expected, so a game update that
  moves loot cannot slip through:
  - **Chests** (176 items): which chest holds the item, where it generates and what opens it. Vanilla chests
    (Gold, Frozen, Ivy, Skyware, Shadow, Dungeon and biome chests ...) and Calamity structures (Abyss, the
    Astral Chest in the Dungeon, Brimstone Crag, Sulphurous Sea, shrines, Draedon labs, Vernal Pass ...).
    Loot Calamity removes or swaps (Step Stool, Fledgling Wings) is hidden while Calamity is loaded.
  - **Fishing** (178 items) with the water, biome, height and progress each catch needs.
  - **Angler quest rewards** (52 items).
- Item pages add up the base materials of a recipe: every craftable part is broken down along its first recipe
  (bars into ores, platforms into wood ...), with the crafting stations used on the way. Recipe groups and
  two-way recipes (Wood and Wood Platform) stay as they are.
- New article **Fishing**: fishing power, pond size, lava and honey, crates and the Angler's quests, with the
  numbers taken from the game code.
- Items without any known source get a hint where one applies: developer sets, Angler quest fish, music boxes,
  furniture and blocks.
- Boss pages say when a step is not synced in multiplayer (Infernum's Bereft Vassal; Wrath of the Gods' Solyn
  steps unless WotGMultiplayerFix is loaded).
- Hunt of the Old God article: the Shadowspec curse. Difficulty article: Calamity switches Revengeance to Death
  (or Malice) by itself in Master and Legendary worlds.

### Changed
- Thai wording reviewed throughout (about 140 texts), e.g. "ทำได้แล้ว" is now "สามารถทำได้".
- While the chat box is closed, chat lines are drawn under the guide window instead of over its buttons.

### Fixed
- Item tags with options in other mods' tooltip lines (e.g. `[i/s10:225]`) showed as raw text on item pages;
  they are drawn as the item icon with its amount.

### QA
- `tools/RunClientQa.ps1 -Multiplayer` starts a dedicated server with the same mods and fixture world and lets
  the test client join it. `tools/qa-plan-mp.json` records the route state of every entry, to compare a
  multiplayer client with a single-player run of the same world.

## 2.1.0 (2026-09-25)

Published as a new Workshop item, so the internal name changed from `BetterGuide` to `KhuMue` (tModLoader ties a
Workshop item to the internal name).

- An existing `BetterGuide/ToggleGuide` key binding is taken over once. Settings, the preferred class and the
  "already opened" flag start fresh, so the guide opens once more for existing characters.
- tModLoader leaves new mod keys unbound (the default key only applies after a reset in Settings > Controls). While no
  key is set, the Home and Settings pages and a one-time chat line say how to set one, and help texts no longer
  claim that B works.
- `/khumue` in chat opens or closes the guide.
- Texts moved from `Mods.BetterGuide.*` to `Mods.KhuMue.*`; the cross-mod API is now `ModLoader.GetMod("KhuMue")`.

## 2.0.2 (2026-09-25)

- Renamed to **Khu Mue [Calamity]** ("khu mue", คู่มือ, is Thai for "guide"). The internal name stays
  `BetterGuide`, so key bindings, settings and character data carry over. The window, the Controls entry and
  the chat notice use the new name.
- Workshop description (`description_workshop.txt`) and change notes (`changelog.txt`).

## 2.0.1 (2026-09-24)

### Fixed
- With the game's UI Scale above 100% the window was drawn too small and toward the top-left corner: the
  screen size was divided by the UI scale twice.
- Clicks inside the window did nothing: the guide read Terraria's click flag at a point of the frame where
  the game had already cleared it. Presses are now taken right after the game reads input, and a click on the
  window no longer reaches inventory slots underneath it.

### QA
- `tools/RunClientQa.ps1 -UIScale` sets the client's UI scale; the QA probe now sends real mouse clicks
  (sidebar, back, buttons, item grid, close) and checks which page the guide shows afterwards.

## 2.0.0 (2026-09-24)

A new Better Guide, built from scratch. The internal mod name stays `BetterGuide`, so the key binding and
character data carry over. The 1.0.x prototype is archived in `_archive/betterguide-prototype-1.0.6`.

### New
- One window with seven tabs: Home, Progression, Gear, Items, Upgrades, Guides, Settings; back/forward
  history (mouse back button), Esc to close, smooth scrolling, readable text sizes, compact layout for small
  windows.
- Everything the game knows is read live: recipes (with recipe groups and stations), NPC and bag drops with
  chances, shops, Shimmer results, bug-net catches, banners, stats, tooltips. No stored copies to go stale.
  Boss pages list the drops of the world's difficulty (the Treasure Bag replaces Classic-only drops in
  Expert and Master).
- 93 bosses, events and quests across Terraria, Calamity, Catalyst, Hunt of the Old God, Wrath of the Gods
  (Solyn's quests), Infernum, Calamity Fables and Spirit Reforged, each with how to start it, what it unlocks,
  tips and warnings, and live done/next/available/locked status from world flags.
- Recommended gear for every class and stage: Calamity Mod Wiki (19 stages, 5 classes) with Calamity loaded,
  Terraria Wiki revision 957024 (9 stages, 4 classes + gear for all classes) without it.
- Permanent upgrades page, including Calamity fruits and hearts, Rage and Adrenaline boosters, Hunt's Auric
  Souls and the 1.4.4 Shimmer boosters, with what the current character has used.
- 24 guide articles: basics, Calamity mechanics (difficulties, Rage/Adrenaline, Rogue, summoner penalty,
  defense damage, biomes, Acid Rain, Draedon's labs, town NPCs, Boss Rush), add-on mods and common traps.
- Full Thai translation.
- Errors inside the guide are logged and shown as a message on the page instead of closing the game.
- Default key changed from G to B. B is also vanilla's Quick Buff key: the Settings page warns about any clash and
  each character gets a one-time chat notice.

### Fixed compared with the prototype
See `docs/DATA-VERIFICATION.md`. Highlights: Crabulon never needed the Desert Scourge; the Devourer of Gods
does not need the Sentinels in 2.2.4; Cryogen is not required before the mechanical bosses; Hunt's Auric
Souls drop every kill until absorbed; the Uzi is no longer recommended before the mechanical bosses.

### Removed
- The prototype's graph view and "tier" labels.
- The prototype's rarity workaround for Better Expert Rarity + Daybreak. In-game QA with both mods shows
  tooltips of custom rarities (Auric Bar, Shadowspec Bar) without the old black screen, so the guide no
  longer changes item rarities while tooltips are drawn.
