# Plan, resultados y evidencias de pruebas — LRH

**UNSA · Tecnología de Objetos · 2026-B.** Registro basado en `Tests/CoreChecks/Program.cs`. El núcleo obtuvo **33/33 PASS (T01–T33)** con SDK 10.0.114. Unity Editor **6000.6.8f1** ejecutó **10/10 EditMode y 8/8 PlayMode**; la escena, controles, audio y presentación visual todavía requieren comprobación manual en el entorno objetivo.

## Cómo reproducir el núcleo

Con **.NET SDK 10**, en la raíz del proyecto:

```bash
dotnet run --project Tests/CoreChecks/CoreChecks.csproj
```

El ejecutable de consola compila `Assets/_LRH/Scripts/Core/*.cs` con `Tests/CoreChecks/CoreChecks.csproj`, recorre 33 casos, imprime `PASS`/`FAIL` por caso y devuelve código distinto de cero ante fallos. **No es Unity Test Framework ni NUnit**. Para la reproducción propia se copiaron `Program.cs`, `CoreChecks.csproj` y los `Core/*.cs` conservando su estructura relativa bajo `/tmp/opencode/lrh-doc-verification.PPwxaY/`, y se ejecutó allí el mismo comando, dejando `Tests/` original intacto. Registrar al repetir: fecha/hora, `dotnet --version`, sistema operativo, salida completa y commit/versión del código. No trasladar la cifra 33/33 a la columna de PlayMode/EditMode.

| Campo del registro actual | Valor |
|---|---|
| Comando / fuente | `dotnet run --project Tests/CoreChecks/CoreChecks.csproj` / `Program.cs` |
| Resultado reproducido personalmente | **33/33 PASS (T01–T33)**, copia aislada de los archivos actuales |
| Fecha y SDK de reproducción aislada | 2026-11-06 · .NET SDK **10.0.114** · Linux; máquina exacta **[pendiente]** |
| Log/captura archivada en el repositorio | **[pendiente]** (salida observada en la sesión, sin imagen ni archivo adjunto) |
| Compilación y ejecución en Unity Editor | **No verificadas** |
| Test Runner EditMode / PlayMode | **No ejecutado / sin captura** |

## Casos reales del comprobador .NET

Los identificadores **T01–T33** de esta tabla son los del `Program.cs` actual. No confundirlos con la numeración *propuesta* en el [plan maestro original](PLAN_MAESTRO_ORIGINAL.md) (después de T06 los contenidos no coinciden necesariamente).

| ID | Regla/comportamiento comprobado por `Program.cs` | Resultado comunicado | Alcance |
|---|---|---|---|
| T01 | Daño 25 a vida 100 deja 75 | PASS verificado | C# puro |
| T02 | Daño excesivo limita a cero y muerte ocurre una vez | PASS verificado | C# puro |
| T03 | Máximo de vida inválido arroja excepción | PASS verificado | C# puro |
| T04 | Daño negativo se rechaza sin cambiar vida | PASS verificado | C# puro |
| T05 | Patrullera decide avanzar fuera de alcance | PASS verificado | Estrategia pura |
| T06 | Patrullera decide disparar dentro de alcance | PASS verificado | Estrategia pura |
| T07 | Cañonera decide retroceder si está demasiado cerca | PASS verificado | Estrategia pura |
| T08 | Regla de facciones prohíbe fuego amigo y permite enemigo → aliado | PASS verificado | Política pura |
| T09 | Cadencia impide disparo durante cooldown | PASS verificado | Reloj de arma puro |
| T10 | Objetivo eliminado conduce a victoria terminal | PASS verificado | Estado de misión puro |
| T11 | Muerte del jugador prevalece en resolución simultánea | PASS verificado | Estado de misión puro |
| T12 | Convoy llega vivo: victoria de escolta | PASS verificado | Estado de misión puro |
| T13 | Convoy destruido: derrota aunque haya llegado | PASS verificado | Estado de misión puro |
| T14 | Se requieren todas las baterías para victoria | PASS verificado | Estado de misión puro |
| T15 | Progreso desbloquea misiones en secuencia | PASS verificado | Progreso en memoria |
| T16 | Progreso sobrevive serialización/deserialización en memoria | PASS verificado | Cadena JSON |
| T17 | JSON inválido usa progreso inicial seguro | PASS verificado | Parser en memoria |
| T18 | Reporte asíncrono trabaja sobre copia aunque se modifique origen | PASS verificado | Tarea .NET, no vista Unity |
| T19 | Reporte con token cancelado produce cancelación | PASS verificado | Tarea .NET |
| T20 | Registro genérico no reemplaza ID duplicado | PASS verificado | Colección C# |
| T21 | Ausencia de archivo da progreso nuevo | PASS verificado | Archivo temporal .NET |
| T22 | JSON corrupto conserva copia `.corrupt` y vuelve al inicio | PASS verificado | Archivo temporal .NET |
| T23 | Guardar y recrear repositorio conserva desbloqueo | PASS verificado | Archivo temporal .NET |
| T24 | Sobrescribir conserva respaldo `.bak` | PASS verificado | Archivo temporal .NET |
| T25 | Volumen configurado persiste entre serializaciones | PASS verificado | Modelo `Progress` |
| T26 | Volumen fuera de rango se rechaza sin variar el valor | PASS verificado | Modelo `Progress` |
| T27 | Informe identifica fuente con más impactos (`TopSource`) | PASS verificado | Tarea .NET; no vista Unity |
| T28 | Evento de combate con daño negativo es rechazado | PASS verificado | Contrato de snapshot C# |
| T29 | Snapshot por defecto no falla ni inventa fuente | PASS verificado | Tarea .NET |
| T30 | JSON válido con espacios conserva desbloqueo sin crear `.corrupt` | PASS verificado | Archivo temporal .NET |
| T31 | Pérdida de convoy registra `FailureReason.ObjectiveDestroyed` | PASS verificado | Estado misión puro |
| T32 | Muerte simultánea de convoy y jugador prioriza `FailureReason.PlayerDestroyed` | PASS verificado | Estado misión puro |
| T33 | `IDamageable` despacha `TakeDamage(int)` a `Health` | PASS verificado | Contrato C# puro, no colisión Unity |

