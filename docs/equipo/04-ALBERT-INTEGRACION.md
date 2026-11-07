# Albert — integración y CI

**Cuenta:** `frix241`. **Issue:** #1. **Rama:** `chore/project-baseline`.

La importación inicial ya está publicada en el [PR #5](https://github.com/frix241/La-Resurreccion-del-Huascar/pull/5). Sergio y Christian tienen solicitada la revisión. Para continuar desde un clon nuevo:

```bash
git clone https://github.com/frix241/La-Resurreccion-del-Huascar.git
cd La-Resurreccion-del-Huascar
git fetch origin
git switch --track origin/chore/project-baseline
```

Si la rama ya existe en tu clon, usa `git switch chore/project-baseline` y `git pull --ff-only`.

Para una corrección real de configuración o documentación en el PR:

```bash
git status --short
git add -- .github .gitattributes .gitignore CONTRIBUTING.md README.md docs/GUIA_COMMITS.md docs/REPOSITORIO.md docs/equipo Tools/check_repository.py
git diff --cached --check
git diff --cached --stat
git diff --cached
python Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "ci: ajustar validación y documentación del repositorio"
git push origin chore/project-baseline
gh pr checks 5 --repo frix241/La-Resurreccion-del-Huascar
```

Stagea solo las rutas modificadas y adapta el mensaje al diff. El PR existente se actualiza con el push. Pide aprobación a otro integrante después de los checks; `master` exige una aprobación independiente, ambos checks y conversaciones resueltas.

Después del merge, las tareas #2–#4 comienzan desde `master`. Para otra tarea de Albert crea `feature/albert-integracion-ci` desde `master` actualizado y abre otro PR asociado a su issue real.
