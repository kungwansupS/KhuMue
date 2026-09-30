"""
Source of Data/progression.json.

Every fact here was checked against the exact mod builds listed in Core/SupportedMods.cs:
summon item CanUseItem/UseItem code, recipes and drops from the live data dump
(tools/RunDataDump.ps1), Boss Checklist registrations and world flags. See
docs/DATA-VERIFICATION.md for the evidence behind each entry.

Run from the mod folder:  python tools/data/progression.py
"""
import json
import os

E = []


def add(id, mod, kind, era, order, path="optional", gate=False, npcs=(), summons=(), done="never",
        available=None, show=None, parent=None, replaces=(), repeatable=False, icon=None):
    e = {"id": id, "mod": mod, "kind": kind, "era": era, "order": order, "path": path}
    if gate:
        e["gate"] = True
    if repeatable:
        e["repeatable"] = True
    e["npcs"] = list(npcs)
    if summons:
        e["summons"] = list(summons)
    e["done"] = done
    if available:
        e["available"] = available
    if show:
        e["show"] = show
    if parent:
        e["parent"] = parent
    if replaces:
        e["replaces"] = list(replaces)
    if icon:
        e["icon"] = icon
    E.append(e)


T = "Terraria"
C = "CalamityMod"

# ---------------------------------------------------------------- Terraria 1.4.4
add("KingSlime", T, "boss", "pre", 1.0, npcs=["Terraria/KingSlime"], summons=["Terraria/SlimeCrown"], done="v.KingSlime")
add("TorchGod", T, "event", "pre", 1.5, npcs=["Terraria/TorchGod"], summons=["Terraria/Torch"], done="p.TorchGod")
add("EyeOfCthulhu", T, "boss", "pre", 2.0, "main", npcs=["Terraria/EyeofCthulhu"], summons=["Terraria/SuspiciousLookingEye"], done="v.EyeOfCthulhu")
add("BloodMoon", T, "event", "pre", 2.5, npcs=["Terraria/BloodZombie"], summons=["Terraria/BloodMoonStarter"], repeatable=True)
add("EaterOfWorlds", T, "boss", "pre", 3.0, "main", npcs=["Terraria/EaterofWorldsHead"], summons=["Terraria/WormFood"], done="v.EvilBoss", show="w.Corruption | w.Drunk")
add("BrainOfCthulhu", T, "boss", "pre", 3.0, "main", npcs=["Terraria/BrainofCthulhu"], summons=["Terraria/BloodySpine"], done="v.EvilBoss", show="w.Crimson | w.Drunk")
add("GoblinArmy", T, "event", "pre", 3.33, npcs=["Terraria/GoblinSummoner"], summons=["Terraria/GoblinBattleStandard"], done="v.GoblinArmy")
add("OldOnesArmy", T, "event", "pre", 3.66, npcs=["Terraria/DD2DarkMageT1"], summons=["Terraria/DD2ElderCrystal", "Terraria/DD2ElderCrystalStand"], done="v.OldOnesArmy1", available="v.EvilBoss")
add("DarkMage", T, "miniboss", "pre", 3.67, npcs=["Terraria/DD2DarkMageT1"], done="v.OldOnesArmy1", available="v.EvilBoss", parent="OldOnesArmy")
add("QueenBee", T, "boss", "pre", 4.0, npcs=["Terraria/QueenBee"], summons=["Terraria/Abeemination"], done="v.QueenBee")
add("Skeletron", T, "boss", "pre", 5.0, "main", npcs=["Terraria/SkeletronHead"], summons=["Terraria/ClothierVoodooDoll"], done="v.Skeletron")
add("Deerclops", T, "boss", "pre", 6.0, npcs=["Terraria/Deerclops"], summons=["Terraria/DeerThing"], done="v.Deerclops")
add("WallOfFlesh", T, "boss", "pre", 7.0, "main", gate=True, npcs=["Terraria/WallofFlesh"], summons=["Terraria/GuideVoodooDoll"], done="v.Hardmode")
add("FrostLegion", T, "event", "hm", 7.33, npcs=["Terraria/SnowmanGangsta"], summons=["Terraria/SnowGlobe"], done="v.FrostLegion", available="v.Hardmode")
add("PirateInvasion", T, "event", "hm", 7.66, npcs=["Terraria/PirateCaptain"], summons=["Terraria/PirateMap"], done="v.Pirates", available="v.Hardmode")
add("FlyingDutchman", T, "miniboss", "hm", 7.67, npcs=["Terraria/PirateShip"], done="v.Pirates", available="v.Hardmode", parent="PirateInvasion")
add("QueenSlime", T, "boss", "hm", 8.0, npcs=["Terraria/QueenSlimeBoss"], summons=["Terraria/QueenSlimeCrystal"], done="v.QueenSlime", available="v.Hardmode")
add("Twins", T, "boss", "hm", 9.0, "main", gate=True, npcs=["Terraria/Retinazer", "Terraria/Spazmatism"], summons=["Terraria/MechanicalEye"], done="v.Twins", available="v.Hardmode")
add("Destroyer", T, "boss", "hm", 10.0, "main", gate=True, npcs=["Terraria/TheDestroyer"], summons=["Terraria/MechanicalWorm"], done="v.Destroyer", available="v.Hardmode")
add("SkeletronPrime", T, "boss", "hm", 11.0, "main", gate=True, npcs=["Terraria/SkeletronPrime"], summons=["Terraria/MechanicalSkull"], done="v.SkeletronPrime", available="v.Hardmode")
add("Ogre", T, "miniboss", "hm", 11.01, npcs=["Terraria/DD2OgreT2"], done="v.OldOnesArmy2", available="v.MechAny", parent="OldOnesArmy")
add("SolarEclipse", T, "event", "hm", 11.5, npcs=["Terraria/Eyezor"], summons=["Terraria/SolarTablet"], repeatable=True, available="v.MechAny")
add("Plantera", T, "boss", "hm", 12.0, "main", gate=True, npcs=["Terraria/Plantera"], summons=["CalamityMod/Portabulb"], done="v.Plantera", available="v.MechAll")
add("Golem", T, "boss", "hm", 13.0, "main", gate=True, npcs=["Terraria/Golem"], summons=["Terraria/LihzahrdPowerCell"], done="v.Golem", available="v.Plantera")
add("PumpkinMoon", T, "event", "hm", 13.25, npcs=["Terraria/Scarecrow1"], summons=["Terraria/PumpkinMoonMedallion"], repeatable=True, available="v.Plantera")
add("MourningWood", T, "miniboss", "hm", 13.26, npcs=["Terraria/MourningWood"], done="v.MourningWood", available="v.Plantera", parent="PumpkinMoon")
add("Pumpking", T, "miniboss", "hm", 13.27, npcs=["Terraria/Pumpking"], done="v.Pumpking", available="v.Plantera", parent="PumpkinMoon")
add("FrostMoon", T, "event", "hm", 13.5, npcs=["Terraria/Flocko"], summons=["Terraria/NaughtyPresent"], repeatable=True, available="v.Plantera")
add("Everscream", T, "miniboss", "hm", 13.51, npcs=["Terraria/Everscream"], done="v.Everscream", available="v.Plantera", parent="FrostMoon")
add("SantaNK1", T, "miniboss", "hm", 13.52, npcs=["Terraria/SantaNK1"], done="v.SantaNK1", available="v.Plantera", parent="FrostMoon")
add("IceQueen", T, "miniboss", "hm", 13.53, npcs=["Terraria/IceQueen"], done="v.IceQueen", available="v.Plantera", parent="FrostMoon")
add("MartianMadness", T, "event", "hm", 13.75, npcs=["Terraria/MartianSaucerCore"], done="v.Martians", available="v.Golem")
add("MartianSaucer", T, "miniboss", "hm", 13.76, npcs=["Terraria/MartianSaucerCore"], done="v.Martians", available="v.Golem", parent="MartianMadness")
add("DukeFishron", T, "boss", "hm", 14.0, npcs=["Terraria/DukeFishron"], summons=["Terraria/TruffleWorm"], done="v.DukeFishron", available="v.Hardmode")
add("EmpressOfLight", T, "boss", "hm", 15.0, npcs=["Terraria/HallowBoss"], summons=["Terraria/EmpressButterfly"], done="v.EmpressOfLight", available="v.Plantera")
add("Betsy", T, "boss", "hm", 16.0, npcs=["Terraria/DD2Betsy"], summons=["Terraria/DD2ElderCrystal"], done="v.OldOnesArmy3", available="v.Golem", parent="OldOnesArmy")
add("LunaticCultist", T, "boss", "hm", 17.0, "main", gate=True, npcs=["Terraria/CultistBoss"], summons=["CalamityMod/EidolonTablet"], done="v.Cultist", available="v.Golem")
add("CelestialPillars", T, "event", "hm", 17.01, "main", gate=True, npcs=["Terraria/LunarTowerSolar"], done="v.Pillars", available="v.Cultist")
add("MoonLord", T, "boss", "hm", 18.0, "main", gate=True, npcs=["Terraria/MoonLordHead", "Terraria/MoonLordCore"], summons=["Terraria/CelestialSigil"], done="v.MoonLord", available="v.Pillars | v.Golem")

