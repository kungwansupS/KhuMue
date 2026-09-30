# Data verification (Khu Mue [Calamity] 2.x)

Everything the guide states was checked against the exact builds below. Where the game can report a
fact by itself, the guide does not store it: recipes, drops, drop chances, shops, Shimmer results, bag
contents, NPC stats and item names are read live from the running game (`Core/GuideCatalog.cs`), so they
always match the player's mod set.

## Versions checked

| Mod | Version | How it was checked |
| --- | --- | --- |
| tModLoader / Terraria | 2026.7.3.0 / 1.4.4.9 | decompiled `tModLoader.dll`, runtime dump |
| Calamity Mod | 2.2.4 | decompiled DLL, localization files, runtime dump |
| Catalyst | 1.1.8 | decompiled DLL, runtime dump |
| Calamity: Hunt of the Old God | 1.2.3 | decompiled DLL, runtime dump |
| Wrath of the Gods (NoxusBoss) | 1.2.31 | decompiled DLL, localization, runtime dump |
| Infernum Mode | 2.0.1.35 | decompiled DLL, runtime dump |
| Calamity Fables | 0.1.24 | decompiled DLL, localization, runtime dump |
| Spirit Reforged | 0.2.2.8 | decompiled DLL, runtime dump |

The versions are listed in `Core/SupportedMods.cs`; the Settings page compares them with the loaded
mods and the Home page warns when they differ.

## Method

1. **Runtime dump.** `tools/BGDataDump` is a tool-only mod. `tools/RunDataDump.ps1` loads it in an
   isolated server profile with three mod sets (vanilla, Calamity, full pack) and writes every item,
   recipe, recipe group, NPC, NPC drop rule, bag/crate rule, shop entry, Shimmer transform, buff, Boss
   Checklist registration and bestiary entry to JSON.
2. **Source reading.** Summon rules (`CanUseItem`/`UseItem`), natural spawn conditions, world flags,
   unlock messages and upgrade effects were read from the decompiled mods.
3. **Validation.** `tools/data/validate.py <dump>` checks that every item, NPC, flag, stage, entry
   link and text key referenced by the data exists, that English and Thai define the same keys with
   the same placeholders, and that no key collides with the older Thai pack.