**Interpretación limitada:** T12–T14/T31–T32 prueban reglas de objetivos y causa de derrota, **no** que las misiones 2 y 3 funcionen en Unity. T33 verifica `IDamageable` en `Health`, **no** la colisión ni `ShipActor.Receive` en Play. T21–T24/T30 prueban archivos en directorio temporal de .NET, **no** la ruta ni permisos en Unity; T25–T26 no prueban slider ni volumen audible. T18–T19/T27–T29 no prueban desuscripción/cancelación ni informe visible en una escena.

## Matriz de validación Unity que falta ejecutar

Abrir con Unity Hub usando **6000.3.0f1** y seleccionar `Assets/_LRH/Scenes/Main.unity`; `GameRoot` construye menú y tres niveles al correr. En caso de problemas anotar error exacto y reproducción. **Existen** `Assets/_LRH/Tests/EditMode/CoreTests.cs` y `Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs` (tres pruebas: impacto único, derrota e `IInputSource` falso que mueve nave); ejecutar **Window > General > Test Runner** (ubicación de menú según Editor), por separado EditMode y PlayMode, antes de anotar resultados. **No se han ejecutado aquí**. La victoria está cubierta por el núcleo, pero el flujo visual se comprueba **manualmente**.

| ID de verificación propuesta | Procedimiento / criterio observable | Estado | Fecha / entorno / evidencia |
|---|---|---|---|
| U-01 | Importar proyecto sin errores ni `Missing Script`; seleccionar **Input System Package (New)** o **Both**, aceptar reinicio y comprobar que se generan `ProjectSettings/ProjectSettings.asset`/`Packages/packages-lock.json` | No ejecutado | [pendiente] |
| U-02 | Menú → introducción narrativa → misión 1; créditos vuelven al menú | No ejecutado | [pendiente] |
| U-03 | Con `ENABLE_INPUT_SYSTEM` activo: W/S y A/D mueven/giran, mouse apunta, clic dispara y Escape pausa; fallback legacy no demuestra Input System | No ejecutado | [pendiente] |
| U-04 | Disparo tiene cadencia, proyectil avanza e impacta exactamente una vez | No ejecutado | [pendiente] |
| U-05 | HUD actualiza vida del jugador/convoy/enemigos y objetivo; **victoria manual**, derrota (causa correcta) y reintento | No ejecutado | [pendiente] |
| U-06 | Patrullera y cañonera se diferencian en juego | No ejecutado | [pendiente] |
| U-07 | Convoy llega vivo / muere y cambia resultado de misión 2 | No ejecutado | [pendiente] |
| U-08 | Neutralizar todas las baterías completa misión 3 | No ejecutado | [pendiente] |
| U-09 | Progreso y volumen se guardan, cierra/reabre y recupera; JSON corrupto no bloquea | No ejecutado | [pendiente] |
| U-10 | Pausar o salir con informe asíncrono no toca UI destruida | No ejecutado | [pendiente] |
| U-11 | Suites presentes en Unity Test Runner: EditMode y **3 casos PlayMode** (input falso, impacto, derrota) | **PASS: 10 EditMode + 8 PlayMode** | Unity CLI, Editor 6000.6.8f1; `Build/Artifacts/*-results.xml` |
| U-12 | Build en PC objetivo: arrancar, completar misiones y registrar versión | **BUILD PASS**; ejecución manual del jugador pendiente | Linux Standalone; `Build/Artifacts/LRH.x86_64`; log `Build/Artifacts/unity-build.log` |
| U-13 | Escuchar disparo/impacto `SoundFx`, variar slider, cerrar/reabrir y comprobar volumen; verificar narrativas y créditos con identidades reales | No ejecutado | [pendiente] |
| U-14 | Confirmar preview de cada misión con carta estilizada del litoral peruano y minimapa persistente con marcadores de jugador, convoy y hostiles | Implementado y compilado; pendiente de comprobación visual manual | [pendiente de captura/entorno objetivo] |
| U-15 | Confirmar que los siete SVG y PNG rediseñados conservan rutas, dimensiones y carga por `Visuals.Sprite` | SVG rasterizados con dimensiones originales; compilación pendiente de repetir | [pendiente de build tras arte] |