# ---------------------------------------------------------------- Calamity Mod 2.2.4
# With Calamity Fables loaded and its "replace Calamity" option on (the default), Fables' own
# Desert Scourge and Crabulon replace Calamity's versions.
fablesOff = "!mod:CalamityFables | !fables.ReplacesCalamity"
add("DesertScourge", C, "boss", "pre", 1.6, "main", npcs=["CalamityMod/DesertScourgeHead"], summons=["CalamityMod/DesertMedallion"], done="cal.DesertScourge", show=fablesOff)
add("GiantClam", C, "miniboss", "pre", 1.61, npcs=["CalamityMod/GiantClam"], summons=["InfernumMode/SparklingTunaCan"], done="cal.GiantClam")
add("AcidRain1", C, "event", "pre", 2.67, npcs=["CalamityMod/NuclearToad"], summons=["CalamityMod/CausticTear"], done="cal.AcidRain1")
add("Crabulon", C, "boss", "pre", 2.7, "main", npcs=["CalamityMod/Crabulon"], summons=["CalamityMod/DecapoditaSprout"], done="cal.Crabulon", show=fablesOff)
add("HiveMind", C, "boss", "pre", 3.98, "main", npcs=["CalamityMod/HiveMind"], summons=["CalamityMod/Teratoma"], done="cal.HiveMind", show="w.Corruption | w.Drunk")
add("Perforators", C, "boss", "pre", 3.99, "main", npcs=["CalamityMod/PerforatorHive"], summons=["CalamityMod/BloodyWormFood"], done="cal.Perforators", show="w.Crimson | w.Drunk")
add("SlimeGod", C, "boss", "pre", 6.7, "main", npcs=["CalamityMod/SlimeGodCore"], summons=["CalamityMod/OverloadedSludge"], done="cal.SlimeGod")
add("Cryogen", C, "boss", "hm", 8.5, "main", npcs=["CalamityMod/Cryogen"], summons=["CalamityMod/CryoKey"], done="cal.Cryogen", available="v.Hardmode")
add("AquaticScourge", C, "boss", "hm", 9.5, "main", npcs=["CalamityMod/AquaticScourgeHead"], summons=["CalamityMod/Seafood"], done="cal.AquaticScourge", available="v.Hardmode")
add("AcidRain2", C, "event", "hm", 9.51, npcs=["CalamityMod/FlakCrab"], summons=["CalamityMod/CausticTear"], done="cal.AcidRain2", available="cal.AquaticScourge")
add("CragmawMire", C, "miniboss", "hm", 9.52, npcs=["CalamityMod/CragmawMire"], done="cal.CragmawMire", available="cal.AquaticScourge", parent="AcidRain2")
add("BrimstoneElemental", C, "boss", "hm", 10.5, "main", npcs=["CalamityMod/BrimstoneElemental"], summons=["CalamityMod/CharredIdol"], done="cal.BrimstoneElemental", available="v.Hardmode")
add("CalamitasClone", C, "boss", "hm", 11.7, "main", npcs=["CalamityMod/CalamitasClone"], summons=["CalamityMod/EyeofDesolation"], done="cal.CalamitasClone", available="v.Hardmode")
add("GreatSandShark", C, "miniboss", "hm", 12.09, npcs=["CalamityMod/GreatSandShark"], summons=["CalamityMod/SandstormsCore"], done="cal.GreatSandShark", available="v.Plantera", show="!mod:InfernumMode | !inf.Active")
add("Leviathan", C, "boss", "hm", 12.8, "main", npcs=["CalamityMod/Anahita", "CalamityMod/Leviathan"], summons=["CalamityMod/NaiadsWarhorn"], done="cal.Leviathan", available="v.Plantera")
add("AstrumAureus", C, "boss", "hm", 12.81, "main", npcs=["CalamityMod/AstrumAureus"], summons=["CalamityMod/AstralChunk"], done="cal.AstrumAureus", available="v.Plantera")
add("PlaguebringerGoliath", C, "boss", "hm", 14.5, "main", npcs=["CalamityMod/PlaguebringerGoliath"], summons=["CalamityMod/Abombination"], done="cal.PlaguebringerGoliath", available="v.Golem")
add("Ravager", C, "boss", "hm", 16.5, "main", npcs=["CalamityMod/RavagerBody"], summons=["CalamityMod/DeathWhistle"], done="cal.Ravager", available="v.Plantera")
add("AstrumDeus", C, "boss", "hm", 17.5, "main", npcs=["CalamityMod/AstrumDeusHead"], summons=["CalamityMod/TitanHeart", "CalamityMod/Starcore", "CalamityMod/AstralBeaconItem"], done="cal.AstrumDeus", available="v.Hardmode")
add("ProfanedGuardians", C, "boss", "post", 18.5, "main", gate=True, npcs=["CalamityMod/ProfanedGuardianCommander"], summons=["CalamityMod/ProfanedShard"], done="cal.ProfanedGuardians", available="v.MoonLord")
add("Dragonfolly", C, "boss", "post", 18.6, "main", npcs=["CalamityMod/Dragonfolly"], summons=["CalamityMod/ExoticPheromones"], done="cal.Dragonfolly", available="v.MoonLord")
add("Providence", C, "boss", "post", 19.0, "main", npcs=["CalamityMod/Providence"], summons=["CalamityMod/ProfanedCore"], done="cal.Providence", available="cal.ProfanedGuardians")
add("CeaselessVoid", C, "boss", "post", 19.6, "main", npcs=["CalamityMod/CeaselessVoid"], summons=["CalamityMod/MarkofProvidence"], done="cal.CeaselessVoid", available="v.MoonLord")
add("StormWeaver", C, "boss", "post", 19.61, "main", npcs=["CalamityMod/StormWeaverHead"], summons=["CalamityMod/MarkofProvidence"], done="cal.StormWeaver", available="v.MoonLord")
add("Signus", C, "boss", "post", 19.62, "main", npcs=["CalamityMod/Signus"], summons=["CalamityMod/MarkofProvidence"], done="cal.Signus", available="v.MoonLord")
add("Polterghast", C, "boss", "post", 20.0, "main", npcs=["CalamityMod/Polterghast"], summons=["CalamityMod/NecroplasmicBeacon"], done="cal.Polterghast", available="v.MoonLord")
add("AcidRain3", C, "event", "post", 20.49, npcs=["CalamityMod/Mauler"], summons=["CalamityMod/CausticTear"], done="cal.OldDuke", available="cal.Polterghast")
add("Mauler", C, "miniboss", "post", 20.491, npcs=["CalamityMod/Mauler"], done="cal.Mauler", available="cal.Polterghast", parent="AcidRain3")
add("NuclearTerror", C, "miniboss", "post", 20.492, npcs=["CalamityMod/NuclearTerror"], done="cal.NuclearTerror", available="cal.Polterghast", parent="AcidRain3")
add("OldDuke", C, "boss", "post", 20.5, npcs=["CalamityMod/OldDuke"], summons=["CalamityMod/BloodwormItem"], done="cal.OldDuke", available="cal.Polterghast")
add("DevourerOfGods", C, "boss", "post", 21.0, "main", npcs=["CalamityMod/DevourerofGodsHead"], summons=["CalamityMod/CosmicWorm"], done="cal.DevourerOfGods", available="v.MoonLord")
add("Yharon", C, "boss", "post", 22.0, "main", gate=True, npcs=["CalamityMod/Yharon"], summons=["CalamityMod/YharonEgg"], done="cal.Yharon", available="cal.Dragonfolly")
add("ExoMechs", C, "boss", "end", 22.99, "main", gate=True, npcs=["CalamityMod/Draedon", "CalamityMod/Apollo", "CalamityMod/AresBody", "CalamityMod/ThanatosHead"], summons=["CalamityMod/CodebreakerBase", "CalamityMod/AuricQuantumCoolingCell"], done="cal.ExoMechs", available="cal.Yharon")
add("SupremeCalamitas", C, "boss", "end", 23.0, "main", gate=True, npcs=["CalamityMod/SupremeCalamitas"], summons=["CalamityMod/AshesofCalamity", "CalamityMod/CeremonialUrn", "CalamityMod/AltarOfTheAccursedItem"], done="cal.SupremeCalamitas", available="cal.Yharon")
add("PrimordialWyrm", C, "boss", "end", 23.5, npcs=["CalamityMod/PrimordialWyrmHead"], summons=["InfernumMode/EvokingSearune"], done="cal.PrimordialWyrm")
add("BossRush", C, "event", "end", 25.99, npcs=["CalamityMod/SupremeCalamitas"], summons=["CalamityMod/Terminus"], done="cal.BossRush")