4. **In-game QA.** `tools/RunClientQa.ps1` runs the real client in a throw-away profile with the
   test-only `tools/GuideQaProbe` mod, which builds every page (all entries, articles, loadout stages
   for every class and every item page) at two widths, flags exceptions, missing texts and raw
   placeholders, and saves screenshots in English and Thai. It also sends simulated mouse clicks through the
   game's own input step and checks the page each click opens, at interface scales 1.0 and 1.3.
   With `-Multiplayer` the client joins a dedicated server running the same mods and world: in 2.2.0 every
   page built without problems as a multiplayer client, and the route state of all 87 entries matched a
   single-player run of the same world. That world has not reached the flags known not to sync (Infernum's
   Bereft Vassal, Solyn's steps without WotGMultiplayerFix); boss pages warn about those in multiplayer.

## Errors of the prototype (1.0.6) that 2.0.0 corrects

| Prototype claim | Fact (source) |
| --- | --- |
| Crabulon requires the Desert Scourge | `DecapoditaSprout.CanUseItem` only checks the underground Glowing Mushroom biome. |
| The Devourer of Gods requires the three Sentinels | Calamity 2.2.4 has a second Cosmic Worm recipe: 40 Luminite Bars, 10 of each lunar fragment, 10 Meld Blobs, 40 Necroplasm. |
| Cryogen is required before the mechanical bosses | No code links them; the route order is a recommendation only. |
| The progression graph had cycles | 2.0.0 uses an ordered list with per-entry conditions read from the world. |
| Hunt's Auric Souls drop only on the first kill | `YharonSoulDropRule` etc. drop the soul every kill while any player in the world has not absorbed it. |
| Uzi recommended before the mechanical bosses | In 2.2.4 it comes from Angry Trappers and the Arms Dealer after Plantera; the row is excluded. |
| "Tier" jargon on every recommendation | Replaced by the wikis' own labels (Best, Crowd, Support, Changed ...), each with an explanation. |

Claims of the prototype that were confirmed and kept: the Astral Communicator stops working after the
Moon Lord unless Astrageldon was killed (`AstralCommunicator.CanUseItem`); Goozma via an Overloaded
Sludge thrown onto King Slime during a Slime Rain; Solyn's arrival after Skeletron or in Hardmode.

## Facts added in 2.0.0 and where they come from

| Fact | Source |
| --- | --- |
| Using a Rod of Discord (any Chaos State) in the Abyss summons the Primordial Wyrm at any stage | `WorldMiscUpdateSystem.TrySpawnAEoW`; Calamity game tip "RoDAbyssTip" |
| A Caustic Tear used before the Eye of Cthulhu (or Hardmode) is consumed but starts nothing | `CausticTear` (consumable) calls `AcidRainEvent.TryStartEvent`, which returns early |
| The first Acid Rain starts when a player enters the Sulphurous Sea after the Eye of Cthulhu | `AcidRainEvent.TryToStartEventNaturally` |
| Acid Rain fails after 2.5 minutes without a kill, or when the rain stops | `AcidRainEvent.Update` (9000 ticks) |
| Early Hardmode rework (default on): no ore from altars, Cobalt/Palladium at Hardmode start, next tier per mechanical boss, Hallowed ore after the third | `CalamityServerConfig.EarlyHardmodeProgressionRework`, `ILChanges.PreventSmashAltarCode`, `CalamityGlobalNPC.SpawnMechBossHardmodeOres` |
| Aerialite becomes minable (65% pickaxe) after the first Hive Mind or Perforators kill | `HiveMind.OnKill`, `PerforatorHive`, `AerialiteOre.MinPick` |
| Skeletron unlocks the Abyss's locked chests | `CalamityGlobalNPC` (`UnlockAllAbyssChests`) |
| Blood Moons need 5 used Life Crystals with Calamity (2 in vanilla) | `ILChanges.BloodMoonsRequire200MaxLife`, `Main` start-of-night code |
| Giant Clam only after the Desert Scourge; Sea King after both | `GiantClam.SpawnChance`, `SeaKing.CanTownNPCSpawn` |
| Leviathan's "elusive singer" can appear before Hardmode; killing it summons Anahita | `LeviathanStart.SpawnChance`, `LeviathanStart.HitEffect` |
| Naiad's Warhorn needs Lumenyl, which only drops after the Leviathan | runtime dump (Lumenyl drop conditions) |
| Polterghast: 30 Phantom Spirits; spirits come from strong Dungeon enemies after the Moon Lord; the count resets with the world | `CalamityNaturalSpawnBossNPC`, `CalamityGlobalNPC.CheckDead` |
| Summoner penalty: minions deal 0.75x while holding another class's weapon, with the listed exceptions | `BalancingConstants.SummonerCrossClassNerf`, `CalamityUtils.ShouldTriggerSummonPenalty` |
| Rage: +35% for 9 s (+1 s per booster). Adrenaline: 2.5x for 5 s (+20% per booster), 50% damage reduction while full (+5% per booster); true melee gets half | `BalancingConstants`, `CalamityPlayer`, `CalamityUtils.ApplyRippersToDamage` |
| Rage/Adrenaline boosters drop from bags only in Revengeance and only if not yet used | `CrabulonBag`, `SlimeGodBag` ... `ModifyItemLoot` |
| Celestial Onion: extra slot; in Master Mode it gives the Demon Heart's slot instead | `CelestialOnion.UseItem`, `ILChanges.MasterModeCelestialOnionCheck` |
| Calamity difficulty is chosen from the icon shown with the inventory open and is locked while a boss is alive | `ModeIndicatorUI` |
| A Master world turns Revengeance into Death (Malice with For the Worthy); an Expert world turns Death back into Revengeance; Classic turns both off | `DifficultyModeSystem.PostUpdateWorld` |
| Infernum forces Revengeance and Expert, and turns itself off in Master, For the Worthy and Zenith worlds | `InfernumDifficulty`, `DifficultyManagementSystem` |
| Auric Soul effects | `AuricSoulPlayer.ResetEffects` and `AllStatsUp` |
| Hunt's Shadowspec Curse (client option, on by default) blocks using or equipping every item crafted from Shadowspec Bars until Goozma is defeated | `ShadowspecLocker`, `ShadowspecItemFinder`, `Config.shadowspecCurse` |
| Vanilla boosters from Shimmer (Vital Crystal, Aegis Fruit, Arcane Crystal, Galaxy Pearl, Gummy Worm, Ambrosia, Peddler's Satchel) and their effects | `ItemID.Sets.ShimmerTransformToItem`, `Player.UpdatePermanentBoosters` and related code |
| Solyn's telescope costs 10 Gold or Platinum Bars and 15 Wood | `SolynTelescopeTile.RepairRequirements` |

## Chests, fishing and Angler rewards (2.2.0)

The game keeps no registry of chest loot, fishing catches or Angler rewards, so the runtime dump cannot see
them. `tools/data/obtain_sources.py` reads them from the decompiled code into `Data/obtain.json`:

- **Fishing**: every `rolledItemDrop` assignment of `Projectile.FishingCheck_RollItemDrop` and every catch of
  Calamity's `CalamityPlayer.CatchFish` (including the lists it builds with `AddWithCondition`), with the
  conditions of the branches on the way (liquid, biome, height, Hardmode, bosses defeated). When one catch sits
  in several branches only the conditions they share are kept.
- **Angler**: the item ids in `Player.GetAnglerReward_MainReward`, `_Decoration` and `_Bait`.
- **Chests**: world generation mixes loot with placement logic, so the chest kinds were labelled by reading
  `WorldGen.AddBuriedChest`, its callers, the Dungeon generator and Calamity's world gen (shrines, labs,
  Abyss, Astral Chest, Sulphurous Sea, Brimstone Crag, Vernal Pass, Forsaken Archive, Giant Hive, planetoids,
  the evil floating island and `WorldgenManagementSystem.PostWorldGen`, which edits vanilla chests). The
  script checks that every listed id still appears in the method or lines it was read from and stops
  otherwise. Only notable loot is listed; potions, bars, ammo, torches and coins are left out.

Facts that were checked on the way and differ from common assumptions:

- The Astral Chest is placed inside the Dungeon, not in the Astral Infection, and opens after Astrum Aureus.
- Abyss Treasure Chests open after Skeletron; Dungeon biome chests open only after Plantera (`Chest.Unlock`).
- A normal world has only the biome chest of its own evil; Calamity adds an unlocked Corruption or Crimson
  chest (Scourge of the Corruptor or Vampire Knives) on its evil floating island.
- With Calamity, the Step Stool in surface chests becomes Kylie, Fledgling Wings no longer generate in Skyware
  chests, and the Suspicious Looking Eye in cavern Gold Chests becomes the Enchanted Knife Staff.
- Calamity's shrine chests hold biome keys: Corruption, Crimson, Desert and Frozen Keys, and the mushroom
  shrine three Truffle Worms.

## Recommended gear

`Data/loadouts-calamity.json` is generated by `tools/data/loadouts.py` from the Calamity Mod Wiki
"Guide:Class setups" (Cargo data, written for 2.1.2) and checked against the 2.2.4 dump: every item key
must exist, and `tools/data/check_loadout_stages.py` flags items that cannot be obtained at their stage.
`Data/loadouts-vanilla.json` comes from the Terraria Wiki "Guide:Class setups" revision 957024, the last
revision before the 1.4.5 changes, because tModLoader 2026.7 runs Terraria 1.4.4.9. Both are adapted
under CC BY-NC-SA (see `THIRD_PARTY_NOTICES.md`).

A bug found while building 2.0.0: box ends written as `{{infocard/box/end|title=Best}}` were not
recognized, so later boxes on the Terraria wiki page were nested wrongly (accessories typed as armor,
flasks as weapons) and the "Mixed" card for all classes was skipped. The parser was fixed before the data
was shipped.
