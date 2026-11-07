# Requisitos, alcance y trazabilidad — LRH

**UNSA · Tecnología de Objetos (1703240) · 2026-B.** Este documento adapta el [plan maestro original](PLAN_MAESTRO_ORIGINAL.md) (especificación, no evidencia) al **árbol actual de archivos**. «Presente en código» significa que existe la implementación C#/Unity; **no** significa que funcione en una escena Unity sin una ejecución de Editor. Los 33 casos .NET se reprodujeron anteriormente en una copia aislada del núcleo; además, el usuario comunicó **33/33 passed** sobre el árbol actual (Linux, SDK 10.0.114, 2026-11-06; sin log archivado), registrado por separado en [pruebas](04_pruebas.md). Los criterios del avance del 50 % son una **rúbrica interna propuesta**, su aceptación corresponde a la docente.

## Problema, objetivo y límites

Se propone un videojuego naval 2D para practicar orientación a objetos con reglas comprobables y presentación separada. El jugador controlará el monitor Huáscar en una historia alternativa ficticia posterior a la Guerra del Pacífico. Objetivo académico: diseñar y verificar entidades, estrategias, objetivos, eventos, interfaz y persistencia sin confundir código escrito con producto jugable.

- **Plataforma prevista:** PC, un jugador, sin red. **No** se incluyen multijugador, combate terrestre, simulación histórica exacta, servicios remotos ni 3D.
- **Primer corte propuesto:** menú → misión 1 → mover, apuntar y disparar → combate → victoria/derrota → retorno/reintento. Todavía requiere verificación en Unity.
- **Final propuesto:** tres misiones, segundo enemigo, convoy, artillería costera, progreso persistente, UI, pruebas en Editor y build reproducible. El código para estas funciones está presente en distintos grados; no declarar el 100 % antes de verificarlo en Unity.
- **Decisiones por confirmar:** autorización docente para Unity/C# (el sílabo enfatiza C++/Qt), rúbrica y fechas oficiales; nombres de los cuatro integrantes.

## Campaña y criterios de aceptación

| ID | Objetivo | Victoria propuesta | Derrota propuesta | Base disponible / pendiente |
|---|---|---|---|---|
| M1 · Ecos del Pacífico | Interceptar patrullera | El objetivo `EliminateObjective` queda completo | Muere el Huáscar | `Mission` y `GameRoot.StartMission(1)` presentes; combate sin verificar en Editor |
| M2 · Convoy bajo fuego | Escoltar convoy | Llega vivo al destino | Muere convoy o Huáscar | `EscortObjective` y `GameRoot.StartMission(2)` presentes; sin verificar en Editor |
| M3 · Silencio en la costa | Destruir baterías | Se eliminan todos los objetivos | Muere el Huáscar | `EliminateManyObjective` y `GameRoot.StartMission(3)` presentes; sin verificar en Editor |

**Regla transversal del núcleo:** al resolver una misión, la derrota tiene prioridad sobre una victoria simultánea; una resolución terminal posterior no debe cambiar su resultado. La prueba de este comportamiento es exclusivamente .NET por ahora.

## Requisitos funcionales (RF)

Leyenda: **núcleo verificado** = regla C# ejercitada en `CoreChecks`; **Runtime presente/no verificado** = código Unity escrito, sin ejecución en Editor; **pendiente** = criterio aún sin evidencia. Los tres niveles se crean por código en `GameRoot` sobre `Main.unity`, no corresponden a tres escenas versionadas. El balance se carga desde `Assets/_LRH/Resources/Configs/*.json` con `ShipConfig.Load`, **no** desde ScriptableObjects. Ver [pruebas](04_pruebas.md).

