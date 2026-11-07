# Mauro Sullca Mamani — desarrollo de campaña

**Cuenta GitHub:** `MauroSullcaMamani`  
**Rama sugerida:** `feature/mauro-progresion`

## Alcance inicial

Toma tareas de progresión, persistencia y reglas de misión. Antes de empezar, confirma el issue asignado y crea la rama desde la rama base acordada por el integrador.

Rutas que probablemente tocarás (stage solo lo que cambies):

- `Assets/_LRH/Scripts/Core/Mission.cs`
- `Assets/_LRH/Scripts/Core/Progress.cs`
- `Assets/_LRH/Scripts/Core/ProgressStore.cs`
- `Assets/_LRH/Scripts/Runtime/ProgressRepository.cs`
- `Assets/_LRH/Tests/EditMode/CoreTests.cs`
- `Tests/CoreChecks/Program.cs`

## Comprobación y mensajes de ejemplo

Ejecuta `dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj` y reporta el resultado real. Mensajes posibles, solo si describen el diff: `feat(progress): guardar desbloqueos de campaña` y `test(progress): cubrir restauración y datos inválidos`.

## Flujo Git de esta tarea (#2)

El issue depende de que la importación inicial #1 esté integrada. Después de ese merge, ejecuta desde la raíz del clon:

```bash
git switch master
git pull --ff-only origin master
git switch -c feature/mauro-progresion
git status --short
```

Haz el cambio y stagea solo las rutas que realmente modificaste. Ejemplo para el área:

```bash
git add -- Assets/_LRH/Scripts/Core/Mission.cs Assets/_LRH/Scripts/Core/Progress.cs Assets/_LRH/Scripts/Core/ProgressStore.cs Assets/_LRH/Scripts/Runtime/ProgressRepository.cs Assets/_LRH/Tests/EditMode/CoreTests.cs Tests/CoreChecks/Program.cs
git diff --cached --check
git diff --cached --stat
git diff --cached
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
```

Si el stage incluye archivos intactos, quítalos con `git restore --staged -- ruta`. Con las comprobaciones correctas y un diff propio:

```bash
git commit -m "feat(progress): guardar desbloqueos de campaña"
git push -u origin feature/mauro-progresion
gh pr create --base master --head feature/mauro-progresion --assignee MauroSullcaMamani --reviewer frix241 --title "Mejorar progreso de campaña" --body "Closes #2. Resume el cambio real y anota el resultado de las pruebas."
```

Adapta el mensaje al cambio efectivo. No abras el PR si no hay commits propios para revisar.
