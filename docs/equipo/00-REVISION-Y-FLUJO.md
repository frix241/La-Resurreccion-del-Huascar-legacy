# Revisión del proyecto

Este paquete contiene el estado fuente de Huasca Rebirth, las comprobaciones y una build Linux para revisión visual. El repositorio tiene el tablero público [Huasca Rebirth · Desarrollo](https://github.com/users/frix241/projects/2) en vista Kanban, enlazado al repo. Las tareas están registradas como issues #1–#4 y asignadas a sus responsables.

## Qué contiene

- Proyecto Unity: `Assets`, `Packages` y `ProjectSettings`.
- Chequeos .NET: `Tests/CoreChecks`.
- Workflow de GitHub Actions: `.github/workflows/ci.yml`.
- Documentación y herramientas de producción.
- Solo la build Linux seleccionada (`Build/Artifacts/LRH-terrain*`) y capturas.

No contiene `.git`, `Library`, `Logs`, `UserSettings`, archivos `bin/obj`, builds anteriores ni dumps de crash. La build dentro del ZIP sirve para jugar/revisar; no se propone versionarla en Git.

## Punto de partida de Git

La importación inicial está publicada en el [PR #5](https://github.com/frix241/La-Resurreccion-del-Huascar/pull/5), asignado a Albert y con revisión solicitada a Sergio y Christian. Los issues #2–#4 parten de `master` después del merge de ese PR. Las ramas y números de issue de las guías enviadas se mantienen.

Actions ejecutó correctamente CoreChecks (33/33) y Repository checks en el primer snapshot. Cada actualización del PR tiene su propio run. El resultado actual y las reglas están en [REPOSITORIO.md](../REPOSITORIO.md). Unity EditMode/PlayMode se valida en el Editor y no está cubierto por la Action .NET.