| ID | Requisito / criterio observable | Estado documentable hoy | Evidencia / condición de cierre |
|---|---|---|---|
| RF-01 | Menú principal inicia juego y permite salir | `GameUi.ShowMenu`/`GameRoot` presentes; no verificados en Unity | Menú ejecutado y capturado en Editor/build |
| RF-02 | W/S mueven y A/D giran al Huáscar | `UnityInputAdapter`/`ShipActor` presentes, `IInputSource` inyectable; test PlayMode de input falso escrito, sin ejecutar | Prueba manual y PlayMode |
| RF-03 | Mouse apunta, clic izquierdo dispara con cadencia | `WeaponClock` verificado; `ShipActor`/input sin ejecutar | T09 (.NET) + prueba de arma en Unity |
| RF-04 | Proyectiles avanzan, caducan y portan facción/daño | `ProjectilePool` y `Projectile` escritos; física no verificada | Colisión e impacto observados en PlayMode |
| RF-05 | Daño, vida limitada, muerte única; colisión física | `Health : IDamageable`/`DamageRules` verificados; `ShipActor.Receive` invoca el contrato en código, collider Runtime sin ejecutar | T01–T04, T08, T33 (.NET) + PlayMode |
| RF-06 | Patrullera persigue y dispara | `ChaseStrategy` verificada; `PatrolFactory`/`ShipActor` presentes | T05–T06 (.NET) + demo de patrullera |
| RF-07 | Misión 1 se completa al interceptar enemigo | `EliminateObjective`/`Mission` verificados; test PlayMode de victoria escrito, sin ejecutar | T10–T11 (.NET) + recorrido completo |
| RF-08 | HUD muestra vida y objetivo actuales | `GameUi.ShowHud` enlaza eventos en código; sin ejecutar | Captura de HUD actualizado por daño |
| RF-09 | Victoria, derrota y reintento/menú | `Mission` verificado; tests PlayMode de victoria, derrota mediante `Receive`, reintento y retorno/ reinicio escritos, sin ejecutar | T10–T11 (.NET) + flujos de pantalla |
| RF-10 | Cañonera con estrategia diferenciada | `KeepDistanceStrategy` verificada; `GunboatFactory` presente | T07 (.NET) + comparación en escena |
| RF-11 | Convoy protegido en misión 2 | `EscortObjective` verificado; convoy se monta por `GameRoot` | T12–T13 (.NET) + ruta/combate en Unity |
| RF-12 | Baterías neutralizadas en misión 3 | `EliminateManyObjective` verificado; baterías creadas por `GameRoot` | T14 (.NET) + misión completa en Unity |
| RF-13 | Desbloqueo secuencial 1 → 2 → 3 | `Progress` verificado; PlayMode de desbloqueo de misión 2 y misión bloqueada escritos, sin ejecutar | T15–T16 (.NET) + reinicio real de juego |
| RF-14 | Progreso persistente entre ejecuciones | `ProgressStore` guarda/recupera archivo temporal en T21–T24/T30; adaptador Unity presente | T16–T17/T21–T24/T30 (.NET) + archivo real en Editor |
| RF-15 | Escape pausa/reanuda, regreso al menú | `GameRoot`/`GameUi` y PlayMode de pausa/reanudar y retorno al menú escritos; sin ejecutar | Prueba manual de pausa y navegación |
| RF-16 | Indicadores visuales de vida enemiga | `GameUi.ShowHud` observa `Health.Changed` de cada hostil; sin ejecutar | Captura con cambios tras daño y desuscripción al salir |
| RF-17 | Introducción y cierre narrativo por misión, créditos y aviso de ficción | `GameUi.ShowIntro`, `ShowResult`, `ShowCredits` y aviso presentes; pantalla de créditos aún tiene DEV-1…DEV-4 | Inspección visual y sustitución por nombres reales antes de presentar |
| RF-18 | Reporte de combate asíncrono seguro y fuente con más impactos | `CombatReporter` verificado en .NET; `GameRoot.ShowReportAsync` presente | T18–T19/T27–T29 (.NET) + ciclo de vida en Unity |
| RF-19 | Eventos de vida/misión sin acoplar HUD al modelo | Eventos verificados en núcleo; `GameUi` suscribe/desuscribe en código; `Registry<ShipActor>` controla IDs activos | Inspección y prueba de enlace a UI e idempotencia |
| RF-20 | Creación de tipos de enemigos centralizada | `EnemyFactory` abstracta, `PatrolFactory`/`GunboatFactory` presentes | Instanciación real en Unity; clasificar patrón GoF por código |
| RF-21 | Volumen ajustable y persistente | `Progress.SetVolume` y T25–T26 verificados; slider `GameUi`, `AudioListener` y `SoundFx` procedimental presentes | Cambio audible/valor persistido en Editor por verificar |

