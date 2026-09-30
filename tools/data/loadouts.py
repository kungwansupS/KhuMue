"""
Builds Data/loadouts-calamity.json and Data/loadouts-vanilla.json from wiki snapshots in
tools/data/sources/ and a live data dump (item names -> internal keys).

Sources (snapshots saved 2026-09-24):
  * Calamity Mod Wiki "Guide:Class setups/{Pre-Hardmode,Hardmode,Post-Moon Lord}/data"
    (revisions 300093, 301361, 301088) - CC BY-NC-SA 3.0
  * Terraria Wiki "Guide:Class setups" revision 957024 (2026-01-27), the last revision before
    Terraria 1.4.5 content was added; tModLoader runs Terraria 1.4.4.9 - CC BY-NC-SA 4.0

  python tools/data/loadouts.py <calamity-dump-folder> <vanilla-dump-folder>

  (dumps from tools/RunDataDump.ps1 -Profile calamity / -Profile vanilla, so each name resolves
  against exactly the items that exist in that setup)

Every item name is resolved to the item that exists in the installed mod versions. Names that
cannot be resolved are reported and left out (they never reach the guide as broken entries).
Notes are replaced by note ids whose text lives in the localization files.
"""
import json
import os
import re
import sys
from collections import OrderedDict, defaultdict

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.normpath(os.path.join(HERE, "..", ".."))
SRC = os.path.join(HERE, "sources")

report = []


# ---------------------------------------------------------------- wikitext helpers

def split_args(s):
    args, depth, cur, i = [], 0, "", 0
    while i < len(s):
        two = s[i:i + 2]
        if two in ("{{", "[["):
            depth += 1
            cur += two
            i += 2
            continue
        if two in ("}}", "]]"):
            depth -= 1
            cur += two
            i += 2
            continue
        if s[i] == "|" and depth == 0:
            args.append(cur)
            cur = ""
            i += 1
            continue
        cur += s[i]
        i += 1
    args.append(cur)
    return args


def templates(text, name):
    out = []
    tag = "{{" + name + "|"
    i = 0
    while True:
        j = text.find(tag, i)
        if j < 0:
            break
        depth, k = 0, j
        while k < len(text):
            if text.startswith("{{", k):
                depth += 1
                k += 2
                continue
            if text.startswith("}}", k):
                depth -= 1
                k += 2
                if depth == 0:
                    break
                continue
            k += 1
        out.append((j, text[j + 2:k - 2]))
        i = k
    return out


def item_names(cell):
    names = []
    for _, t in templates(cell, "item"):
        a = [x for x in split_args(t)[1:] if not re.match(r"^\s*\w+\s*=", x)]
        if a:
            names.append(a[0].strip())
    return names


def plain(s):
    s = s.replace("'''", "").replace("''", "")
    while True:
        ts = templates(s, "item")
        if not ts:
            break
        _, t = ts[0]
        a = [x for x in split_args(t)[1:] if not re.match(r"^\s*\w+\s*=", x)]
        s = s.replace("{{" + t + "}}", a[0].strip() if a else "", 1)
    s = re.sub(r"\{\{[^{}]*\}\}", "", s)
    s = re.sub(r"\[\[(?:[^\]|]*\|)?([^\]]*)\]\]", r"\1", s)
    s = re.sub(r"<[^>]+>", "", s)
    return re.sub(r"\s+", " ", s).strip()


# ---------------------------------------------------------------- name -> key