# ---------------------------------------------------------------- Catalyst 1.1.8
# The Astral Communicator stops working after Moon Lord unless Astrageldon was already defeated.
add("Astrageldon", "CatalystMod", "boss", "hm", 17.99, npcs=["CatalystMod/Astrageldon"], summons=["CatalystMod/AstralCommunicator"], done="catalyst.Astrageldon", available="!v.MoonLord | catalyst.Astrageldon")

# ---------------------------------------------------------------- Infernum 2.0.1.35 (shown while Infernum is on in this world)
add("Dreadnautilus", "InfernumMode", "boss", "hm", 7.9, npcs=["Terraria/BloodNautilus"], summons=["InfernumMode/RedBait"], done="cal.Dreadnautilus", available="inf.Active & v.Hardmode", show="inf.Active")
add("BereftVassal", "InfernumMode", "boss", "hm", 17.75, npcs=["InfernumMode/BereftVassal"], summons=["CalamityMod/SandstormsCore"], done="inf.BereftVassal", available="inf.Active & v.Cultist", show="inf.Active")

# ---------------------------------------------------------------- Hunt of the Old God 1.2.3
add("Goozma", "CalamityHunt", "boss", "end", 23.6, npcs=["CalamityHunt/Goozma"], summons=["CalamityMod/OverloadedSludge", "CalamityHunt/PluripotentSpawnEgg"], done="hunt.Goozma", available="v.Hardmode")

