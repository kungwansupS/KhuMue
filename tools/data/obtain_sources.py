"""
Extract "where does this item come from" facts the game has no database for, straight from the game code:
fishing catches (with the conditions of the branch that yields them), Angler quest rewards and chest loot.

    python tools/data/obtain_sources.py <decompiled tModLoader src> <decompiled CalamityMod src> <dump folder>

Writes Data/obtain.json: {"Mod/Item": [{"how": "fishing", "where": [["jungle", "cavern"]]}, {"how": "angler"},
{"how": "chest", "chest": "Ivy", "with": "CalamityMod"}]}. "with"/"without" name a mod the entry depends on.
The decompiled sources are only read; nothing of them is copied into the mod.
Checked against tModLoader 2026.7 (Terraria 1.4.4.9) and Calamity 2.2.4.
"""
import json
import os
import re
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
OUT = os.path.join(HERE, '..', '..', 'Data', 'obtain.json')


# ---------------------------------------------------------------- C# helpers

def method_body(text, signature):
    """Body of the first method whose declaration contains `signature` (text between its outer braces)."""
    start = text.index(signature)
    open_at = text.index('{', start)
    depth = 0
    for i in range(open_at, len(text)):
        if text[i] == '{':
            depth += 1
        elif text[i] == '}':
            depth -= 1
            if depth == 0:
                return text[open_at + 1:i]
    raise ValueError('unbalanced method ' + signature)


def balanced(text, i, open_ch='(', close_ch=')'):
    """Index just after the bracket group starting at text[i] == open_ch."""
    depth = 0
    for j in range(i, len(text)):
        if text[j] == open_ch:
            depth += 1
        elif text[j] == close_ch:
            depth -= 1
            if depth == 0:
                return j + 1
    raise ValueError('unbalanced group')


def walk(body, assign_rx):
    """
    Yield (conditions, statement) for every statement matching assign_rx, where conditions are the positive
    if / else-if conditions on the path to it (negations from skipped branches are not tracked).
    """
    results = []

    def block(text, conds):
        i = 0
        n = len(text)
        while i < n:
            m = re.compile(r'\s*(else\s+if|if|else)\b').match(text, i)
            if m and (m.group(1) != 'else' or not text[m.end():].lstrip().startswith('if')):
                kind = m.group(1)
                j = m.end()
                cond = None
                if kind != 'else':
                    p = text.index('(', j)
                    end = balanced(text, p)
                    cond = text[p + 1:end - 1]
                    j = end
                k = j
                while k < n and text[k].isspace():
                    k += 1
                if k < n and text[k] == '{':
                    end = balanced(text, k, '{', '}')
                    inner = text[k + 1:end - 1]
                    i = end
                else:
                    end = text.index(';', k) + 1
                    inner = text[k:end]
                    i = end
                block(inner, conds + ([cond] if cond else []))
                continue
            # A plain statement: up to the next ';' at this nesting level (skip nested braces, e.g. array literals).
            k = i
            depth = 0
            while k < n:
                c = text[k]
                if c in '({[':
                    depth += 1
                elif c in ')}]':
                    depth -= 1
                elif c == ';' and depth <= 0:
                    break
                elif c == '{' and depth == 0:
                    break
                k += 1
            stmt = text[i:k + 1]
            if assign_rx.search(stmt):
                results.append((conds, stmt.strip()))
            i = k + 1

    block(body, [])
    return results


def ids_in(expr):
    """(item reference, extra tag) pairs of an assigned expression: literals, ternaries, lists, ranges, ModContent."""
    out = []
    t = re.search(r'\((?:Main\.)?hardMode\s*\?\s*(.+?)\s*:\s*(.+?)\)\s*;', expr)
    if t:
        out += [(r, 'hardmode') for r in refs(t.group(1))]
        out += [(r, 'prehardmode') for r in refs(t.group(2))]
        return out
    r = re.search(r'Main\.rand\.Next\((\d+),\s*(\d+)\)', expr)
    if r:
        return [(str(x), None) for x in range(int(r.group(1)), int(r.group(2)))]
    return [(x, None) for x in refs(expr)]


def refs(expr):
    found = re.findall(r'ModContent\.ItemType<(\w+)>\(\)', expr)
    found += re.findall(r'(?<![\w.])(\d{2,5})(?![\w.])', re.sub(r'new short\[\d+\]', '', expr))
    return found


# ---------------------------------------------------------------- condition tags