class Names:
    def __init__(self, dump):
        with open(os.path.join(dump, "items.json"), encoding="utf-8") as f:
            self.items = json.load(f)
        self.by_name = defaultdict(list)
        for it in self.items:
            self.by_name[self.norm(it["name"])].append(it)

    @staticmethod
    def norm(s):
        return re.sub(r"\s+", " ", s.replace("’", "'")).strip().lower()

    # Wiki page names that differ from the in-game item name, or that name a group.
    SPECIAL = {
        "apprentice armor": ["Terraria/ApprenticeHat", "Terraria/ApprenticeRobe", "Terraria/ApprenticeTrousers"],
        "monk armor": ["Terraria/MonkBrows", "Terraria/MonkShirt", "Terraria/MonkPants"],
        "squire armor": ["Terraria/SquireGreatHelm", "Terraria/SquirePlating", "Terraria/SquireGreaves"],
        "huntress armor": ["Terraria/HuntressWig", "Terraria/HuntressJerkin", "Terraria/HuntressPants"],
        "shinobi infiltrator armor": ["Terraria/MonkAltHead", "Terraria/MonkAltShirt", "Terraria/MonkAltPants"],
        "valhalla knight armor": ["Terraria/SquireAltHead", "Terraria/SquireAltShirt", "Terraria/SquireAltPants"],
        "dark artist armor": ["Terraria/ApprenticeAltHead", "Terraria/ApprenticeAltShirt", "Terraria/ApprenticeAltPants"],
        "red riding armor": ["Terraria/HuntressAltHead", "Terraria/HuntressAltShirt", "Terraria/HuntressAltPants"],
        # Generic wiki entries: shown as a representative item (or the purchasable set).
        "extra jump": ["Terraria/CloudinaBottle"],
        "strings": ["Terraria/WhiteString"],
        "candles#purchased": ["CalamityMod/ResilientCandle", "CalamityMod/SpitefulCandle", "CalamityMod/VigorousCandle",
                              "CalamityMod/WeightlessCandle"],
        # Terraria wiki "Mixed" card names mounts, not their summoning items.
        "golf cart": ["Terraria/GolfCart"],
        "goat mount": ["Terraria/WallOfFleshGoatMountItem"],
        "unicorn mount": ["Terraria/BlessedApple"],
        "basilisk mount": ["Terraria/AncientHorn"],
        "winged slime mount": ["Terraria/QueenSlimeMountSaddle"],
        "scutlix mount": ["Terraria/BrainScrambler"],
        "cute fishron": ["Terraria/ShrimpyTruffle"],
        "ufo mount": ["Terraria/CosmicCarKey"],
        "counterweights": ["Terraria/BlackCounterweight", "Terraria/BlueCounterweight", "Terraria/GreenCounterweight",
                           "Terraria/PurpleCounterweight", "Terraria/RedCounterweight", "Terraria/YellowCounterweight"],
    }
    CLASS_WORDS = {
        "melee": ["melee"], "ranged": ["ranged"], "magic": ["magic", "mana"],
        "summon": ["minion", "summon", "sentr"], "rogue": ["rogue", "stealth"],
    }

    def resolve(self, name, cls):
        n = self.norm(name)
        if n in self.SPECIAL:
            keys = [k for k in self.SPECIAL[n] if any(it["key"] == k for it in self.items)]
            return keys
        m = re.match(r"^(.*) armor$", n)
        if n in self.by_name and not m:
            return [self.by_name[n][0]["key"]]
        if m:
            prefix = m.group(1)
            pieces = [it for it in self.items
                      if (it["headSlot"] > 0 or it["bodySlot"] > 0 or it["legSlot"] > 0) and not it["vanity"]
                      and self.norm(it["name"]).startswith(prefix + " ")]
            if not pieces:
                return [self.by_name[n][0]["key"]] if n in self.by_name else []
            heads = [p for p in pieces if p["headSlot"] > 0]
            rest = [p for p in pieces if p["headSlot"] <= 0]
            if len(heads) > 1 and cls in self.CLASS_WORDS:
                words = self.CLASS_WORDS[cls]
                match = [h for h in heads if any(w in " ".join(h["tooltip"]).lower() for w in words)]
                if len(match) >= 1:
                    heads = match[:1]
            return [p["key"] for p in heads[:2] + rest]
        return []


# ---------------------------------------------------------------- Calamity

