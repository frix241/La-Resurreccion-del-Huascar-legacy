# Repositorio y revisión

[Project Kanban](https://github.com/users/frix241/projects/2) · [PR inicial #5](https://github.com/frix241/La-Resurreccion-del-Huascar/pull/5)

## Reglas de master

- PR con una aprobación independiente del autor y del último push.
- Checks obligatorios: `Core checks (.NET 10)` y `Repository checks`.
- La rama debe estar actualizada; conversaciones resueltas antes de integrar.
- Se descartan aprobaciones anteriores cuando cambia el código.
- Sin force push ni borrado de master; protección también para administradores.
- Merge commit para conservar los commits del desarrollador; squash y rebase merge deshabilitados. Eliminación de la rama integrada.

## Tablero

Por hacer → En curso → Revisión → Aprobado. Cada issue contiene una historia, tareas técnicas y casos verificables. Estimaciones iniciales, a confirmar por el responsable:

| Issue | Responsable | Puntos | Prioridad | Revisor previsto |
|---|---|---:|---|---|
| #1 Integración y CI | Albert | 3 | P0 | Sergio |
| #2 Campaña y persistencia | Mauro | 5 | P1 | Albert |
| #3 Navegación y combate | Sergio | 8 | P1 | Mauro |
| #4 Arte y audio | Christian | 8 | P1 | Sergio |

El PR se incluye como tarjeta sin sumar puntos otra vez. Los cuatro desarrolladores pueden editar el tablero. Las vistas incluyen Kanban, Backlog, Mis tareas y En revisión. Arrastra la tarjeta al estado que corresponda; Aprobado exige revisión real, no solo checks verdes. #2–#4 dependen de integrar #1.

## Evidencia

[Primera ejecución de Actions](https://github.com/frix241/La-Resurreccion-del-Huascar/actions/runs/42403048010): 33/33 CoreChecks y validación del repositorio correctos. El log es un artefacto descargable con retención de 14 días. Los runs posteriores son la evidencia del commit que se esté revisando.

Repository checks comprueba archivos generados versionados por error, JSON inválido, sintaxis Python y marcadores de conflictos sin resolver. CoreChecks valida reglas del núcleo C#. Las pruebas Unity y revisión visual se ejecutan en el Editor 6000.6.8f1 y se documentan por issue. No hay credenciales de licencia Unity configuradas para ejecutarlas en Actions.

Las plantillas de historia y PR piden criterios y evidencia. CODEOWNERS propone revisores por ruta. Dependabot propone actualizaciones mensuales de Actions tras integrar el archivo en la rama predeterminada.
