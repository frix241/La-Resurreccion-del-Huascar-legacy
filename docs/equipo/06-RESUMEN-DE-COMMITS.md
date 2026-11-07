# Lista de commits por desarrollador

Este índice resume los bloques propuestos. Cada guía contiene las rutas de stage, pruebas y comandos de la rama. Cada commit se realiza después de un cambio real; las correcciones de revisión pueden agregar commits.

## Mauro Sullca Mamani

| Bloque | Resultado | Mensaje propuesto |
|---|---|---|
| 1 | Reglas de victoria y derrota | `fix(missions): resolver victoria y derrota de forma consistente` |
| 2 | Desbloqueo secuencial de campaña | `fix(progress): mantener desbloqueos en orden de campaña` |
| 3 | Lectura y serialización de progreso | `fix(progress): validar y restaurar el estado serializado` |
| 4 | Guardado y recuperación de archivos | `fix(save): conservar respaldo y recuperar progreso corrupto` |
| 5 | Integración del progreso con Unity | `fix(save): restaurar progreso al iniciar la campaña` |
| 6 | Registro de validación de campaña | `docs(progress): registrar validación de guardado y campaña` |

[Guía con archivos y comandos](01-MAURO.md).

## Sergio

| Bloque | Resultado | Mensaje propuesto |
|---|---|---|
| 1 | Contratos de daño y fuego entre facciones | `fix(combat): respetar daño y reglas entre facciones` |
| 2 | Contactos entre cascos | `fix(ships): impedir empujes indebidos entre cascos` |
| 3 | Límites y colisiones del mapa | `fix(map): respetar límites y obstáculos navegables` |
| 4 | Seguimiento de cámara y carta táctica | `fix(camera): seguir al Huascar dentro de los límites` |
| 5 | Actualización del objetivo cercano | `fix(objectives): actualizar el objetivo activo más cercano` |
| 6 | Evidencia de navegación y objetivos | `docs(gameplay): registrar pruebas de navegación y objetivos` |

[Guía con archivos y comandos](02-SERGIO.md).

## Christian Yana

| Bloque | Resultado | Mensaje propuesto |
|---|---|---|
| 1 | Silueta y acabado de la flota | `fix(art): mejorar lectura de barcos y baterías` |
| 2 | Terreno y objetos del mapa | `fix(art): unificar terreno y objetos del mapa` |
| 3 | Paneles, botones y tipografía | `fix(ui): unificar bordes y estados de los paneles` |
| 4 | Fondos de menú y resultados | `feat(art): ajustar fondos de menú y resultados` |
| 5 | Recursos de sonido y música | `feat(audio): ajustar música y efectos de campaña` |
| 6 | Eventos de audio y presentación Runtime | `fix(audio): sincronizar efectos y transiciones de campaña` |
| 7 | Registro de arte y validación | `docs(art): registrar recursos y validación audiovisual` |

[Guía con archivos y comandos](03-CHRISTIAN.md).

## Albert

| Bloque | Resultado | Mensaje propuesto |
|---|---|---|
| 1 | Checks del núcleo en Actions | `ci(core): ejecutar y publicar comprobaciones del núcleo` |
| 2 | Validación del árbol y recursos Unity | `ci(repo): validar archivos y metadatos de Unity` |
| 3 | Exclusiones y atributos de archivos | `chore(git): mantener exclusiones y atributos del proyecto` |
| 4 | Revisores y plantillas del equipo | `chore(github): definir revisión y plantillas del equipo` |
| 5 | Portada y documentación de uso | `docs(repo): explicar inicio y flujo de contribución` |
| 6 | Guías de commits y evidencia de integración | `docs(team): detallar commits y revisión por desarrollador` |

[Guía con archivos y comandos](04-ALBERT-INTEGRACION.md).