# Rows the data check found to be wrong for the installed Calamity version (see docs/DATA-VERIFICATION.md).
def set_label(name):
    """Readable name of a multi-piece alternative: "Statigel armor" -> "Statigel Armor"."""
    name = plain(name)
    if name.lower().endswith(" armor"):
        name = name[:-6] + " Armor"
    return name


EXCLUDE = {
    ("ranged", "pre-mech", "Terraria/Uzi"): "Calamity 2.2.4: only from Angry Trappers and the Arms Dealer after Plantera.",
}

CAL_STAGES = ["pre-boss", "pre-evil1", "pre-evil2", "pre-skeletron", "pre-wof", "pre-mech", "post-mech1", "post-mech2",
              "pre-plantera", "pre-golem", "post-golem", "pre-lunar", "pre-moonlord", "pre-provi", "pre-polter", "pre-dog",
              "pre-yharon", "pre-scal-exo", "endgame"]


def calamity(names, notes):
    rows = []
    for page in ("Pre-Hardmode", "Hardmode", "Post-Moon_Lord"):
        text = open(os.path.join(SRC, f"data_{page}.wikitext"), encoding="utf-8").read()
        for _, t in templates(text, "ClassSetupsItem"):
            a = split_args(t)
            cls, stage, typ = a[1].strip(), a[2].strip(), a[3].strip()
            cell = a[4] if len(a) > 4 else ""
            note = plain(a[5]) if len(a) > 5 else ""
            sym = plain(a[6]) if len(a) > 6 else ""
            if len(sym) > 12:  # a note in the symbol column
                note, sym = (note + " " + sym).strip(), ""
            if typ == "accessorGeneral":
                typ = "accessoryGeneral"
            if stage in ("pre-scal", "pre-exo"):
                # Fight-specific extras: shown with the combined final-boss stage, with a note.
                note = (note + " " if note else "") + ("For Supreme Witch, Calamitas." if stage == "pre-scal" else "For the Exo Mechs.")
                stage = "pre-scal-exo"
            alternatives = []
            labels = []
            for name in item_names(cell):
                keys = names.resolve(name, cls)
                if keys and len(keys) == 1 and (cls, stage, keys[0]) in EXCLUDE:
                    report.append(f"calamity: excluded {keys[0]} at {cls}/{stage}: {EXCLUDE[(cls, stage, keys[0])]}")
                    continue
                if keys:
                    alternatives.append(keys)
                    labels.append(set_label(name) if len(keys) > 1 else None)
                else:
                    report.append(f"calamity: unresolved '{name}' ({cls}/{stage}/{typ})")
            if not alternatives:
                continue
            row = OrderedDict(**{"class": cls, "stage": stage, "type": typ, "items": alternatives})
            if any(labels):
                row["labels"] = labels
            if sym:
                row["flags"] = sym
            if note:
                row["note"] = notes.id_for(note)
            rows.append(row)
    return rows


# ---------------------------------------------------------------- vanilla

VAN_STAGES = OrderedDict([
    ("Pre-Bosses", "v-pre-boss"), ("Pre-Skeletron", "v-pre-skeletron"), ("Pre-Wall of Flesh", "v-pre-wof"),
    ("Pre-Mech Bosses", "v-pre-mech"), ("Pre-Plantera", "v-pre-plantera"), ("Pre-Golem", "v-pre-golem"),
    ("Pre-Lunatic Cultist", "v-pre-cultist"), ("Pre-Moon Lord", "v-pre-moonlord"), ("Endgame", "v-endgame"),
])
VAN_CLASSES = {"Melee": "melee", "Ranged": "ranged", "Magic": "magic", "Summoning": "summon", "Mixed": "all"}


