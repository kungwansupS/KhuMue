# Third-party notices

Khu Mue [Calamity]'s code (internal name `KhuMue`) is released under the MIT License (`LICENSE`).
The project authors' rights in the mod icons and reusable icon frame are dedicated
to the public domain under CC0 1.0 Universal; see `LICENSE-ART` for the file list and terms.
Data adapted from community wikis keeps the source licenses described below.

## Calamity Mod Wiki: recommended gear (Calamity)

- Files: `Data/loadouts-calamity.json`, and the English texts of `Mods.KhuMue.Guide.LoadoutNotes.C*`
  (written in `tools/data/loc/loc_misc.py`) with their Thai translations.
- Source: "Guide:Class setups" and its Cargo table `ClassSetups`, Calamity Mod Wiki,
  <https://calamitymod.wiki.gg/wiki/Guide:Class_setups>, retrieved 2026-09-24 (guide written for Calamity 2.1.2).
- License: Creative Commons Attribution-NonCommercial-ShareAlike 3.0,
  <https://creativecommons.org/licenses/by-nc-sa/3.0/>.
- Changes: converted to item keys, merged the "Pre-Exo Mechs" and "Pre-Supreme Calamitas" stages into one,
  removed items that cannot be obtained at their stage in Calamity 2.2.4 (the Uzi before the mechanical
  bosses), and shortened and translated the notes.

## Terraria Wiki: recommended gear (vanilla)

- Files: `Data/loadouts-vanilla.json`, and the texts of `Mods.KhuMue.Guide.LoadoutNotes.V*`.
- Source: "Guide:Class setups", Terraria Wiki, revision 957024,
  <https://terraria.wiki.gg/wiki/Guide:Class_setups?oldid=957024>, retrieved 2026-09-24.
- License: Creative Commons Attribution-NonCommercial-ShareAlike 4.0,
  <https://creativecommons.org/licenses/by-nc-sa/4.0/>.
- Changes: converted to item keys (mounts to their summoning items), kept the "Best" and crowd-control
  markers, translated box titles into notes.

These adapted files are shared under the same licenses as their sources. The guide shows the source,
version and license under every gear page and in Settings > About.

## Games and mods

Terraria is a trademark of Re-Logic. Calamity Mod, Catalyst, Calamity: Hunt of the Old God, Wrath of the
Gods, Infernum Mode, Calamity Fables and Spirit Reforged belong to their authors. Khu Mue contains
none of their code or art: names, pictures and stats are read from the installed mods at run time.

## Source snapshots and generator inputs

The public source also retains the inputs needed to regenerate the loadout data:

- `tools/data/sources/data_Pre-Hardmode.wikitext`, `data_Hardmode.wikitext` and
  `data_Post-Moon_Lord.wikitext`: Calamity Mod Wiki class setup snapshots, CC BY-NC-SA 3.0.
  These are inputs to the Calamity class setup data described above.
- `tools/data/sources/tr_class_setups.wikitext`: Terraria Wiki's class setup snapshot
  (revision 957024), CC BY-NC-SA 4.0, from the source linked above.
- `tools/data/loadout-notes.json`: adapted loadout notes; the `calamity` section uses
  CC BY-NC-SA 3.0 and the `vanilla` section uses CC BY-NC-SA 4.0.
- The same licenses apply to those notes embedded in `tools/data/loc/loc_misc.py`
  and the generated English and Thai localization files. Unrelated code in these files is MIT.

The MIT license does not replace these source licenses. Attribution and descriptions of the
changes are retained above. Raw dumps from installed games/mods and personal QA saves are not distributed.