Para cada fila ejecutada, consignar **resultado real** (PASS, FAIL o bloqueo), fecha, versión/editor, máquina, rama/commit y enlace a archivo de evidencia creado por el equipo. Un fallo **no** debe esconderse: abrir issue, registrar reproducción y repetir la prueba tras corregirlo.

## Plantilla de bitácora por ejecución

| Fecha y hora | Entorno / versión | Caso | Resultado real | Error o evidencia real | Responsable |
|---|---|---|---|---|---|
| [pendiente] | [pendiente] | [U-XX o TXX] | [PASS/FAIL/BLOQUEADO/NO EJECUTADO] | [ruta, log o issue real] | [persona real] |

## Capturas y trazabilidad mínima

Guardar imágenes solo **después** de la prueba, por ejemplo en `docs/evidencias/` si el equipo decide crear esa carpeta. Capturar menú, combate, HUD, victoria, derrota, código POO con ruta visible, Test Runner **real** y actividad GitHub **real**. Escribir pie con fecha de captura, versión, autor y contexto; no usar imágenes de referencia ni fingir un Test Runner a partir de `CoreChecks`. Los espacios preparados se encuentran en [avance del 50 %](05_avance_50.md) e [informe final](06_avance_final.md).

## Alcance visual naval incorporado

La interfaz runtime usa una dirección 2D naval atmosférica: azul noche/petróleo, dorado envejecido, crema, rojo señal, veladuras, marcos ornamentales y contraste de siluetas. `CartographyVisuals` genera en memoria una carta original con oleaje, costa, rutas, cuadrícula y rosa de los vientos, sin descargar ni incorporar material externo. La pantalla de preview presenta la misión y su carta antes de iniciar; el HUD conserva una carta táctica/minimapa durante todo el combate. Los marcadores se actualizan leyendo las posiciones de actores, incluyen un halo pulsante de lectura y se eliminan al destruirse el actor, sin alterar las reglas de combate, el progreso, la IA ni el adaptador de controles.

La iteración visual más reciente añadió `OceanVisuals`, un fondo de mar procedural con variación de profundidad, bandas de onda, espuma tenue, bruma y reflejos; además, `ShipActor` y `GameRoot.Island` crean sombras, estelas y espuma como capas decorativas separadas. Estas capas no tienen lógica de combate ni colisionadores y deben validarse manualmente en una ejecución de la escena para comprobar el acabado en la resolución objetivo.

La pasada 2D moderna reemplazó los PNG planos por raster RGBA de alta resolución: naves `512x768`, batería `512x512`, isla `1024x736` y proyectil `256x256`. Se conserva la escala mundial mediante `Visuals.Sprite` y su tabla de `pixelsPerUnit`; la comprobación objetiva es que las rutas/IDs cargan y las dimensiones físicas no cambian. La UI usa estados `normal`, `highlighted`, `pressed` y `disabled` en sus botones para evitar que el acabado dependa solo de marcos decorativos.

La dirección global también centraliza los límites del mapa (`WorldMinX/MaxX = -12.8/12.8`, `WorldMinY/MaxY = -6.7/6.7`) y ajusta la cámara ortográfica a `7.6` para conservar margen visual alrededor de la arena. `OceanVisuals` genera un fondo 1024×576 con relación 16:9 suficiente para cubrir la cámara; `UiVisuals` aporta fondo con viñeta y superficies redondeadas compartidas por menú, preview, pausa, resultado y HUD. La investigación y sus referencias primarias están en `docs/research_visual_direction.md`.

En esta iteración se integraron los paquetes suministrados por el usuario: `Assets/_LRH/Resources/Sprites/` contiene los siete PNG navales con los nombres runtime conservados; `Assets/_LRH/Resources/UI/` contiene fondo, logo, panel, marco, mapa táctico, HUD y estados de botones. `GameUi` usa rutas `Resources/UI/...` reales y asigna `SpriteState` a los `Button`; los paneles se crean con `Sprite.Create` y bordes para 9-slice. La evidencia de integración está en `GameUi.cs`, `UiVisuals.cs` y la build `Build/Artifacts/LRH-supplied-assets.x86_64`.
