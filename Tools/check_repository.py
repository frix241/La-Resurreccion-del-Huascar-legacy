"""Validate the tracked delivery without opening Unity or modifying files."""
import json
from pathlib import Path
import re
import subprocess
import sys

root = Path(__file__).resolve().parents[1]
files = subprocess.check_output(['git', 'ls-files', '-z'], cwd=root).decode().split('\0')
errors = []
count = 0
for name in filter(None, files):
    path = Path(name)
    if (any(p.lower() in {'library', 'logs', 'temp', 'usersettings', 'build', 'builds', 'bin', 'obj', '__pycache__'} for p in path.parts)
            or name.startswith('mono_crash.') or path.suffix.lower() in {'.zip', '.log', '.blob'}):
        errors.append(f'{name}: generated artifact must not be tracked')
    full = root / name
    if not full.is_file():
        errors.append(f'{name}: tracked file missing')
        continue
    if full.suffix == '.json':
        try:
            json.loads(full.read_text(encoding='utf-8-sig'))
            count += 1
        except (ValueError, UnicodeError) as exc:
            errors.append(f'{name}: invalid JSON: {exc}')
    if full.suffix == '.py':
        try:
            compile(full.read_text(encoding='utf-8'), name, 'exec')
        except SyntaxError as exc:
            errors.append(f'{name}: {exc}')
    if full.suffix in {'.cs', '.py', '.json', '.yml', '.yaml', '.unity', '.asmdef'}:
        if re.search(r'^(?:<{7} |={7}$|>{7} )', full.read_text(encoding='utf-8-sig'), re.M):
            errors.append(f'{name}: unresolved merge conflict')
# Keep Unity asset identities stable across machines and branches.
guids = {}
for name in filter(None, files):
    path = Path(name)
    if not name.startswith('Assets/'):
        continue
    full = root / name
    if path.suffix != '.meta':
        if not Path(str(full) + '.meta').is_file():
            errors.append(f'{name}: Unity metadata missing')
        continue
    match = re.search(r'^guid: ([0-9a-f]{32})$', full.read_text(encoding='utf-8-sig'), re.M)
    if not match:
        errors.append(f'{name}: invalid Unity metadata GUID')
    elif match[1] in guids:
        errors.append(f'{name}: duplicate Unity GUID also in {guids[match[1]]}')
    else:
        guids[match[1]] = name
for err in errors:
    print(err, file=sys.stderr)
print(f'Repository checks: {len(errors)} errors; {count} JSON files parsed.')
sys.exit(bool(errors))
