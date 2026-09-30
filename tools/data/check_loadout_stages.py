"""
Sanity check for Data/loadouts-calamity.json: estimates the earliest Calamity class-setup stage at
which each recommended item can be obtained, from the live data dump, and lists rows that are
recommended before that stage.

This is a review aid only (the guide never shows these estimates): it propagates boss drops,
shop conditions and a small table of progression-gated materials through the recipe graph.

  python tools/data/check_loadout_stages.py <calamity-dump-folder>
"""
import json
import os
import re
import sys
from collections import defaultdict

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.normpath(os.path.join(HERE, "..", ".."))

STAGES = ["pre-boss", "pre-evil1", "pre-evil2", "pre-skeletron", "pre-wof", "pre-mech", "post-mech1", "post-mech2",
          "pre-plantera", "pre-golem", "post-golem", "pre-lunar", "pre-moonlord", "pre-provi", "pre-polter", "pre-dog",
          "pre-yharon", "pre-scal-exo", "endgame"]
S = {s: i for i, s in enumerate(STAGES)}

# Boss Checklist key -> the class-setup stage in which that boss is fought (its drops are usable there).
BOSS_STAGE = {
    "Terraria KingSlime": 0, "Terraria EyeofCthulhu": 0, "CalamityMod DesertScourge": 0, "CalamityMod GiantClam": 1,
    "CalamityMod Crabulon": 1, "Terraria EaterofWorlds": 1, "Terraria BrainofCthulhu": 1, "CalamityMod AcidRainT1": 1,
    "Terraria GoblinArmy": 1, "Terraria BloodMoon": 1, "Terraria OldOnesArmy": 2, "Terraria DD2DarkMageT3": 2,
    "CalamityMod HiveMind": 2, "CalamityMod Perforators": 2, "Terraria QueenBee": 3, "Terraria Deerclops": 3,
    "Terraria Skeletron": 3, "CalamityMod SlimeGod": 4, "Terraria WallofFlesh": 4, "Terraria TorchGod": 0,
    "Terraria QueenSlimeBoss": 5, "CalamityMod Cryogen": 5, "Terraria PirateInvasion": 5, "Terraria PirateShip": 5,
    "Terraria FrostLegion": 5, "Terraria TheTwins": 5, "Terraria TheDestroyer": 5, "Terraria SkeletronPrime": 5,
    "CalamityMod AquaticScourge": 6, "CalamityMod AcidRainT2": 6, "CalamityMod CragmawMire": 6, "Terraria Eclipse": 6,
    "Terraria DD2OgreT3": 6, "CalamityMod BrimstoneElemental": 7, "CalamityMod CalamitasClone": 8,
    "Terraria Plantera": 8, "CalamityMod GreatSandShark": 9, "CalamityMod Leviathan": 9, "CalamityMod AstrumAureus": 9,
    "Terraria Golem": 9, "Terraria PumpkinMoon": 10, "Terraria MourningWood": 10, "Terraria Pumpking": 10,
    "Terraria FrostMoon": 10, "Terraria Everscream": 10, "Terraria SantaNK1": 10, "Terraria IceQueen": 10,
    "Terraria MartianMadness": 10, "Terraria MartianSaucer": 10, "Terraria DukeFishron": 10,
    "CalamityMod PlaguebringerGoliath": 10, "Terraria HallowBoss": 10, "Terraria DD2Betsy": 10, "CalamityMod Ravager": 10,
    "Terraria CultistBoss": 11, "Terraria LunarEvent": 12, "CalamityMod AstrumDeus": 12, "Terraria MoonLord": 12,
    "CalamityMod ProfanedGuardians": 13, "CalamityMod Dragonfolly": 13, "CalamityMod Providence": 13,
    "CalamityMod CeaselessVoid": 14, "CalamityMod StormWeaver": 14, "CalamityMod Signus": 14, "CalamityMod Polterghast": 14,
    "CalamityMod AcidRainT3": 15, "CalamityMod Mauler": 15, "CalamityMod NuclearTerror": 15, "CalamityMod OldDuke": 15,
    "CalamityMod DevourerofGods": 15, "CalamityMod Yharon": 16, "CalamityMod ExoMechs": 17, "CalamityMod Calamitas": 17,
    "CalamityMod BossRush": 18,
}

