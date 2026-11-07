# Kanban, PR y conservación de commits

[Tablero del equipo](https://github.com/users/frix241/projects/2)

## Tarjetas y estados

- **Por hacer:** tarea asignada con criterios y plan de pruebas.
- **En curso:** responsable trabajando en los bloques de su rama.
- **Revisión:** PR publicado, casos documentados y revisor solicitado.
- **Aprobado:** un compañero aprobó el resultado del último commit y las conversaciones están resueltas.

La tarjeta de un PR acompaña al issue. Los puntos están en el issue para no contabilizar dos veces la misma tarea. Estimaciones iniciales: Albert 3, Mauro 5, Sergio 8, Christian 8. Cada responsable confirma la estimación; los puntos no equivalen a un número obligatorio de commits.

## Revisores y dependencias

| Issue | Responsable | Revisor previsto | Condición para empezar |
|---|---|---|---|
| #1 | Albert | Sergio; Christian también tiene revisión solicitada | PR #5 ya publicado |
| #2 | Mauro | Albert | #1 integrado en master |
| #3 | Sergio | Mauro | #1 integrado en master |
| #4 | Christian | Sergio | #1 integrado en master |

Los cuatro integrantes pueden editar el tablero. Cada issue contiene las tareas técnicas y los casos de aceptación; sus guías desglosan el trabajo en commits.

## Cómo revisar la serie de commits

1. Abrir la pestaña **Commits** del PR y revisar el propósito de cada commit.
2. Contrastar sus archivos con el bloque indicado en la guía y con el caso que resuelve.
3. Revisar el resultado total en **Files changed**, incluidas referencias y metadatos de Unity.
4. Verificar que el último commit tenga Core checks (.NET 10) y Repository checks correctos.
5. Repetir los casos manuales/Unity correspondientes y revisar sus evidencias.
6. Solicitar cambios concretos o aprobar. Las correcciones nuevas se publican como commits adicionales.

## Integrar conservando el historial

El repositorio usa **Create a merge commit**. Esa operación conserva los commits originales de la rama y añade el commit de integración. Squash y rebase merge están deshabilitados para esta organización del equipo.

`master` exige una aprobación independiente, ambos checks y conversaciones resueltas. Está protegido contra force push y borrado. Las ramas deben actualizarse antes de integrar. Cuando el PR se acepta, `Closes #n` cierra el issue vinculado; revisa el estado de la tarjeta en el Project.

## Archivos compartidos

- `Tests/CoreChecks/Program.cs`: las comprobaciones del núcleo se coordinan entre Mauro y Sergio, y otros cambios solo si su tarea lo necesita.
- `Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs`: casos de integración compartidos por las áreas.
- `GameRoot.cs`: carga de progreso, actores, cámara e integración; acordar la sección que modifica cada tarea.
- `GameUi.cs`: objetivos y UI; coordinar lógica con Sergio y apariencia con Christian/Albert.
- Generadores de arte: revisar sus exportaciones; añadir solo los assets del bloque en curso.

Usa `git add -p` en texto compartido y prepara binarios por bloque. Un cambio y su prueba de regresión pueden estar en el mismo commit. Los logs, builds y ZIPs se distribuyen como evidencia o entrega y quedan fuera del Git fuente.
