# Christian Yana — arte, audio y desarrollo

**Cuenta GitHub:** `ChristianAlexYana`  
**Rama sugerida:** `feature/christian-arte-audio`

Christian trabaja tanto en contenido visual/sonoro como en integración de código relacionada. Rutas candidatas (stage solo cambios propios):

- `Assets/_LRH/Art/**`
- `Assets/_LRH/Resources/Retro/**`, `Assets/_LRH/Resources/Audio/**`
- `Tools/build_retro_art.py`, `Tools/build_retro_ui.py`, `Tools/generate_audio.py`
- `Assets/_LRH/Scripts/Runtime/SoundFx.cs`, `Visuals.cs`, `UiTheme.cs`, `UiVisuals.cs`

Comprueba que los recursos se importen y se vean/escuchen en Unity. Si un asset cambia, incluye su `.meta` cuando corresponda y registra su licencia/procedencia en `docs/creditos_assets.md` si aplica.

## Flujo Git de esta tarea (#4)

Después de que #1 esté integrado en `master`:

```bash
git switch master
git pull --ff-only origin master
git switch -c feature/christian-arte-audio
git status --short
```

Modifica recursos y/o integración relacionados con el issue. Stagea solo el grupo de archivos que efectivamente cambiaste. Por ejemplo:

```bash
git add -A -- Assets/_LRH/Art Assets/_LRH/Resources Tools/build_retro_art.py Tools/build_retro_ui.py Tools/generate_audio.py Assets/_LRH/Scripts/Runtime/Visuals.cs Assets/_LRH/Scripts/Runtime/UiTheme.cs Assets/_LRH/Scripts/Runtime/UiVisuals.cs Assets/_LRH/Scripts/Runtime/SoundFx.cs docs/creditos_assets.md
git diff --cached --check
git diff --cached --stat
git diff --cached
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
```

Este `git add` cubre áreas candidatas: revisa el stage y quita rutas intactas con `git restore --staged -- ruta`. Incluye `.meta` de assets Unity nuevos/modificados. Adjunta capturas y pruebas reales en el PR.

```bash
git commit -m "feat(art): unificar presentación visual y audio"
git push -u origin feature/christian-arte-audio
gh pr create --base master --head feature/christian-arte-audio --assignee ChristianAlexYana --reviewer SrEstrada --title "Unificar arte y audio" --body "Closes #4. Resume los recursos cambiados, capturas y comprobaciones reales."
```

Adapta el commit al diff. No abras PR sin commits propios para revisar.
