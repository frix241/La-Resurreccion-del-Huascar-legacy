# Contribuir al juego

Empieza por el issue asignado y su guía en [docs/equipo](docs/equipo/00-REVISION-Y-FLUJO.md). Las ramas #2–#4 parten de `master` tras aprobar e integrar la importación #1.

1. Actualiza `master` con `git pull --ff-only origin master` y crea la rama indicada.
2. Limita el cambio a los criterios del issue y conserva los `.meta` de Unity.
3. Stagea rutas explícitas; revisa `git diff --cached --check` y `git diff --cached`.
4. Abre PR a `master`, enlaza el issue con `Closes #n`, asígnalo al autor y solicita otro revisor.
5. Espera los checks y una aprobación. Solo se integra un cambio con las conversaciones resueltas.

## Comprobaciones

```bash
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
python Tools/check_repository.py
```

`CoreChecks` es un ejecutable de comprobaciones C# del núcleo; su salida y código de retorno determinan el resultado en CI. No sustituye al Test Runner de Unity. Para comportamiento Unity, abre Unity 6000.6.8f1 y ejecuta los casos EditMode/PlayMode aplicables; para UI añade capturas a 1920×1080. Registra los casos no ejecutados.

Cada issue incluye tareas, puntos estimados y plan de pruebas. Adjunta enlace al run de Actions, commit probado y resultados antes de moverlo a Revisión. Aprobado requiere revisión de otra persona; cerrar el issue corresponde al merge aceptado.

No versionar `Build`, `Library`, `Temp`, `Logs`, `UserSettings`, `bin`, `obj`, ZIPs ni dumps. Distribuye la build final por Releases o el paquete de revisión. Los recursos externos deben conservar atribución y licencia aplicables.

## Commits por tarea

Cada guía en `docs/equipo` divide la tarea en bloques con archivos y mensajes explícitos. Registra el cambio y su regresión antes de pasar al siguiente. El PR conserva la serie; se integra mediante **Create a merge commit**. Consulta [el índice de commits](docs/equipo/06-RESUMEN-DE-COMMITS.md).