VANILLA_FLAGS = {'flag': 'corruption', 'flag2': 'crimson', 'flag3': 'jungle', 'flag4': 'snow', 'flag5': 'dungeon',
                 'flag6': 'desert'}
CALAMITY_FLAGS = {'flag': 'sky', 'flag2': 'surface', 'flag3': 'underground', 'flag4': 'cavern', 'flag6': 'sulphur'}

COMMON = [
    (r'inLava\b', 'lava'), (r'inHoney\b', 'honey'), (r'\.crate\b', 'crate'), (r'bloodMoon', 'bloodmoon'),
    (r'ZoneBeach', 'ocean'), (r'ZoneHallow', 'hallow'), (r'ZoneSnow', 'snow'), (r'ZoneDesert', 'desert'),
    (r'ZoneJungle', 'jungle'), (r'ZoneCorrupt', 'corruption'), (r'ZoneCrimson', 'crimson'),
    (r'ZoneDungeon', 'dungeon'), (r'ZoneGlowshroom', 'mushroom'), (r'ZoneUnderworldHeight', 'underworld'),
    (r'heightLevel == 0', 'sky'), (r'heightLevel == 1', 'surface'), (r'heightLevel == 2', 'underground'),
    (r'heightLevel == 3', 'cavern'), (r'heightLevel (?:== 4|> 3|>= 4)', 'underworld'),
    (r'heightLevel (?:>= 2|> 1)', 'underground'), (r'questFish ==', 'quest'),
    (r'Main\.hardMode', 'hardmode'), (r'ZoneCalamity', 'brimstone'), (r'ZoneAbyss', 'abyss'),
    (r'ZoneSulphur', 'sulphur'), (r'ZoneSunkenSea', 'sunkensea'), (r'ZoneAstral', 'astral'),
]


# Boss flags in conditions -> progression entry ids (Data/progression.json), shown as "after <boss>".
DOWNED = {
    'NPC.downedBoss1': 'EyeOfCthulhu', 'NPC.downedBoss3': 'Skeletron', 'NPC.downedQueenBee': 'QueenBee',
    'NPC.downedPlantBoss': 'Plantera', 'NPC.downedGolemBoss': 'Golem', 'NPC.downedMoonlord': 'MoonLord',
    'NPC.downedFishron': 'DukeFishron',
}


def tags(conds, flags):
    out = []
    for cond in conds:
        for m in re.finditer(r'(?<!!)DownedBossSystem\.downed(\w+)', cond):
            tag = 'after:' + m.group(1)
            if tag not in out:
                out.append(tag)
        for flag, entry in DOWNED.items():
            if re.search(r'(?<!!)' + re.escape(flag) + r'\b', cond) and 'after:' + entry not in out:
                out.append('after:' + entry)
        # "!x" inside a positive condition is a negation of x: drop those parts.
        positive = re.sub(r'!\s*[\w.\[\]()]+', '', cond)
        for rx, tag in COMMON:
            if re.search(rx, positive) and tag not in out:
                out.append(tag)
        for var, tag in flags.items():
            if re.search(r'(?<![\w.!])' + var + r'(?![\w])', positive) and tag not in out:
                out.append(tag)
    return out


# ---------------------------------------------------------------- sources

def vanilla_fishing(src, keys):
    text = open(os.path.join(src, 'Terraria', 'Projectile.cs'), encoding='utf-8').read()
    body = method_body(text, 'void FishingCheck_RollItemDrop(ref FishingAttempt fisher)')
    found = {}
    for conds, stmt in walk(body, re.compile(r'fisher\.rolledItemDrop\s*=')):
        for ref, extra in ids_in(stmt):
            key = keys.get(int(ref)) if ref.isdigit() else None
            if key:
                where = tags(conds, VANILLA_FLAGS) + ([extra] if extra else [])
                found.setdefault(key, []).append(where)
    return found


