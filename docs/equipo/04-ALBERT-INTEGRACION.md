# Albert Riveros — integración y CI

**Cuenta GitHub:** `frix241`  
**Rama de la importación inicial:** `chore/project-baseline`

Alcance de integración, interfaz, documentación y configuración del proyecto:

- `Assets/_LRH/Scripts/Runtime/GameUi.cs`, `GameRoot.cs`
- `Assets/_LRH/Scenes/Main.unity`
- `ProjectSettings/**`, `.github/**`
- `README.md`, `CONTRIBUTING.md`, `docs/**`

El workflow `.github/workflows/ci.yml` ejecuta los chequeos .NET en GitHub Actions para PRs y pushes a `master`/`develop`. La comprobación local es `dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj`. No declares una ejecución de Actions hasta que el run exista en GitHub.

## Flujo Git de la importación inicial (#1)

El remoto comienza con `master` vacío y esta tarea prepara el primer PR con el estado fuente revisado. En la carpeta local del proyecto que ya contiene los archivos revisados, ejecuta:

```bash
git remote add origin https://github.com/frix241/La-Resurreccion-del-Huascar.git
git fetch origin
git switch -c chore/project-baseline origin/master
```

La rama queda creada dentro de esa carpeta y conserva el árbol de trabajo local. No copies `.git`, `Library`, `Logs`, `UserSettings`, `Build`, `Builds`, `Tests/CoreChecks/bin`, `Tests/CoreChecks/obj` ni volcados `mono_crash`. Antes de registrar:

```bash
git status --short
git diff --check
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
```

Revisa el stage explícitamente y confirma la importación con la identidad real del integrador:

```bash
git add .github .gitignore Assets CONTRIBUTING.md Packages ProjectSettings README.md Tests Tools docs
git diff --cached --check
git diff --cached --stat
git diff --cached
git commit -m "chore(repo): importar estado inicial revisado"
git push -u origin chore/project-baseline
gh pr create --base master --head chore/project-baseline --assignee frix241 --reviewer SrEstrada --title "Importar el estado inicial del proyecto" --body "Closes #1. Importación inicial por el integrador; incluye CI .NET. No representa autoría retrospectiva por desarrollador."
```

No añadas la build Linux ni archivos generados. Después de abrir el PR, espera el run de Actions y añade su enlace/resultado al issue. Las tareas #2, #3 y #4 deben partir de `master` una vez integrada esta importación.