# ---------------------------------------------------------------- Wrath of the Gods 1.2.31 (Solyn's story + bosses)
W = "NoxusBoss"
add("SolynMeet", W, "quest", "pre", 5.1, npcs=["NoxusBoss/Solyn"], done="wotg.Intro", available="v.Skeletron | v.Hardmode")
add("SolynCamp", W, "quest", "pre", 5.2, npcs=["NoxusBoss/Solyn"], summons=["NoxusBoss/StrangeFlagpole"], done="wotg.Camp | wotg.Stargaze", available="wotg.Intro")
add("SolynStargaze", W, "quest", "pre", 5.3, npcs=["NoxusBoss/Solyn"], done="wotg.Stargaze", available="wotg.Intro")
add("PermafrostKeep", W, "quest", "hm", 8.6, npcs=["NoxusBoss/Solyn"], summons=["NoxusBoss/DormantKey"], done="wotg.Keep", available="wotg.Stargaze & cal.Cryogen")
add("GenesisReveal", W, "quest", "post", 18.1, npcs=["NoxusBoss/Solyn"], done="wotg.GenesisIntro", available="wotg.Keep & v.MoonLord")
add("Antiseed", W, "quest", "post", 19.65, npcs=["NoxusBoss/Solyn"], done="wotg.Antiseed", available="wotg.GenesisIntro & cal.Providence & cal.CeaselessVoid")
add("Mars", W, "boss", "end", 23.25, npcs=["NoxusBoss/MarsBody"], done="wotg.Mars", available="wotg.Antiseed & cal.ExoMechs")
add("AvatarOfEmptiness", W, "boss", "end", 27.0, npcs=["NoxusBoss/AvatarOfEmptiness"], done="wotg.Avatar", available="wotg.Mars & wotg.GenesisDone")
add("NamelessDeity", W, "boss", "end", 28.0, npcs=["NoxusBoss/NamelessDeityBoss"], summons=["CalamityMod/Terminus"], done="wotg.NamelessDeity", available="wotg.Avatar")