def calamity_fishing(src):
    path = os.path.join(src, 'CalamityMod.CalPlayer', 'CalamityPlayer.cs')
    text = open(path, encoding='utf-8').read()
    body = method_body(text, 'public override void CatchFish(FishingAttempt attempt, ref int itemDrop')
    found = {}
    rx = re.compile(r'\bitemDrop\s*=|AsSpan\(\w+\)\[\d+\]\s*=|\.AddWithCondition\(|\.Add\(ModContent\.ItemType')
    for conds, stmt in walk(body, rx):
        extra_cond = []
        m = re.search(r'AddWithCondition\((ModContent\.ItemType<\w+>\(\)),\s*(.+)\)\s*;', stmt)
        if m:
            extra_cond = [m.group(2)]
            stmt = m.group(1) + ';'
        elif stmt.lstrip().startswith('itemDrop') and re.search(r'\w+\[Main\.rand\.Next', stmt):
            continue  # picks from a list built above; the list entries are recorded where they are added
        for ref, extra in ids_in(stmt):
            if ref.isdigit():
                continue  # vanilla fallbacks inside Calamity's method
            where = tags(conds + extra_cond, CALAMITY_FLAGS) + ([extra] if extra else [])
            found.setdefault('CalamityMod/' + ref, []).append(where)
    return found


def angler_rewards(src, keys):
    text = open(os.path.join(src, 'Terraria', 'Player.cs'), encoding='utf-8').read()
    found = set()
    for sig in ('void GetAnglerReward_MainReward(', 'void GetAnglerReward_Decoration(', 'void GetAnglerReward_Bait('):
        body = method_body(text, sig)
        groups = [body[m.end():balanced(body, m.end() - 1) - 1] for m in re.finditer(r'SetDefaults\(', body)]
        groups += re.findall(r'new (?:List<int>|int\[\d*\]|short\[\d*\])\s*\{([^}]*)\}', body)
        for group in groups:
            # Drop switch-arm labels ("3 => 2443") and random ranges, which are not item ids.
            group = re.sub(r'\b\d+\s*=>', '', re.sub(r'Main\.rand\.Next\([^)]*\)', '', group))
            for x in re.findall(r'(?<![\w.])(\d{2,5})(?![\w.])', group):
                if int(x) in keys:
                    found.add(keys[int(x)])
    return found


# ---------------------------------------------------------------- chests
#
# World generation has no loot table, so the chest kinds below were labelled by reading the generation code
# (Terraria 1.4.4.9 WorldGen, Calamity world gen). Every id must still appear at the place given, otherwise the
# script stops: a game update that moves loot cannot slip through. Only notable loot is listed (weapons,
# accessories, tools, keys, unique items); potions, bars, ammo, torches and coins are left out.

BURIED = ('method', 'public static bool AddBuriedChest(int i, int j, int contain = 0')
CAL_POST = ('method', 'public override void PostWorldGen()')
# The Hardmode biome chests of the Dungeon: one switch, case 0 = Jungle ... case 4 = Desert (read by hand).
BIOME_CHESTS = ('after', 'style2 = 23;', 40)

