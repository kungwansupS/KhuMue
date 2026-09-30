"""
Validates the shipped guide data against a live data dump (tools/RunDataDump.ps1 -Profile full)
and the localization files.

  python tools/data/validate.py <dump-folder>

Checks:
  * every item / NPC key referenced by Data/*.json exists in the dumped game registries
  * every condition flag used is registered in Core/Flags.cs
  * every progression entry has Summary/Summon text in en-US and th-TH
  * every upgrade, article, stage and loadout note has its texts
  * en-US and th-TH define the same localization keys, with the same {n} placeholders
  * no guide key collides with keys an installed Thai translation pack overrides
Exit code 1 on any error.
"""
import json
import os
import re
import sys

ROOT = os.path.normpath(os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", ".."))
errors = []
warnings = []


def load(path):
    with open(path, encoding="utf-8-sig") as f:
        return json.load(f)


def hjson_keys(path):
    """Very small HJSON reader for the subset used by this mod (nested objects, key: value, ''' blocks)."""
    keys = {}
    stack = []
    with open(path, encoding="utf-8-sig") as f:
        lines = f.read().split("\n")
    i = 0
    while i < len(lines):
        raw = lines[i]
        line = raw.strip()
        i += 1
        if not line or line.startswith("//") or line.startswith("#"):
            continue
        if line == "}":
            stack.pop()
            continue
        m = re.match(r'^([A-Za-z0-9_.\-]+)\s*:\s*\{\s*$', line)
        if m:
            stack.append(m.group(1))
            continue
        m = re.match(r"^([A-Za-z0-9_.\-]+)\s*:\s*(.*)$", line)
        if not m:
            errors.append(f"{os.path.basename(path)}: cannot parse line {i}: {line[:60]}")
            continue
        key, value = m.group(1), m.group(2)
        # tModLoader rewrites long texts as "key:" followed by a ''' block on the next line.
        if value == "" and i < len(lines) and lines[i].strip().startswith("'''"):
            value = lines[i].strip()
            i += 1
        if value.startswith("'''"):
            block = []
            rest = value[3:]
            if rest.endswith("'''") and len(rest) >= 3:
                block.append(rest[:-3])
            else:
                while i < len(lines):
                    l2 = lines[i]
                    i += 1
                    if l2.strip().endswith("'''"):
                        block.append(l2.strip()[:-3])
                        break
                    block.append(l2.strip())
            value = "\n".join(b for b in block).strip()
        elif value.startswith('"') and value.endswith('"') and len(value) >= 2:
            value = value[1:-1]
        keys[".".join(stack + [key])] = value
    return keys


def main():
    if len(sys.argv) < 2:
        print(__doc__)
        sys.exit(2)
    dump = sys.argv[1]
    items = {i["key"] for i in load(os.path.join(dump, "items.json"))}
    npcs = {n["key"] for n in load(os.path.join(dump, "npcs.json"))}
    mods = {m["name"] for m in load(os.path.join(dump, "mods.json"))}

    flags_src = open(os.path.join(ROOT, "Core", "Flags.cs"), encoding="utf-8").read()
    flags = set(re.findall(r'Add\("([^"]+)"', flags_src))
    for flag, _ in re.findall(r'\("([A-Za-z0-9]+)", "([a-z0-9]+)"\)', flags_src):
        flags.add("cal." + flag)

    def check_condition(where, text):
        if not text:
            return
        for token in re.split(r"[&|!()]", text):
            token = token.strip()
            if not token or token in ("always", "never", "true", "false"):
                continue
            if token.startswith("mod:"):
                continue
            if token not in flags and not token.startswith("bc."):
                errors.append(f"{where}: unknown flag '{token}'")

    def owner_loaded(key):
        return key.split("/")[0] in mods or key.startswith("Terraria/")

    # progression
    prog = load(os.path.join(ROOT, "Data", "progression.json"))["entries"]
    ids = {e["id"] for e in prog}
    for e in prog:
        where = "progression:" + e["id"]
        for k in e.get("npcs", []):
            if owner_loaded(k) and k not in npcs:
                errors.append(f"{where}: NPC '{k}' not found")
        for k in e.get("summons", []):
            if owner_loaded(k) and k not in items:
                errors.append(f"{where}: item '{k}' not found")
        for field in ("done", "available", "show"):
            check_condition(where + "." + field, e.get(field))
        if e.get("parent") and e["parent"] not in ids:
            errors.append(f"{where}: parent '{e['parent']}' missing")
        for r in e.get("replaces", []):
            if r not in ids:
                errors.append(f"{where}: replaces unknown '{r}'")

    # stages
    stages = load(os.path.join(ROOT, "Data", "stages.json"))
    for ladder in ("calamity", "vanilla"):
        for s in stages.get(ladder, []):
            check_condition(f"stages:{ladder}:{s['id']}", s.get("when"))
            for t in s.get("targets", []):
                if t not in ids:
                    errors.append(f"stages:{ladder}:{s['id']}: target '{t}' missing")

    # loadouts
    for name in ("loadouts-calamity.json", "loadouts-vanilla.json"):
        path = os.path.join(ROOT, "Data", name)
        if not os.path.exists(path):
            warnings.append(f"{name} missing")
            continue
        data = load(path)
        for row in data["rows"]:
            for alt in row["items"]:
                for k in alt:
                    if owner_loaded(k) and k not in items:
                        errors.append(f"{name}: item '{k}' ({row['class']}/{row['stage']}/{row['type']}) not found")

    # upgrades & guides
    upgrades = load(os.path.join(ROOT, "Data", "upgrades.json"))
    for u in upgrades:
        if owner_loaded(u["item"]) and u["item"] not in items:
            errors.append(f"upgrades:{u['id']}: item '{u['item']}' not found")
        for field in ("available", "requires", "show"):
            check_condition(f"upgrades:{u['id']}.{field}", u.get(field))
    articles = load(os.path.join(ROOT, "Data", "guides.json"))
    for a in articles:
        if a.get("icon") and owner_loaded(a["icon"]) and a["icon"] not in items:
            errors.append(f"guides:{a['id']}: icon '{a['icon']}' not found")

    # localization
    en = hjson_keys(os.path.join(ROOT, "Localization", "en-US.hjson"))
    th = hjson_keys(os.path.join(ROOT, "Localization", "th-TH.hjson"))
    for k in sorted(set(en) - set(th)):
        errors.append(f"th-TH missing key {k}")
    for k in sorted(set(th) - set(en)):
        errors.append(f"en-US missing key {k} (present in th-TH)")
    root = "Mods.KhuMue.Guide."
    prefix = root + "Entries."
    for e in prog:
        for field in ("Summary", "Short", "Summon"):
            key = prefix + e["id"] + "." + field
            if e["kind"] == "miniboss" and field == "Summon":
                continue
            if key not in en:
                errors.append(f"en-US missing {key}")
    for u in upgrades:
        for field in ("Effect", "Where"):
            if f"{root}Upgrades.Items.{u['id']}.{field}" not in en:
                errors.append(f"en-US missing upgrade text {u['id']}.{field}")
    for a in articles:
        if f"{root}Guides.{a['id']}.Title" not in en:
            errors.append(f"en-US missing article title {a['id']}")
        if f"{root}Guides.Categories.{a['category']}" not in en:
            errors.append(f"en-US missing article category {a['category']}")
        for sec in a["sections"]:
            if f"{root}Guides.{a['id']}.{sec}.Body" not in en:
                errors.append(f"en-US missing article section {a['id']}.{sec}.Body")

    # fishing / Angler sources (tools/data/obtain_sources.py)
    obtain = load(os.path.join(ROOT, "Data", "obtain.json"))
    for k, infos in obtain.items():
        if owner_loaded(k) and k not in items:
            errors.append(f"obtain: item '{k}' not found")
        for info in infos:
            if info["how"] not in ("fishing", "angler", "chest"):
                errors.append(f"obtain:{k}: unknown source '{info['how']}'")
            if info["how"] == "chest" and f"{root}Items.Chest.{info.get('chest')}" not in en:
                errors.append(f"obtain:{k}: no text for chest '{info.get('chest')}' (Items.Chest.*)")
            for field in ("with", "without"):
                if info.get(field) and info[field] not in mods:
                    errors.append(f"obtain:{k}: {field} unknown mod '{info[field]}'")
            for place in info.get("where", []):
                for tag in place:
                    if tag.startswith("after:"):
                        if tag[6:] not in ids:
                            errors.append(f"obtain:{k}: '{tag}' is not a progression entry")
                    elif tag != "crate" and f"{root}Items.Where.{tag}" not in en:
                        errors.append(f"obtain:{k}: no text for place '{tag}' (Items.Where.{tag})")
    for ladder in ("calamity", "vanilla"):
        for st in stages.get(ladder, []):
            if f"{root}Stages.{st['id']}.Name" not in en:
                errors.append(f"en-US missing stage name {st['id']}")
    for name in ("loadouts-calamity.json", "loadouts-vanilla.json"):
        for row in load(os.path.join(ROOT, "Data", name))["rows"]:
            if row.get("note") and f"{root}LoadoutNotes.{row['note']}" not in en:
                errors.append(f"en-US missing loadout note {row['note']}")
    # Same placeholders in both languages (a missing {1} is harmless, an extra one throws at runtime).
    for k in set(en) & set(th):
        pe = set(re.findall(r"\{(\d+)\}", en[k]))
        pt = set(re.findall(r"\{(\d+)\}", th[k]))
        if pt - pe:
            errors.append(f"th-TH {k}: placeholders {sorted(pt - pe)} not in en-US")
        if pe - pt:
            warnings.append(f"th-TH {k}: placeholders {sorted(pe - pt)} unused")
    # A Thai pack that still carries the prototype's texts overrides any key with the same name.
    pack = os.path.normpath(os.path.join(ROOT, "..", "ThaiOfflineAllMods", "ThaiOfflineAllMods", "Translations", "th-TH.json"))
    if os.path.exists(pack):
        def flat(o, p=""):
            if isinstance(o, dict):
                for kk, vv in o.items():
                    yield from flat(vv, p + ("." if p else "") + kk)
            else:
                yield p
        packed = {k for k in flat(load(pack)) if k.startswith("Mods.KhuMue.")}
        for k in sorted((set(en) & packed) - {"Mods.KhuMue.Keybinds.ToggleGuide.DisplayName"}):
            errors.append(f"key overridden by the Thai pack: {k}")
    # item/entry references inside text
    for lang, table in (("en-US", en), ("th-TH", th)):
        for k, v in table.items():
            for ref in re.findall(r"\[(?:i|item):([^\]]+)\]", v):
                if "/" in ref and owner_loaded(ref) and ref not in items:
                    errors.append(f"{lang} {k}: item tag '{ref}' not found")
            for ref in re.findall(r"\[e:([^\]]+)\]", v):
                if ref not in ids:
                    errors.append(f"{lang} {k}: entry link '{ref}' not found")
            for ref in re.findall(r"\[npc:([^\]]+)\]", v):
                if owner_loaded(ref) and ref not in npcs:
                    errors.append(f"{lang} {k}: npc tag '{ref}' not found")

    for w in warnings:
        print("WARN ", w)
    for e in errors:
        print("ERROR", e)
    print(f"{len(errors)} error(s), {len(warnings)} warning(s)")
    sys.exit(1 if errors else 0)


if __name__ == "__main__":
    main()