def box_type(title, cls):
    """Row type (and extra flag) for a top-level box of the Terraria wiki infocards (revision 957024)."""
    t = plain(title).lower()
    if t == "armor":
        return "armor", ""
    if t in ("single-target weapons", "single target weapons"):
        return "weapon", ""
    if t == "weapons":
        # The "Mixed" card's weapons are side weapons any class can carry (Vampire Knives, Betsy's Wrath).
        return ("support", "") if cls == "all" else ("weapon", "")
    if t in ("crowd-control weapons", "crowd control weapons"):
        return "weapon", "C"
    if t == "crowd control":
        return ("ammo", "C") if cls == "ranged" else ("weapon", "C")
    if t == "support weapons":
        return "support", ""
    if t in ("ammunition", "single-target ammunition"):
        return "ammo", ""
    if t in ("crowd-control ammunition", "crowd control ammunition"):
        return "ammo", "C"
    if t == "accessories":
        return "accessory", ""
    if t == "mobility accessories" or t == "mounts":
        return "accessoryMobility", ""
    if t == "survivability accessories":
        return "accessoryDefense", ""
    if t == "offensive accessories":
        return "accessoryOffense", ""
    if t in ("buffs/potions", "buffs/potions/flasks", "potions"):
        return "buff", ""
    if t == "minions":
        return "minions", ""
    if t == "sentries":
        return "sentries", ""
    if t == "whips":
        return "weaponSummon", ""
    return None


def vanilla(names, notes):
    text = open(os.path.join(SRC, "tr_class_setups.wikitext"), encoding="utf-8").read()
    rows = []
    starts = [(m.start(), m.group(1), m.group(2)) for m in re.finditer(r"\{\{infocard/start\|type=([^|]*)\|name=([^|}]*)", text)]
    for idx, (pos, stage_name, cls_name) in enumerate(starts):
        end = text.find("{{infocard/end}}", pos)
        body = text[pos:end]
        if stage_name not in VAN_STAGES or cls_name not in VAN_CLASSES:
            continue
        stage, cls = VAN_STAGES[stage_name], VAN_CLASSES[cls_name]
        # Walk box starts/ends and items in order, tracking the box title stack.
        tokens = []
        # Box ends may carry a title ("{{infocard/box/end|title=Best}}"); they still close the innermost box.
        for m in re.finditer(r"\{\{infocard/box/start\|title=((?:[^{}]|\{\{[^{}]*\}\})*)\}\}|\{\{infocard/box/end(?:\|[^{}]*)?\}\}", body):
            tokens.append((m.start(), "start" if m.group(1) is not None else "end", m.group(1) or ""))
        for p, t in templates(body, "item"):
            tokens.append((p, "item", t))
        tokens.sort(key=lambda x: x[0])
        stack = []
        prev_item_end = -1
        for p, kind, payload in tokens:
            if kind == "start":
                stack.append(payload)
            elif kind == "end":
                if stack:
                    stack.pop()
            else:
                if not stack:
                    continue
                args = split_args(payload)
                pos_args = [x for x in args[1:] if not re.match(r"^\s*\w+\s*=", x)]
                if not pos_args:
                    continue
                name = pos_args[0].strip()
                main = stack[0]
                kind = box_type(main, cls)
                if kind is None:
                    report.append(f"vanilla: box '{plain(main)}' ignored ({cls}/{stage})")
                    continue
                typ, flags = kind
                sub = ""
                for s in stack[1:]:
                    label = plain(s)
                    if label.lower() == "best":
                        flags = "B" + flags
                    elif label.lower() in ("second best", "great"):
                        pass
                    else:
                        sub = label
                if plain(main).lower() == "mounts":
                    sub = "Mount"
                note_parts = []
                if sub:
                    note_parts.append(sub)
                foot = re.findall(r"footnote\|name=([^}|]+)", payload)
                if "Corruption" in foot:
                    note_parts.append("Corruption worlds only.")
                if "Crimson" in foot:
                    note_parts.append("Crimson worlds only.")
                keys = names.resolve(name, cls)
                if not keys:
                    report.append(f"vanilla: unresolved '{name}' ({cls}/{stage}/{typ})")
                    continue
                # "A or B" written as two consecutive item templates joined by "or".
                between = body[prev_item_end:p] if prev_item_end >= 0 else ""
                prev_item_end = p + len(payload) + 4
                if rows and re.search(r"^\s*or\s*$", between.replace("<!--", "").replace("-->", "").strip(), re.I) \
                        and rows[-1]["stage"] == stage and rows[-1]["class"] == cls and rows[-1]["type"] == typ:
                    rows[-1]["items"].append(keys)
                    if len(keys) > 1 or "labels" in rows[-1]:
                        labels = rows[-1].setdefault("labels", [None] * (len(rows[-1]["items"]) - 1))
                        labels.append(set_label(name) if len(keys) > 1 else None)
                    continue
                row = OrderedDict(**{"class": cls, "stage": stage, "type": typ, "items": [keys]})
                if len(keys) > 1:
                    row["labels"] = [set_label(name)]
                if flags:
                    row["flags"] = flags
                if note_parts:
                    row["note"] = notes.id_for(" ".join(note_parts))
                rows.append(row)
    # Drop exact duplicates (the same item listed as "Best" in one box and again in another).
    seen = set()
    unique = []
    for r in rows:
        key = (r["class"], r["stage"], r["type"], json.dumps(r["items"]))
        if key in seen:
            continue
        seen.add(key)
        unique.append(r)
    return unique