# kind -> list of (source, file, place, refs, extra fields)
CHESTS = [
    ('Surface', 'tml', 'Terraria/WorldGen.cs', BURIED, [280, 281, 284, 285, 953, 946, 3068, 3069, 3084, 3093, 4345], {}),
    ('Surface', 'tml', 'Terraria/WorldGen.cs', BURIED, [4341], {'without': 'CalamityMod'}),  # Calamity swaps it for Kylie
    ('Underground', 'tml', 'Terraria/WorldGen.cs', BURIED, [49, 50, 53, 54, 5011, 975, 930, 997], {}),
    ('LavaLayer', 'tml', 'Terraria/WorldGen.cs', BURIED, [906], {}),
    ('Frozen', 'tml', 'Terraria/WorldGen.cs', BURIED, [670, 724, 950, 1319, 987, 1579, 669, 3199, 997], {}),
    ('Sandstone', 'tml', 'Terraria/WorldGen.cs', BURIED, [4056, 4055, 4262, 4263, 4346, 4066, 4423], {}),
    ('SandstoneDeep', 'tml', 'Terraria/WorldGen.cs', BURIED, [4061, 4062, 4276], {}),
    ('Ivy', 'tml', 'Terraria/WorldGen.cs', ('method', 'public static int GetNextJungleChestItem()'),
     [211, 212, 213, 964, 753, 2292, 3017], {}),
    ('Ivy', 'tml', 'Terraria/WorldGen.cs', BURIED, [3360, 3361, 4426], {}),
    ('Water', 'tml', 'Terraria/WorldGen.cs', ('after', 'AddGenerationPass("Water Chests"', 12), [863, 186, 277, 187, 4404], {}),
    ('Water', 'tml', 'Terraria/WorldGen.cs', BURIED, [4425, 4460], {}),
    ('Skyware', 'tml', 'Terraria/WorldGen.cs', ('after', 'int num13 = GenVars.skyIslandHouseCount;', 22), [159, 65, 158, 2219], {}),
    ('Skyware', 'tml', 'Terraria/WorldGen.cs', BURIED, [4978], {'without': 'CalamityMod'}),  # ILChanges.DisableFledglingWingsSecondary
    ('LivingWood', 'tml', 'Terraria/WorldGen.cs', BURIED, [832, 933, 4429, 4427], {}),
    ('LivingWood', 'tml', 'Terraria/WorldGen.cs', ('after', 'int contain = 832;', 4), [4281], {}),
    ('Pyramid', 'tml', 'Terraria/WorldGen.cs', BURIED, [848, 866, 857, 934], {}),
    ('Web', 'tml', 'Terraria/WorldGen.cs', ('after', 'AddBuriedChest(item.X, item.Y, 939', 1), [939], {}),
    ('Lihzahrd', 'tml', 'Terraria/WorldGen.cs', ('after', 'int contain = 1293;', 1), [1293], {}),
    ('Shadow', 'tml', 'Terraria/WorldGen.cs', ('after', 'List<int> list = new List<int> { 274, 220, 112, 218, 3019 };', 1),
     [274, 220, 112, 218, 3019], {}),
    ('Shadow', 'tml', 'Terraria/WorldGen.cs', BURIED, [5010, 4443, 4737, 4551], {}),
    ('Dungeon', 'tml', 'Terraria/WorldGen.cs', ('after', 'int style3 = 2;', 45), [155, 156, 157, 163, 113, 3317, 164], {}),
    ('Dungeon', 'tml', 'Terraria/WorldGen.cs', BURIED, [329], {}),
    ('DungeonOpen', 'tml', 'Terraria/WorldGen.cs', ('after', 'int style3 = 2;', 45), [327], {}),
    ('BiomeJungle', 'tml', 'Terraria/WorldGen.cs', BIOME_CHESTS, [1156], {}),
    ('BiomeCorruption', 'tml', 'Terraria/WorldGen.cs', BIOME_CHESTS, [1571], {}),
    ('BiomeCrimson', 'tml', 'Terraria/WorldGen.cs', BIOME_CHESTS, [1569], {}),
    ('BiomeHallow', 'tml', 'Terraria/WorldGen.cs', BIOME_CHESTS, [1260], {}),
    ('BiomeFrozen', 'tml', 'Terraria/WorldGen.cs', BIOME_CHESTS, [1572], {}),
    ('BiomeDesert', 'tml', 'Terraria/WorldGen.cs', BIOME_CHESTS, [4607], {}),
    ('Mushroom', 'tml', 'Terraria/WorldGen.cs', BURIED, [4450, 4779, 4780, 4781], {}),

    # Calamity: changes to vanilla chests (WorldgenManagementSystem.PostWorldGen)
    ('Surface', 'cal', 'CalamityMod.Systems/WorldgenManagementSystem.cs', CAL_POST, ['Kylie'], {}),
    ('Cavern', 'cal', 'CalamityMod.Systems/WorldgenManagementSystem.cs', CAL_POST, ['EnchantedKnifeStaff'], {}),
    ('Mushroom', 'cal', 'CalamityMod.Systems/WorldgenManagementSystem.cs', CAL_POST, [4764], {'with': 'CalamityMod'}),
    ('Sandstone', 'cal', 'CalamityMod.Systems/WorldgenManagementSystem.cs', CAL_POST, ['DesertMedallion', 'TheComb'], {}),
    # Calamity structures
    ('CalAbyss', 'cal', 'CalamityMod.World/Abyss.cs', ('after', 'itemChoice = itemChoice switch', 13),
     ['TorrentialTear', 'IronBoots', 'DepthCharm', 'Archerfish', 'AnechoicPlating', 'BallOFugu', 'StrangeOrb',
      'HerringStaff', 'BlackAnurian', 'Lionfish'], {}),
    ('CalAbyssShrine', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillAbyssShrineChest('),
     ['Terminus', 'AbyssShocker', 'DepthCrusher', 'InkBomb'], {}),
    ('CalAstral', 'cal', 'CalamityMod.World/AstralChestGeneration.cs', ('method', 'public static void PlaceAstralChest()'),
     ['HeavenfallenStardisk'], {}),
    ('CalBrimstone', 'cal', 'CalamityMod.World/BrimstoneCrag.cs', ('method', 'public static void FillBrimstoneChests('),
     ['SlagfireDouser', 'SlurperPole'], {}),
    ('CalBrimstone', 'cal', 'CalamityMod.World/BrimstoneCrag.cs', ('method', 'public static void FillBrimstoneChests('),
     [907], {'with': 'CalamityMod'}),
    ('CalSulphur', 'cal', 'CalamityMod.World/SulphurousSea.cs', ('file',),
     ['EffigyOfDecay', 'BrokenWaterFilter', 'RustyBeaconPrototype', 'ScionsCurio'], {}),
    ('CalShrineCorruption', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillCorruptionShrineChest('),
     ['CorruptionEffigy', 1534], {'with': 'CalamityMod'}),
    ('CalShrineCrimson', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillCrimsonShrineChest('),
     ['CrimsonEffigy', 1535], {'with': 'CalamityMod'}),
    ('CalShrineDesert', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillDesertShrineChest('),
     ['LuxorsGift', 4714], {'with': 'CalamityMod'}),
    ('CalShrineGranite', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillGraniteShrineChest('),
     ['UnstableGraniteCore'], {}),
    ('CalShrineIce', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillIceShrineChest('),
     ['FrozenCube', 1537], {'with': 'CalamityMod'}),
    ('CalShrineMarble', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillMarbleShrineChest('),
     ['GladiatorsLocket'], {}),
    ('CalShrineMushroom', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillMushroomShrineChest('),
     ['FungalSymbiote', 2673], {'with': 'CalamityMod'}),
    ('CalShrineSurface', 'cal', 'CalamityMod.World/UndergroundShrines.cs', ('method', 'public static void FillSurfaceShrineChest('),
     ['TrinketofChi'], {}),
    ('CalLabHell', 'cal', 'CalamityMod.World/DraedonStructures.cs', ('method', 'public static void FillHellLaboratoryChest('),
     ['WhiteSeekingMechanism', 'DraedonsLogHell', 'Murasama', 'EncryptedSchematicHell'], {}),
    ('CalLabSunkenSea', 'cal', 'CalamityMod.World/DraedonStructures.cs', ('method', 'public static void FillSunkenSeaLaboratoryChest('),
     ['YellowSeekingMechanism', 'DraedonsLogSunkenSea', 'EncryptedSchematicSunkenSea'], {}),
    ('CalLabIce', 'cal', 'CalamityMod.World/DraedonStructures.cs', ('method', 'public static void FillIceLaboratoryChest('),
     ['LabSeekingMechanism', 'DraedonsLogSnowBiome', 'EncryptedSchematicIce'], {}),
    ('CalLabPlague', 'cal', 'CalamityMod.World/DraedonStructures.cs', ('method', 'public static void FillPlagueLaboratoryChest('),
     ['RedSeekingMechanism', 'DraedonsLogJungle', 'EncryptedSchematicJungle'], {}),
    ('CalLabPlanetoid', 'cal', 'CalamityMod.World/DraedonStructures.cs', ('method', 'public static void FillPlanetoidLaboratoryChest('),
     ['GreenSeekingMechanism', 'DraedonsLogPlanetoid', 'EncryptedSchematicPlanetoid', 'PlasmaDriveCore'], {}),
    ('CalLabWorkshop', 'cal', 'CalamityMod.World/DraedonStructures.cs', ('method', 'public static void FillWorkshopChest('),
     ['LabSeekingMechanism', 'CyanSeekingMechanism', 'GreenSeekingMechanism', 'WhiteSeekingMechanism',
      'RedSeekingMechanism', 'YellowSeekingMechanism', 'SuspiciousScrap', 'DubiousPlating', 'MysteriousCircuitry'], {}),
    ('CalVernal', 'cal', 'CalamityMod.World/VernalPass.cs', ('method', 'public static void FillVernalPassChests('),
     ['BelladonnaSpiritStaff', 'FellerofEvergreens'], {}),
    ('CalVernal', 'cal', 'CalamityMod.World/VernalPass.cs', ('method', 'public static void FillVernalPassChests('),
     [211, 212, 213], {'with': 'CalamityMod'}),
    ('CalArchive', 'cal', 'CalamityMod.World/DungeonArchive.cs', ('method', 'public static void FillArchiveChests('),
     [329, 3095, 531], {'with': 'CalamityMod'}),
    ('CalHive', 'cal', 'CalamityMod.World/GiantHive.cs', ('after', 'FocusLootHoney = new int[4]', 1),
     [223, 887, 3017, 4426], {'with': 'CalamityMod'}),
    ('CalMechanic', 'cal', 'CalamityMod.World/MechanicShed.cs', ('method', 'public static void FillMechanicChest('),
     [1923, 3624, 2214, 2215, 2216, 2217], {'with': 'CalamityMod'}),
    ('CalShimmer', 'cal', 'CalamityMod.World/ShimmerShrine.cs', ('file',), [29, 109], {'with': 'CalamityMod'}),
    ('CalPlanetoid', 'cal', 'CalamityMod.World.Planets/MudPlanet.cs', ('after', 'FocusLoot = new int[3]', 1),
     [187, 268, 277], {'with': 'CalamityMod'}),
    ('CalEvilIsland', 'cal', 'CalamityMod.World/WorldEvilIsland.cs', ('after', 'int contain = ((!genCorruptHouse) ? 1569 : 1571);', 1),
     [1569, 1571], {'with': 'CalamityMod'}),
]


def region(text, place):
    """The part of a source file a chest entry must be found in."""
    if place[0] == 'file':
        return text
    if place[0] == 'method':
        return method_body(text, place[1])
    start = text.index(place[1])
    end = start
    for _ in range(place[2] + 1):
        nxt = text.find('\n', end + 1)
        end = len(text) if nxt < 0 else nxt
    return text[start:end]


def chests(tml, calamity, keys, known):
    found = {}
    cache = {}
    for kind, src, rel, place, refs, extra in CHESTS:
        path = os.path.join(tml if src == 'tml' else calamity, *rel.split('/'))
        if path not in cache:
            cache[path] = open(path, encoding='utf-8').read()
        try:
            part = region(cache[path], place)
        except ValueError as e:
            raise SystemExit(f'chest {kind}: {place} not found in {rel} ({e})')
        for ref in refs:
            if isinstance(ref, int):
                if not re.search(r'(?<![\w.])' + str(ref) + r'(?![\w.])', part):
                    raise SystemExit(f'chest {kind}: item id {ref} not found in {rel} at {place}')
                key = keys.get(ref)
            else:
                if 'ItemType<' + ref + '>' not in part:
                    raise SystemExit(f'chest {kind}: ItemType<{ref}> not found in {rel} at {place}')
                key = 'CalamityMod/' + ref
            if key not in known:
                raise SystemExit(f'chest {kind}: {ref} is not a known item')
            entry = {'how': 'chest', 'chest': kind, **extra}
            if entry not in found.setdefault(key, []):
                found[key].append(entry)
    return found


def merge_where(lists):
    """
    Distinct condition sets of the branches that yield the item. When the same catch sits in several biome
    branches (e.g. Scaly Truffle under every evil/Hallow branch), keep only the conditions they all share.
    """
    cleaned = []
    for where in lists:
        w = [t for t in where if t != 'quest']
        if w not in cleaned:
            cleaned.append(w)
    if len(cleaned) > 1:
        shared = [t for t in cleaned[0] if all(t in w for w in cleaned[1:])]
        if shared:
            return [shared]
    cleaned.sort(key=len, reverse=True)
    return cleaned[:4]


def main():
    tml, calamity, dump = sys.argv[1], sys.argv[2], sys.argv[3]
    items = json.load(open(os.path.join(dump, 'items.json'), encoding='utf-8'))
    items = items if isinstance(items, list) else list(items.values())
    keys = {x['type']: x['key'] for x in items if x['key'].startswith('Terraria/')}
    known = {x['key'] for x in items}

    result = {}
    fishing = vanilla_fishing(tml, keys)
    for key, wheres in calamity_fishing(calamity).items():
        fishing.setdefault(key, []).extend(wheres)
    for key, wheres in fishing.items():
        if key not in known:
            print('unknown item (skipped):', key)
            continue
        result.setdefault(key, []).append({'how': 'fishing', 'where': merge_where(wheres)})
    for key in sorted(angler_rewards(tml, keys)):
        result.setdefault(key, []).append({'how': 'angler'})
    for key, entries in chests(tml, calamity, keys, known).items():
        result.setdefault(key, []).extend(entries)

    result = dict(sorted(result.items()))
    with open(OUT, 'w', encoding='utf-8') as f:
        json.dump(result, f, ensure_ascii=False, indent=1)
        f.write('\n')
    print(f'{len(result)} items -> {os.path.normpath(OUT)}')
    for how in ('fishing', 'angler', 'chest'):
        print(how + ':', sum(1 for v in result.values() if any(e['how'] == how for e in v)))


if __name__ == '__main__':
    main()
