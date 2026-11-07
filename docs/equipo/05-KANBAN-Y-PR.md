# Tablero, issues y revisión

## Flujo sugerido

Columnas: **Por hacer → En curso → Revisión → Aprobado**. Cada tarjeta tiene una persona responsable, resultado esperado y criterios verificables. Mantén una tarea por issue y enlaza el issue en el PR (`Closes #n` cuando corresponda).

Asignación de áreas sugerida:

| Responsable | Área |
|---|---|
| Mauro (`MauroSullcaMamani`) | Progresión, persistencia y reglas de misión |
| Sergio (`SrEstrada`) | Combate, navegación, colisiones y mapa |
| Christian (`ChristianAlexYana`) | Arte, UI visual, audio y su integración |
| Albert (`frix241`) | Integración, configuración, documentación y CI |

Las asignaciones deben reflejar el issue real y la disponibilidad del equipo. Un PR se asigna al autor real; un compañero distinto lo revisa. No hay que inventar PRs vacíos ni dividir retrospectivamente el estado actual en autorías que no constan en Git.

## Checklist de PR

- Issue vinculado y alcance claro.
- Archivos staged revisados; sin `Build`, `Library`, `Logs`, `UserSettings`, `bin` u `obj` en el diff.
- Resultado real de tests locales indicado; enlace al run de Actions cuando exista.
- Capturas para cambios visuales y pasos concretos para reproducir.
- Revisión aprobada por otra persona antes del merge.

Las tareas creadas son #1 (Albert, integración), #2 (Mauro, progreso), #3 (Sergio, navegación/combate) y #4 (Christian, arte/audio). #2–#4 arrancan después de integrar #1. Todavía no existen PR de esos cambios hasta que cada responsable publique commits propios; la importación inicial #1 sí tiene un flujo de PR detallado en su guía.
