"""Build a six-state illustrated, nine-slice UI atlas from smooth raster geometry."""
from pathlib import Path
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / 'Assets/_LRH/Art/RetroSource/ui.png'
OUT.parent.mkdir(parents=True, exist_ok=True)

STATES = [
    ('panel', '#101827', '#bfaa78', '#203247'),
    ('normal', '#102234', '#aa9467', '#1a3547'),
    ('hover', '#153a42', '#76b2a3', '#20515a'),
    ('pressed', '#0b1421', '#8c704d', '#172333'),
    ('disabled', '#222b37', '#59616b', '#2e3947'),
    ('header', '#142437', '#bfaa78', '#20364b'),
]

def draw_state(path, base, brass, highlight):
    subprocess.run([
        'magick', '-size', '512x512', 'xc:none', '-fill', base,
        '-stroke', '#050b13', '-strokewidth', '12',
        '-draw', 'roundrectangle 8,8 504,504 34,34',
        str(path),
    ], check=True)
    commands = [
        'magick', str(path),
        '-fill', 'none',
        '-stroke', brass, '-strokewidth', '5',
        '-draw', 'roundrectangle 18,18 494,494 29,29',
        '-stroke', '#405269', '-strokewidth', '2',
        '-draw', 'roundrectangle 29,29 483,483 23,23',
        '-stroke', brass, '-strokewidth', '4',
        '-draw', 'path "M 38,54 C 38,45 45,38 54,38 M 38,46 C 42,46 46,42 46,38"',
        '-draw', 'path "M 474,54 C 474,45 467,38 458,38 M 474,46 C 470,46 466,42 466,38"',
        '-draw', 'path "M 38,458 C 38,467 45,474 54,474 M 38,466 C 42,466 46,470 46,474"',
        '-draw', 'path "M 474,458 C 474,467 467,474 458,474 M 474,466 C 470,466 466,470 466,474"',
        '-stroke', highlight, '-strokewidth', '2', '-draw', 'line 118,52 394,52',
        '-stroke', 'none', '-fill', brass,
        '-draw', 'circle 38,38 42,38 circle 474,38 478,38 circle 38,474 42,474 circle 474,474 478,474',
        str(path),
    ]
    subprocess.run(commands, check=True)

with tempfile.TemporaryDirectory(prefix='lrh-ui-') as tmp:
    tmp = Path(tmp)
    tiles = []
    for name, base, brass, highlight in STATES:
        tile = tmp / f'{name}.png'
        draw_state(tile, base, brass, highlight)
        tiles.append(str(tile))
    top, bottom = tmp / 'top.png', tmp / 'bottom.png'
    subprocess.run(['magick', *tiles[:3], '+append', str(top)], check=True)
    subprocess.run(['magick', *tiles[3:], '+append', str(bottom)], check=True)
    subprocess.run(['magick', str(top), str(bottom), '-append', str(OUT)], check=True)
print(f'Built illustrated six-state UI atlas: {OUT}')