# Materials whose availability is gated by world progression rather than by a boss drop table.
MATERIAL_STAGE = {
    "Terraria/SoulofLight": 5, "Terraria/SoulofNight": 5, "Terraria/SoulofFlight": 5, "Terraria/PixieDust": 5,
    "Terraria/CrystalShard": 5, "Terraria/UnicornHorn": 5, "Terraria/CursedFlame": 5, "Terraria/Ichor": 5,
    "Terraria/CobaltBar": 5, "Terraria/PalladiumBar": 5, "Terraria/CobaltOre": 5, "Terraria/PalladiumOre": 5,
    "Terraria/MythrilBar": 6, "Terraria/OrichalcumBar": 6, "Terraria/MythrilOre": 6, "Terraria/OrichalcumOre": 6,
    "Terraria/AdamantiteBar": 7, "Terraria/TitaniumBar": 7, "Terraria/AdamantiteOre": 7, "Terraria/TitaniumOre": 7,
    "Terraria/SoulofMight": 5, "Terraria/SoulofSight": 5, "Terraria/SoulofFright": 5, "Terraria/HallowedBar": 5,
    "Terraria/ChlorophyteBar": 8, "Terraria/ChlorophyteOre": 8, "Terraria/Ectoplasm": 9, "Terraria/SpectreBar": 9,
    "Terraria/ShroomiteBar": 9, "Terraria/TurtleShell": 5, "Terraria/BeetleHusk": 10, "Terraria/LunarTabletFragment": 9,
    "Terraria/FragmentSolar": 11, "Terraria/FragmentVortex": 11, "Terraria/FragmentNebula": 11, "Terraria/FragmentStardust": 11,
    "Terraria/LunarBar": 12, "Terraria/LunarOre": 12, "Terraria/Hellstone": 2, "Terraria/HellstoneBar": 2,
    "Terraria/Meteorite": 2, "Terraria/MeteoriteBar": 2,
    "CalamityMod/AerialiteBar": 3, "CalamityMod/AerialiteOre": 3, "CalamityMod/EssenceofEleum": 5,
    "CalamityMod/EssenceofHavoc": 5, "CalamityMod/EssenceofSunlight": 5, "CalamityMod/CryonicBar": 7,
    "CalamityMod/CryonicOre": 7, "CalamityMod/PerennialBar": 9, "CalamityMod/PerennialOre": 9,
    "CalamityMod/ScoriaBar": 10, "CalamityMod/ScoriaOre": 10, "CalamityMod/AstralBar": 12, "CalamityMod/AstralOre": 12,
    "CalamityMod/UelibloomBar": 14, "CalamityMod/UelibloomOre": 14, "CalamityMod/DivineGeode": 14,
    "CalamityMod/Bloodstone": 14, "CalamityMod/BloodstoneCore": 14, "CalamityMod/RuinousSoul": 15,
    "CalamityMod/CosmiliteBar": 16, "CalamityMod/AuricBar": 17, "CalamityMod/AuricOre": 17,
    "CalamityMod/DarksunFragment": 16, "CalamityMod/EndothermicEnergy": 16, "CalamityMod/NightmareFuel": 16,
    "CalamityMod/AscendantSpiritEssence": 16, "CalamityMod/ExoPrism": 18, "CalamityMod/AshesofAnnihilation": 18,
    "CalamityMod/ShadowspecBar": 18, "CalamityMod/UnholyEssence": 13, "CalamityMod/Necroplasm": 13,
    "CalamityMod/Lumenyl": 9, "CalamityMod/DepthCells": 9, "CalamityMod/PlagueCellCanister": 10,
    "CalamityMod/InfectedArmorPlating": 10, "CalamityMod/LifeAlloy": 8, "CalamityMod/CoreofCalamity": 8,
    "CalamityMod/GalacticaSingularity": 12, "CalamityMod/MeldBlob": 11, "CalamityMod/MeldConstruct": 11,
    "CalamityMod/StarblightSoot": 5, "CalamityMod/AstralMonolith": 5, "CalamityMod/Stardust": 5,
    "CalamityMod/UnholyCore": 5, "CalamityMod/SolarVeil": 8, "CalamityMod/GrandScale": 9,
    "CalamityMod/ArmoredShell": 14, "CalamityMod/TwistingNether": 14, "CalamityMod/DarkPlasma": 14,
    "CalamityMod/EffulgentFeather": 13, "CalamityMod/ReaperTooth": 15, "CalamityMod/Polypebble": 5,
    "CalamityMod/CorrodedFossil": 6, "CalamityMod/InfernalSuevite": 6, "CalamityMod/SeaPrism": 1,
    "CalamityMod/PearlShard": 0, "CalamityMod/PurifiedGel": 4, "CalamityMod/BlightedGel": 1,
    "CalamityMod/DubiousPlating": 0, "CalamityMod/MysteriousCircuitry": 0, "CalamityMod/YharonSoulFragment": 17,
}

