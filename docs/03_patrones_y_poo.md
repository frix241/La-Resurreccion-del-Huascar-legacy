# Orientación a objetos, principios y patrones — LRH

**UNSA · Tecnología de Objetos · 2026-B.** Se distingue lo que puede mostrarse en archivos actuales de lo **verificado en ejecución**. Hay código de patrones en Runtime, pero Unity Editor no se ha ejecutado aquí; un patrón no está «demostrado en juego» solo porque aparece en un informe.

## Evidencia localizada en el núcleo C#

| Concepto | Archivo y elemento comprobable | Alcance / prueba |
|---|---|---|
| Encapsulación y contrato | `Assets/_LRH/Scripts/Core/Health.cs`: `Current { get; private set; }`, validación de `max` y daño; `Health : IDamageable`; `Died` una vez | T01–T04/T33 del ejecutable .NET; falta prueba de colisión Unity |
| Abstracción y polimorfismo | `Combat.cs`: `IAttackStrategy`, `ChaseStrategy`, `KeepDistanceStrategy`; `Mission.cs`: `IMissionObjective` y tres objetivos | T05–T07, T10–T14; en C# puro, no comportamiento visual |
| Composición | `Mission` recibe `IMissionObjective` por constructor | T10–T14; composición de nave + salud pendiente en Unity |
| Colecciones y genéricos | `Registry<T>` usa `Dictionary<string,T>` y `GameRoot` instancia `Registry<ShipActor>` para IDs activos; `EliminateManyObjective` usa `HashSet<string>` | T20 y T14 prueban núcleo; uso de actores Unity aún por verificar |
| Delegados y eventos | `Health.Changed`, `Health.Died`, `Mission.Finished` | T02 comprueba muerte única; suscriptores UI aún por verificar |
| Manejo de errores/persistencia | `Progress.TryFromJson` distingue JSON válido formateado de corrupto; `ProgressStore.Load/Save` lee/escribe JSON, preserva corruptos/backups | T03–T04, T17, T21–T24/T30 .NET; ruta `persistentDataPath` Unity pendiente |
| Volumen y UI | `Progress.SetVolume` guarda valor entre 0 y 1; `GameUi` tiene slider enlazado a `GameRoot.SetVolume` | T25–T26 .NET; slider y audio en Unity sin verificar |
| Concurrencia acotada | `CombatReporter.AnalyzeAsync` clona el array, usa `Task.Run`/`CancellationToken`, agrega daño y reconoce `TopSource` por cantidad de impactos | T18–T19/T27–T29 .NET; no hay garantía de integración Unity ni de ganancia de rendimiento |

## Patrones: términos precisos

| Patrón / familia | Evidencia de código actual | Qué falta para sostener la demostración académica |
|---|---|---|
| **Strategy** · comportamiento GoF | `IAttackStrategy.Decide` y dos estrategias intercambiables | Mostrar dos conductas en enemigos de Unity; T05–T07 solo demuestran decisiones puras |
| **Observer** · comportamiento GoF | Eventos `Changed`/`Died`/`Finished`; `GameUi` escucha salud de jugador, convoy y enemigos, y elimina listeners; `GameRoot` escucha fin de misión | Verificar HUD y ciclo de vida en Editor |
| **Factory Method** · creacional GoF | `ActorFactory.cs`: `EnemyFactory.Create` abstracto, redefinido por `PatrolFactory` y `GunboatFactory` | Clases presentes; demostrar instanciación/conducta en Unity |
| **Simple Factory** · factoría no-GoF | No es la clasificación del `EnemyFactory` existente | No atribuirle un `switch` por prefab inexistente |
| **Adapter** · estructural GoF | `InputAdapter.cs`: `IInputSource`, `ShipCommand`, `UnityInputAdapter` traduce Input System cuando existe `ENABLE_INPUT_SYSTEM`; en caso contrario usa legacy como fallback; `ShipActor.Spawn` permite inyectar fuente falsa | Existe `InjectedInput_MovesShipForward` PlayMode, **sin ejecutar**; activar Input System en Player Settings y probarlo en Editor |
| **MVC/MVP adaptado** · arquitectónico | Modelo `LRH.Core`, `GameRoot` orquesta, `GameUi` presenta menú, intro, HUD, resultados y créditos | Estructura codificada; observar el flujo real modelo → HUD/resultado en Play |
| **Object Pool** · reutilización | `ProjectilePool` almacena y recicla `Projectile` en `Queue<Projectile>` | Medir/reproducir impactos y reciclaje en PlayMode |
| **State GoF** | No acreditado: `MissionState` es enum, no patrón GoF State | No presentarlo como implementado sin objetos estado reales |