## Requisitos no funcionales (RNF)

| ID | Meta propuesta | Estado / evidencia necesaria |
|---|---|---|
| RNF-01 | Mantenibilidad y separación de reglas y Unity | Núcleo `LRH.Core` independiente; revisar integración real |
| RNF-02 | Extender IA sin duplicar daño | Contrato `IAttackStrategy`; comprobar en escena con dos enemigos |
| RNF-03 | Reglas críticas comprobables sin Editor | 33/33 .NET en reproducción aislada anterior y ejecución actual comunicada; EditMode y 8 UnityTest PlayMode escritos, no ejecutados |
| RNF-04 | 60 FPS de referencia | Sin perfilado ni equipo consignado; no medido |
| RNF-05 | Proyecto Unity reproducible | Versión `6000.3.0f1` fijada; clon/importación pendientes |
| RNF-06 | HUD legible y controles claros | Mapeo documentado en README; usabilidad no probada |
| RNF-07 | Guardado tolerante a datos ausentes/corruptos y JSON formateado | T21–T24/T30 comprueban IO en directorio temporal .NET; `Application.persistentDataPath` no comprobado |
| RNF-08 | Robustez de escenas y referencias | No evaluada sin Editor |
| RNF-09 | Requisito ↔ clase ↔ prueba ↔ evidencia GitHub | Matriz inicial aquí; issues/PR reales pendientes |
| RNF-10 | No versionar archivos regenerados y conservar `.meta` | Revisar al importar assets en Unity y crear repositorio |

`MissionPlayTests.cs` contiene ocho casos: movimiento con input falso, impacto único, derrota mediante `Receive`, victoria con guardado en memoria y desbloqueo de misión 2, pausa/reanudar, reintento tras derrota, retorno al menú desde pausa y reinicio, y misión bloqueada. Inyectan `MemoryRepository` con `GameRoot.ConfigureRepository` antes de `Awake` en un `GameObject` inactivo, sin escribir progreso real; `UnitySetUp`/`UnityTearDown` limpian entre casos. No se acredita compilación ni ejecución Unity; la validación visual/manual y la persistencia real en Unity siguen pendientes.

## Historias de usuario y seguimiento

| HU | Como… quiero… | RF | Prueba mínima a documentar |
|---|---|---|---|
| HU-01 | Jugador, navegar para posicionarme | RF-02 | Movimiento/rotación en Play |
| HU-02 | Jugador, apuntar y disparar contra enemigos | RF-03–05 | T01–T04, T08–T09 + impacto Unity |
| HU-03 | Jugador, ver vida y objetivo | RF-08, RF-19 | Daño actualiza HUD |
| HU-04 | Jugador, conocer resultado y reintentar | RF-07, RF-09 | T10–T11 + navegación UI |
| HU-05 | Jugador, escoltar un convoy vulnerable | RF-11 | T12–T13 + escena |
| HU-06 | Jugador, neutralizar baterías costeras | RF-12 | T14 + escena |
| HU-07 | Jugador, conservar desbloqueo y ajustar volumen | RF-13–14, RF-21 | T15–T17/T21–T26/T30 + reinicio de aplicación Unity |
| HU-08 | Docente, identificar POO y pruebas | RF-18–20, RNF-03/09 | Código, diagramas, tests y PR reales |

**Cierre por evidencia, no por título:** un requisito no pasa a «probado» solo por estar en un script, un informe o una carpeta. Anotar fecha, entorno y enlace de prueba al actualizar cada fila.