class Notes:
    def __init__(self, prefix):
        self.prefix = prefix
        self.map = OrderedDict()

    def id_for(self, text):
        if text not in self.map:
            self.map[text] = f"{self.prefix}{len(self.map) + 1}"
        return self.map[text]


def write(path, source, url, version, retrieved, stages, rows):
    data = OrderedDict(source=source, sourceUrl=url, sourceVersion=version, retrieved=retrieved, stages=stages, rows=rows)
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write("{\n")
        for k in ("source", "sourceUrl", "sourceVersion", "retrieved"):
            f.write(f"  {json.dumps(k)}: {json.dumps(data[k], ensure_ascii=False)},\n")
        f.write(f"  \"stages\": {json.dumps(stages)},\n  \"rows\": [\n")
        f.write(",\n".join("    " + json.dumps(r, ensure_ascii=False) for r in rows))
        f.write("\n  ]\n}\n")


def main():
    if len(sys.argv) < 3:
        print(__doc__)
        sys.exit(2)
    cal_notes, van_notes = Notes("C"), Notes("V")
    cal = calamity(Names(sys.argv[1]), cal_notes)
    van = vanilla(Names(sys.argv[2]), van_notes)
    write(os.path.join(ROOT, "Data", "loadouts-calamity.json"),
          "Calamity Mod Wiki - Guide:Class setups (CC BY-NC-SA 3.0)",
          "https://calamitymod.wiki.gg/wiki/Guide:Class_setups", "2.1.2 (checked against 2.2.4)", "2026-09-24",
          CAL_STAGES, cal)
    write(os.path.join(ROOT, "Data", "loadouts-vanilla.json"),
          "Terraria Wiki - Guide:Class setups (CC BY-NC-SA 4.0)",
          "https://terraria.wiki.gg/wiki/Guide:Class_setups?oldid=957024", "1.4.4 (revision 957024, before 1.4.5 changes)", "2026-09-24",
          list(VAN_STAGES.values()), van)
    with open(os.path.join(HERE, "loadout-notes.json"), "w", encoding="utf-8", newline="\n") as f:
        json.dump({"calamity": {v: k for k, v in cal_notes.map.items()},
                   "vanilla": {v: k for k, v in van_notes.map.items()}}, f, ensure_ascii=False, indent=1)
    print(f"calamity rows: {len(cal)}  notes: {len(cal_notes.map)}")
    print(f"vanilla rows:  {len(van)}  notes: {len(van_notes.map)}")
    for line in report:
        print("  ", line)


if __name__ == "__main__":
    main()