### Por qué la decisión de IA es polimórfica

`IAttackStrategy.Decide(distance, range)` acepta dos implementaciones sin que el consumidor necesite conocerlas. `ChaseStrategy` avanza fuera de rango; `KeepDistanceStrategy` puede retroceder al acercarse demasiado. Esta diferencia está en `Combat.cs` y se comprueba con T05–T07; afirmar que la **patrullera y la cañonera ya exhiben ese comportamiento en escena** requiere pruebas adicionales.

### Observer y responsabilidad sobre eventos

`Health.TakeDamage` emite `Changed` cuando cambia la vida y `Died` al agotarla; `Mission.Resolve` emite `Finished` solo al pasar a un estado terminal. `GameUi.ShowHud` y `GameUi.Clear` conectan/desconectan los cambios de salud de jugador, convoy y hostiles; comprobar en Editor que se reflejen sin duplicidades ni fugas. La presencia de eventos/suscriptores en archivos, por sí sola, no prueba el flujo visual.

## SOLID y GRASP con límites explícitos

| Principio | Aplicación localizada / decisión de diseño | Verificación pendiente |
|---|---|---|
| SRP | `Health` gestiona vida; `Mission` estado/objetivo; `ProgressStore` guarda archivos fuera del modelo `Progress` | Verificar IO y UI en Unity y vigilar cohesión de `GameRoot` |
| OCP y polimorfismo | `IAttackStrategy` y `IMissionObjective` aceptan implementaciones distintas sin cambiar el contrato consumidor | Añadir un tipo y probar que no se rompen los actuales |
| LSP | Cada objetivo expone `Complete`/`Failed` y reacciona a `Destroy/Arrive` | Comprobar en uso real que todos sustituyen al contrato sin supuestos adicionales |
| ISP | `IDamageable` expone solo `TakeDamage(int)`; contratos de IA y objetivo son pequeños y específicos | T33 prueba despacho a `Health`; verificar aplicación en impacto Unity |
| DIP | `Mission` recibe `IMissionObjective` en constructor, no crea una implementación fija | Evitar `Find...` global desde reglas del dominio |
| Information Expert (GRASP) | `Health` conoce su vida; cada objetivo conoce su criterio de cumplimiento | Revisar que UI no decida la victoria |
| Controller/Low Coupling (GRASP) | `Mission.Resolve` centraliza el resultado; `GameRoot` coordina pantallas y niveles sin separar escenas | Comprobar inicio/retorno y ausencia de referencias faltantes |
| Protected Variations (GRASP) | `IAttackStrategy` aísla variaciones de conducta | Revisión de integración al añadir cañonera |

**Herencia vs. composición:** estrategias y objetivos implementan interfaces; `EnemyFactory` usa herencia polimórfica real entre creadores; `ShipActor` compone `Health`, `WeaponClock` y una estrategia. No hay jerarquía `NavalUnit` del plan maestro: heredar de `MonoBehaviour` por sí solo no demuestra herencia de dominio. La batería comparte composición y no necesita heredar de una nave.

## Temas del sílabo y comparación sin equivalencias falsas

| Tema | Estado honesto en LRH |
|---|---|
| C# vs. C++/Qt/CMake | Stack seleccionado para proyecto, **aceptación docente pendiente**; Unity no emplea Qt ni CMake como build principal |
| Memoria y punteros inteligentes C++ | C# usa GC/referencias administradas y Unity administra objetos nativos; `Destroy()` no es `delete`, no se ha implementado `unique_ptr` |
| Genéricos y templates | `Registry<T>` muestra genéricos C#; no afirmar especialización o semántica idéntica a templates C++ |
| `friend`, `inline`, destructores | No hay equivalencia literal; `internal`, optimizaciones del runtime y ciclo de vida `OnDestroy` no son los mecanismos de C++ |
| Colecciones, excepciones, contratos | `Dictionary`, `HashSet`, `Queue<Projectile>`, precondiciones y errores; los 33 casos .NET cubren la parte independiente del Editor |
| Concurrencia y objetos paralelos | Cálculo de `CombatReporter` usa datos copiados, sin API Unity en el trabajador; integración, cancelación de escena y retorno al hilo principal por verificar |
| Sockets/servicios remotos | Fuera del alcance offline; no añadir solo para cubrir una casilla académica |

Para la defensa, seleccionar un ejemplo real, enseñar **la clase**, explicar **por qué resuelve el problema** y asociarlo con **una prueba o captura fechada**. Las capturas del Test Runner Unity y la ejecución visual siguen pendientes: [registro de pruebas](04_pruebas.md).
