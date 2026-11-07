# Flujo del equipo y guías de commits

El [Project Kanban](https://github.com/users/frix241/projects/2) conserva las cuatro tareas asignadas. El código inicial está en el [PR #5](https://github.com/frix241/La-Resurreccion-del-Huascar/pull/5), pendiente de revisión independiente e integración en `master`.

## Una tarea, una rama, varios commits

Cada desarrollador trabaja el issue de su área en la rama indicada. Dentro de esa rama registra un commit por bloque coherente. Al terminar, un PR reúne la serie de commits y enlaza el issue. La integración mediante **Create a merge commit** conserva esa serie en `master`.

| Persona | Issue | Rama | Bloques previstos | Guía |
|---|---|---|---:|---|
| Mauro | #2 | `feature/mauro-progresion` | 6 | [Abrir guía](01-MAURO.md) |
| Sergio | #3 | `feature/sergio-combate-mapa` | 6 | [Abrir guía](02-SERGIO.md) |
| Christian | #4 | `feature/christian-arte-audio` | 7 | [Abrir guía](03-CHRISTIAN.md) |
| Albert | #1 / PR #5 | `chore/project-baseline` | 6 | [Abrir guía](04-ALBERT-INTEGRACION.md) |

Los bloques proponen una separación de cambios reales. Se puede omitir un bloque que ya esté correcto y dejar su comprobación en el issue. No se reconstruye retrospectivamente la autoría del snapshot inicial. La cantidad final de commits depende del diff; una corrección de revisión puede agregar otro commit.

## Orden de trabajo

1. Revisar el PR #5 y completar la aprobación independiente de la base.
2. Tras el merge, Mauro, Sergio y Christian actualizan `master` y crean su rama una sola vez.
3. Realizar el primer bloque de la guía, probarlo, seleccionar sus archivos y registrarlo.
4. Repetir con cada bloque siguiente. Los archivos de pruebas y Runtime compartidos se coordinan y se stagean por fragmentos cuando corresponda.
5. Revisar la serie con `git log --oneline --reverse origin/master..HEAD`.
6. Publicar, abrir el PR vinculado al issue y adjuntar evidencia del último commit.
7. Obtener checks correctos y aprobación de un compañero; integrar conservando los commits.

## Qué tiene cada guía

- Preparación del clon, identidad, rama nueva y rama ya existente.
- Tabla de commits y mensaje propuesto para cada bloque.
- Resultado esperado, trabajo previo, pruebas y rutas explícitas de stage.
- `git add`, `git add -p`, `git commit -m`, inspección y push.
- Actualización de la rama, archivos compartidos y tratamiento de conflictos.
- Apertura o actualización del PR, responsable y revisor.
- Evidencia del issue, checks de CI y requisitos de integración.

## Comprobaciones y estado

CI ejecuta CoreChecks (.NET 10) y Repository checks. El último run del PR determina el estado del commit revisado. La validación Unity y las capturas a 1920×1080 se realizan con Unity 6000.6.8f1 y se registran aparte. [Reglas y evidencia del repo](../REPOSITORIO.md).

[Tablero y revisión](05-KANBAN-Y-PR.md) · [Lista de mensajes de commit](06-RESUMEN-DE-COMMITS.md).
