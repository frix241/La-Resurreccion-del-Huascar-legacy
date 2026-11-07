# Sergio — desarrollo de combate y mapa

**Cuenta GitHub:** `SrEstrada`  
**Rama sugerida:** `feature/sergio-combate-mapa`

## Alcance inicial

Toma tareas de navegación, combate, colisiones, límites y cartografía. Rutas candidatas (stage solo cambios propios):

- `Assets/_LRH/Scripts/Core/Health.cs`, `Combat.cs`, `Registry.cs`, `CombatReport.cs`
- `Assets/_LRH/Scripts/Runtime/ShipActor.cs`, `ProjectilePool.cs`, `InputAdapter.cs`, `ActorFactory.cs`
- `Assets/_LRH/Scripts/Runtime/OpenSea.cs`, `TerrainMap.cs`, `CartographyVisuals.cs`
- `Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs`
- `Assets/_LRH/Resources/Maps/pacific.json` (si existe/cambia; incluir su `.meta` correspondiente)

## Comprobación y mensajes de ejemplo

Para cambios de lógica ejecuta `dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj`; los tests de Unity se corren desde Test Runner o en el entorno configurado. Mensajes posibles: `fix(combat): detener avance al colisionar cascos` y `test(gameplay): cubrir límites de navegación`.

## Flujo Git de esta tarea (#3)

Empieza cuando la importación #1 esté integrada en `master`:

```bash
git switch master
git pull --ff-only origin master
git switch -c feature/sergio-combate-mapa
git status --short
```

Implementa primero los casos de colisión, límite y objetivo cercano del issue. Stage explícito de las rutas que cambiaste:

```bash
git add -- Assets/_LRH/Scripts/Runtime/ShipActor.cs Assets/_LRH/Scripts/Runtime/OpenSea.cs Assets/_LRH/Scripts/Runtime/TerrainMap.cs Assets/_LRH/Scripts/Runtime/CartographyVisuals.cs Assets/_LRH/Scripts/Core/Combat.cs Assets/_LRH/Scripts/Core/Registry.cs Assets/_LRH/Scripts/Core/Health.cs Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs Assets/_LRH/Resources/Maps/pacific.json Assets/_LRH/Resources/Maps/pacific.json.meta
git diff --cached --check
git diff --cached --stat
git diff --cached
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
```

Retira del stage cualquier archivo sin cambio propio con `git restore --staged -- ruta`. Luego registra y publica:

```bash
git commit -m "fix(combat): corregir colisiones y límites de navegación"
git push -u origin feature/sergio-combate-mapa
gh pr create --base master --head feature/sergio-combate-mapa --assignee SrEstrada --reviewer MauroSullcaMamani --title "Corregir navegación y colisiones" --body "Closes #3. Resume los casos corregidos y resultados reales de pruebas."
```

Adapta mensaje y resumen al diff. No abras PR sin commits propios; informa si PlayMode no pudo ejecutarse.
