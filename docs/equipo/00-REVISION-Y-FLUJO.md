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

No se han creado commits en nombre de Mauro, Sergio o Christian. El remoto todavía parte de un commit de inicialización vacío, así que #1 describe la importación inicial que Albert debe publicar en un PR auténtico. Los issues #2–#4 esperan ese merge; sus guías contienen los comandos de rama, staging, commit y apertura de PR.

La Action compila y ejecuta `Tests/CoreChecks` en .NET 10 al abrir un PR o actualizar `master`/`develop`. El workflow está en el estado fuente; Actions mostrará un run cuando la rama de integración se publique y abra el PR de #1. El último chequeo local documentado fue 33/33; PlayMode Unity fue 23/23 en el entorno de desarrollo.
