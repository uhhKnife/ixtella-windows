"""Restore Sprite geometry that AssetRipper 2.0.0 exported wrongly.

For tightly-trimmed sprites, AssetRipper writes the trimmed render rect as the
sprite's m_Rect and shifts m_Border/m_Pivot/m_Offset to match. That breaks
9-sliced and tiled UI images (e.g. UiBtnIcon_00_01: 100x100 with 50px borders
became 91.85x91.85 with a ~0 px centre -> millions of tiles, "Mesh can not have
more than 65000 vertices"), and shifts SetNativeSize / preserveAspect sizes.

This copies m_Rect, m_Offset, m_Border, m_Pivot and m_RD.textureRectOffset back
from the original game data, matching sprites by m_RenderDataKey and checking
the name and m_RD.textureRect agree.

usage: python fix_sprites.py <data.unity3d from the APK> <Unity project dir> [--dry-run]
Needs UnityPy.
"""
import os, re, sys
import UnityPy

FLOAT = r"-?[0-9.eE+-]+|-?Infinity|NaN"


def guid_str(d):
    return "".join("".join("%x" % ((x >> (4 * j)) & 0xF) for j in range(8)) for x in d)


def f(v):
    """Format a float the way Unity's YAML writer does (shortest round-trip)."""
    v = float(v)
    if v == int(v) and abs(v) < 1e15:
        return str(int(v))
    s = repr(v)
    # Unity writes floats as 32-bit; use the shortest repr that round-trips float32
    import struct
    f32 = struct.unpack("<f", struct.pack("<f", v))[0]
    for p in range(6, 10):
        t = "%.*g" % (p, f32)
        if struct.unpack("<f", struct.pack("<f", float(t)))[0] == f32:
            return t
    return s


def load_original(path):
    env = UnityPy.load(path)
    out = {}
    for obj in env.objects:
        if obj.type.name != "Sprite":
            continue
        d = obj.read_typetree()
        key = d["m_RenderDataKey"]
        g = key[0]
        k = (guid_str([g["data[0]"], g["data[1]"], g["data[2]"], g["data[3]"]]), int(key[1]))
        if k in out:
            print("WARNING duplicate render key", k, d["m_Name"])
        out[k] = d
    return out


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    data, proj = sys.argv[1], sys.argv[2]
    dry = "--dry-run" in sys.argv
    orig = load_original(data)
    print("original sprites:", len(orig))

    fixed = same = unmatched = 0
    for root, _, files in os.walk(os.path.join(proj, "Assets")):
        for fn in files:
            if not fn.endswith(".asset"):
                continue
            p = os.path.join(root, fn)
            raw = open(p, "rb").read()
            if b"\nSprite:" not in raw:
                continue
            nl = "\r\n" if b"\r\n" in raw else "\n"
            s = raw.decode("utf-8")
            m = re.search(r"m_RenderDataKey:\s*\n\s*([0-9a-f]{32}): (-?\d+)", s)
            name = re.search(r"\n  m_Name: (.*)", s).group(1).strip()
            if not m or (m.group(1), int(m.group(2))) not in orig:
                unmatched += 1
                print("UNMATCHED", p)
                continue
            o = orig[(m.group(1), int(m.group(2)))]
            if o["m_Name"] != name:
                print("NAME MISMATCH", p, name, "vs", o["m_Name"])
                unmatched += 1
                continue
            # sanity: render rect must agree
            tr = re.search(r"\n    textureRect:\s*\n\s*serializedVersion: 2\s*\n\s*x: (" + FLOAT + r")\s*\n\s*y: (" + FLOAT + r")\s*\n\s*width: (" + FLOAT + r")\s*\n\s*height: (" + FLOAT + r")", s)
            otr = o["m_RD"]["textureRect"]
            if not tr or any(abs(float(a) - otr[b]) > 1e-3 for a, b in zip(tr.groups(), ("x", "y", "width", "height"))):
                print("TEXTURERECT MISMATCH", p)
                unmatched += 1
                continue

            r, off, b, pv = o["m_Rect"], o["m_Offset"], o["m_Border"], o["m_Pivot"]
            tro = o["m_RD"]["textureRectOffset"]
            new = s
            new = re.sub(r"(\n  m_Rect:\s*\n\s*serializedVersion: 2\s*\n\s*x: )(" + FLOAT + r")(\s*\n\s*y: )(" + FLOAT + r")(\s*\n\s*width: )(" + FLOAT + r")(\s*\n\s*height: )(" + FLOAT + r")",
                         lambda mm: mm.group(1) + f(r["x"]) + mm.group(3) + f(r["y"]) + mm.group(5) + f(r["width"]) + mm.group(7) + f(r["height"]), new, count=1)
            new = re.sub(r"\n  m_Offset: \{[^}]*\}", "\n  m_Offset: {x: %s, y: %s}" % (f(off["x"]), f(off["y"])), new, count=1)
            new = re.sub(r"\n  m_Border: \{[^}]*\}", "\n  m_Border: {x: %s, y: %s, z: %s, w: %s}" % (f(b["x"]), f(b["y"]), f(b["z"]), f(b["w"])), new, count=1)
            new = re.sub(r"\n  m_Pivot: \{[^}]*\}", "\n  m_Pivot: {x: %s, y: %s}" % (f(pv["x"]), f(pv["y"])), new, count=1)
            # first textureRectOffset belongs to m_RD (the second one is m_AtlasRD)
            new = re.sub(r"\n    textureRectOffset: \{[^}]*\}", "\n    textureRectOffset: {x: %s, y: %s}" % (f(tro["x"]), f(tro["y"])), new, count=1)
            if new == s:
                same += 1
                continue
            fixed += 1
            if not dry:
                open(p, "wb").write(new.encode("utf-8"))
    print("fixed:", fixed, "already correct:", same, "unmatched:", unmatched)


if __name__ == "__main__":
    main()
