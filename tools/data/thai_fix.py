"""
Replace the Thai text of given keys in tools/data/loc/*.py, keeping the English text and file layout.

    python tools/data/thai_fix.py fixes.json

fixes.json maps a key path as printed by the loc tables (e.g. "GUIDE.States.Available" or "E.KingSlime.Summon")
to the new Thai text, or to {"text": ..., "all": true} to change every copy of a shared (English, Thai) pair.
The script finds the pair, checks that the old Thai text directly follows its English text and replaces it.
Unknown keys or ambiguous matches are reported and nothing is written for them.
"""
import importlib.util
import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
LOC = os.path.join(HERE, 'loc')
MODULES = ('loc_ui', 'loc_misc', 'loc_entries', 'loc_guides')


def load(name):
    spec = importlib.util.spec_from_file_location(name, os.path.join(LOC, name + '.py'))
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def walk(d, prefix):
    for k, v in d.items():
        if isinstance(v, dict):
            yield from walk(v, prefix + k + '.')
        elif isinstance(v, tuple) and len(v) == 2:
            yield prefix + k, v


def literal(text):
    """How the tables write a string: double quotes, with \\n and \\" escaped."""
    return '"' + text.replace('\\', '\\\\').replace('"', '\\"').replace('\n', '\\n') + '"'


def main():
    fixes = json.load(open(sys.argv[1], encoding='utf-8'))
    index = {}
    for name in MODULES:
        module = load(name)
        for attr in ('GUIDE', 'MOD', 'E', 'G'):
            table = getattr(module, attr, None)
            if isinstance(table, dict):
                for key, pair in walk(table, attr + '.'):
                    index.setdefault(key, (name, pair))
    texts = {name: open(os.path.join(LOC, name + '.py'), encoding='utf-8').read() for name in MODULES}
    done, failed = 0, []
    for key, new in fixes.items():
        # {"text": ..., "all": true} replaces every tuple with the same English and Thai text (shared texts).
        replace_all = isinstance(new, dict) and new.get('all', False)
        if isinstance(new, dict):
            new = new['text']
        if key not in index:
            failed.append(f'{key}: unknown key')
            continue
        name, (en, th) = index[key]
        text = texts[name]
        en_lit, th_lit = literal(en), literal(th)
        pos = text.find(en_lit)
        hits = []
        while pos >= 0:
            after = text.find(th_lit, pos + len(en_lit))
            # The Thai text follows its English text directly (same tuple), allowing for a line break.
            if after >= 0 and text[pos + len(en_lit):after].strip() in (',',):
                hits.append(after)
            pos = text.find(en_lit, pos + 1)
        if not hits or (len(hits) > 1 and not replace_all):
            failed.append(f'{key}: {len(hits)} matches')
            continue
        for at in reversed(hits):
            text = text[:at] + literal(new) + text[at + len(th_lit):]
        texts[name] = text
        done += 1
    for name, text in texts.items():
        open(os.path.join(LOC, name + '.py'), 'w', encoding='utf-8').write(text)
    print(f'{done} replaced, {len(failed)} failed')
    for f in failed:
        print('  ' + f)


if __name__ == '__main__':
    main()