SHOP_TEXT = [
    (r"Calamitas Clone", 8), (r"Supreme Witch|Supreme Calamitas", 18), (r"Moon Lord", 13), (r"Plantera", 9), (r"Golem", 10), (r"Wall of Flesh|Hardmode", 5), (r"Skeletron Prime", 6),
    (r"Destroyer|Twins|mechanical boss", 6), (r"all Mechanical|all three Mechanical", 8), (r"Providence", 14),
    (r"Polterghast", 15), (r"Devourer", 16), (r"Yharon", 17), (r"Old Duke", 15), (r"Skeletron", 4),
    (r"Lunatic Cultist", 12), (r"Cryogen", 5), (r"Astrum Deus", 12),
]


def main():
    dump = sys.argv[1]
    load = lambda n: json.load(open(os.path.join(dump, n), encoding="utf-8"))
    items = {i["key"]: i for i in load("items.json")}
    recipes = [r for r in load("recipes.json") if not r["disabled"]]
    groups = {g["name"]: g["items"] for g in load("recipeGroups.json")}
    bc = load("bosschecklist.json")
    npc_loot = load("npcLoot.json")
    item_loot = load("itemLoot.json")
    shops = load("shops.json")

    gate = {}

    def lower(key, value):
        if value < gate.get(key, 99):
            gate[key] = value
            return True
        return False

    for k, v in MATERIAL_STAGE.items():
        lower(k, v)
    for e in bc:
        stage = BOSS_STAGE.get(e["key"])
        if stage is None:
            continue
        for k in e.get("loot", []):
            lower(k, stage)
        bag = e.get("treasureBag")
        if bag:
            for d in item_loot.get(bag, []):
                lower(d["itemKey"], stage)
    for s in shops:
        for entry in s["entries"]:
            stage = 0
            for c in entry["conditions"]:
                # "After defeating A, B or C": the earliest named boss unlocks it.
                hits = [st for pattern, st in SHOP_TEXT if re.search(pattern, c["text"])]
                if hits:
                    stage = max(stage, min(hits) if " or " in c["text"] else max(hits))
            lower(entry["item"], stage)

    # Anything never reached by the above but droppable by ordinary enemies without conditions: stage 0 guess.
    for npc, drops in npc_loot.items():
        for d in drops:
            if not d["conditions"]:
                gate.setdefault(d["itemKey"], 0)

    changed = True
    rounds = 0
    while changed and rounds < 50:
        changed = False
        rounds += 1
        for r in recipes:
            worst = 0
            ok = True
            for ing in r["ingredients"]:
                k = ing["item"]
                g = None
                for gname in r["groups"]:
                    members = groups.get(gname, [])
                    if k in members:
                        g = min(gate.get(m, 99) for m in members)
                        break
                if g is None:
                    g = gate.get(k, 99 if k.startswith("CalamityMod/") else 0)
                if g >= 99:
                    ok = False
                    break
                worst = max(worst, g)
            if ok and lower(r["result"], worst):
                changed = True

    data = json.load(open(os.path.join(ROOT, "Data", "loadouts-calamity.json"), encoding="utf-8"))
    problems = []
    unknown = set()
    for row in data["rows"]:
        stage = S[row["stage"]]
        for alt in row["items"]:
            for k in alt:
                g = gate.get(k)
                if g is None or g >= 99:
                    unknown.add(k)
                    continue
                if g > stage:
                    problems.append((row["stage"], row["class"], row["type"], k, STAGES[g]))
    seen = set()
    for p in problems:
        if p[3] in seen:
            continue
        seen.add(p[3])
        print(f"EARLY  {p[0]:<13} {p[1]:<8} {p[2]:<18} {p[3]:<45} earliest≈{p[4]}")
    print(f"{len(seen)} items recommended earlier than estimated; {len(unknown)} items without an estimate")
    for k in sorted(unknown):
        print("  ?", k)


if __name__ == "__main__":
    main()
