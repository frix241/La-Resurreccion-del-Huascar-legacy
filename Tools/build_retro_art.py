"""Prepare full-colour illustrated LRH art at its native display grid."""
from pathlib import Path
import json
import re
import subprocess
import tempfile
import uuid

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / 'Assets/_LRH/Art/RetroSource'
OUT = ROOT / 'Assets/_LRH/Resources/Retro'
OUT.mkdir(parents=True, exist_ok=True)
subprocess.run(['python3', str(ROOT / 'Tools/build_retro_ui.py')], check=True)

def run(*args):
    subprocess.run(['magick', *map(str, args)], check=True)

manifest = []

def asset(name, source, size, crop=None, transparent=False, fit=False):
    with tempfile.TemporaryDirectory(prefix='lrh-illustrated-') as folder:
        folder = Path(folder)
        working = folder / 'working.png'
        args = [SOURCE / source]
        if crop:
            args += ['-crop', crop, '+repage']
        if fit:
            args += ['-trim', '+repage']
        args += ['-filter', 'Lanczos', '-resize', size if fit else size + '!',
                 '-gravity', 'center', '-background', 'none', '-extent', size,
                 '-define', 'png:color-type=6', working]
        run(*args)
        run(working, OUT / f'{name}.png')
    manifest.append({'file': f'{name}.png', 'size': size, 'transparent': transparent})

# Full-HD scenes are kept in natural colour and are never reduced to a pixel grid.
for name in ('menu', 'victory', 'defeat'):
    asset(f'{name}-background', f'{name}.png', '1920x1080')

# The generated atlases are 1774x887: four 443px cells by two 443px cells.
# Keep aspect ratios when fitting irregular silhouettes into runtime sprite bounds.
CELL = 443
for i, name in enumerate(('huascar', 'patrol', 'gunboat', 'convoy')):
    asset(name, 'ships.png', '192x384', f'{CELL}x{CELL}+{i * CELL}+0', True, True)

# The same illustrated atlas supplies the battery, island, shell and wake.
run(SOURCE / 'ships.png', '-crop', f'{CELL}x{CELL}+0+{CELL}', '+repage', '-trim', '+repage',
    '-background', 'none', '-gravity', 'center', '-extent', '384x384', SOURCE / 'battery.png')
for name, size, i in (('battery', '256x256', 0), ('island', '256x256', 1),
                      ('projectile', '96x96', 2), ('wake', '192x192', 3)):
    source = 'battery.png' if name == 'battery' else 'ships.png'
    crop = None if name == 'battery' else f'{CELL}x{CELL}+{i * CELL}+{CELL}'
    asset(name, source, size, crop, True, True)

for i, name in enumerate(('water1', 'shallow0', 'shallow1',
                           'sand0', 'sand1', 'stone0', 'stone1')):
    index = i + 1
    asset(name, 'terrain.png', '384x384', f'{CELL}x{CELL}+{(index % 4) * CELL}+{(index // 4) * CELL}')
# Use the separately authored calm seamless tile for the full-screen sea layer.
asset('water0', 'ocean-tile.png', '384x384')

for i, name in enumerate(('rocks', 'reef', 'shrub', 'palms', 'pier', 'ruins', 'cliff', 'crates')):
    asset(name, 'props.png', '256x256', f'{CELL}x{CELL}+{(i % 4) * CELL}+{(i // 4) * CELL}', True, True)

for i, name in enumerate(('panel', 'normal', 'hover', 'pressed', 'disabled', 'header')):
    asset(name, 'ui.png', '256x256', f'512x512+{(i % 3) * 512}+{(i // 3) * 512}', True)

swatches = [
    ('ink', '#101827'), ('deep_sea', '#102234'), ('sea_glass', '#277a78'),
    ('pine', '#344c43'), ('parchment', '#d9c8a2'), ('aged_brass', '#bfaa78'),
    ('signal_crimson', '#a63e42'), ('slate', '#4c5a68'),
]
run(*sum((['xc:' + color] for _, color in swatches), []), '+append', OUT / 'palette.png')
(OUT / 'palette.json').write_text(json.dumps({
    'style': 'illustrated gothic naval adventure',
    'colors': [{'name': name, 'hex': color} for name, color in swatches],
    'assets': manifest,
}, indent=2) + '\n')

template = (ROOT / 'Assets/_LRH/Resources/Pixel/huascar.png.meta').read_text()
for png in OUT.glob('*.png'):
    meta = Path(str(png) + '.meta')
    if not meta.exists():
        meta.write_text(re.sub(r'guid: \w+', 'guid: ' + uuid.uuid4().hex, template, count=1))
    content = meta.read_text()
    content = content.replace('filterMode: 0', 'filterMode: 1')
    content = content.replace('filterMode: 2', 'filterMode: 1')
    content = content.replace('isReadable: 0', 'isReadable: 1')
    content = content.replace('spriteGenerateFallbackPhysicsShape: 1', 'spriteGenerateFallbackPhysicsShape: 0')
    meta.write_text(content)

print(f'Prepared {len(manifest)} full-colour illustrated assets at display-ready sizes')