# ---------------------------------------------------------------- Calamity Fables 0.1.24
F = "CalamityFables"
fablesOn = "!mod:CalamityMod | fables.ReplacesCalamity"
# Both are drawn by custom code (no usable NPC sprite), so their relics are shown instead.
add("SirNautilus", F, "boss", "pre", 1.9, npcs=["CalamityFables/SirNautilus"], done="fables.SirNautilus", icon="CalamityFables/SirNautilusRelicItem")
add("FablesDesertScourge", F, "boss", "pre", 2.5, "main", npcs=["CalamityFables/DesertScourge"], summons=["CalamityFables/StormlionLarvaItem", "CalamityFables/DeadStormlionLarvaItem", "CalamityFables/BucketOfLarvae"], done="fables.DesertScourge", show=fablesOn, replaces=["DesertScourge"])
add("ScourgeVsScarab", F, "boss", "pre", 2.999, npcs=["CalamityFables/ScourgeVsScarab"], summons=["CalamityFables/StormlionLarvaItem", "SpiritReforged/ScarabAltarItem"], done="fables.ScourgeVsScarab", show="mod:SpiritReforged", icon="CalamityFables/ScourgeVsScarabRelicItem")
add("FablesCrabulon", F, "boss", "pre", 3.6, "main", npcs=["CalamityFables/Crabulon"], summons=["CalamityFables/Hookstaff"], done="fables.Crabulon", show=fablesOn, replaces=["Crabulon"])

# ---------------------------------------------------------------- Spirit Reforged 0.2.2.8
add("Scarabeus", "SpiritReforged", "boss", "pre", 2.2, npcs=["SpiritReforged/Scarabeus"], summons=["SpiritReforged/ScarabAltarItem"], done="spirit.Scarabeus")


def main():
    out = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..", "Data", "progression.json")
    ids = [e["id"] for e in E]
    assert len(ids) == len(set(ids)), "duplicate ids"
    with open(out, "w", encoding="utf-8", newline="\n") as f:
        f.write("{\n  \"entries\": [\n")
        f.write(",\n".join("    " + json.dumps(e, ensure_ascii=False) for e in sorted(E, key=lambda e: e["order"])))
        f.write("\n  ]\n}\n")
    print(len(E), "entries ->", os.path.normpath(out))


if __name__ == "__main__":
    main()
