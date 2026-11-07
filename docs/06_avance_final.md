# Informe académico — entrega final propuesta (100 %)

> **Borrador pendiente de ejecución y evidencias.** «100 %» designa la **meta del plan maestro**, no un resultado obtenido. En el entorno de preparación no hay Unity Editor. El núcleo C# obtuvo **33/33 PASS** en copia aislada (incluidos `IDamageable`, archivos temporales, JSON formateado, volumen y causas de derrota); hay scripts Runtime y tests Unity escritos, pero **no están verificados** los tres niveles en Editor, el Test Runner, el guardado mediante Unity, una build ni capturas reales. Conservar esta advertencia hasta disponer de evidencia.

**Universidad Nacional de San Agustín de Arequipa (UNSA)**  
**Facultad de Ingeniería de Producción y Servicios · Escuela Profesional de Ingeniería de Sistemas**  
**Tecnología de Objetos (1703240) · Periodo académico 2026-B**  
**Proyecto:** *La Resurrección del Huáscar: Una Historia Alternativa Después de la Guerra del Pacífico*  
**Documento:** Informe de entrega final — meta del 100 %  
**Sección y docente:** [pendiente de confirmar] · **Fecha oficial:** [pendiente]  
**Tag/versión comprobada:** [pendiente; no afirmar `v1.0.0` sin release]  
**Repositorio y build verificables:** [pendiente]  
**Arequipa, Perú.**

| Identificador provisional | Nombre y código reales | Usuario GitHub / contribución comprobada |
|---|---|---|
| DEV-1 | [pendiente] | [pendiente] |
| DEV-2 | [pendiente] | [pendiente] |
| DEV-3 | [pendiente] | [pendiente] |
| DEV-4 | [pendiente] | [pendiente] |

## 1. Resumen

LRH plantea una campaña naval 2D de tres misiones para mostrar diseño orientado a objetos, separación entre dominio y presentación Unity, patrones, persistencia y pruebas. El código inspeccionable modela `Health : IDamageable`, combate, objetivos de interceptación/escolta/artillería, causas de derrota, persistencia a disco temporal (incluido JSON formateado), volumen y cálculo asíncrono con fuente de más impactos; `GameRoot` construye los tres niveles sobre `Main.unity`. **33/33 casos .NET** se verificaron en copia aislada con SDK 10.0.114 el 2026-11-06. **Estas reglas y scripts no acreditan por sí mismos una campaña jugable:** falta comprobar el Editor, la UI, las pruebas Unity, el guardado real en la ruta del juego y una build fuera del Editor. El presente informe establece cómo registrar resultados finales sin atribuir funcionalidades o contribuciones no observadas.

**Palabras clave:** orientación a objetos, videojuegos, Unity, C#, misiones, trazabilidad, pruebas.

## 2. Contexto, justificación y límites

El problema académico consiste en producir un software interactivo extensible en el que barcos, convoy y baterías compartan reglas de daño, pero tengan criterios de comportamiento y victoria distintos. Se usa una historia **alternativa ficticia** inspirada en un periodo histórico; sus enfrentamientos no ocurrieron como se representan. El alcance previsto es un juego offline de un jugador para PC; se excluyen multijugador, sockets en el juego principal, simulación histórica exacta y combate terrestre.

El plan del curso propone Unity/C#; la aprobación docente debe registrarse porque el sílabo aborda C++/Qt/CMake. Los genéricos, la gestión de memoria y el ciclo de vida de C# y Unity **no son equivalentes literales** de templates, punteros inteligentes ni destructores C++.

## 3. Objetivos y producto final esperado

**Objetivo general:** implementar y validar un juego naval 2D en Unity/C# que demuestre POO, patrones justificados, GUI, persistencia local, pruebas e informe de combate asíncrono seguro.

| Misión final propuesta | Condición de victoria / derrota | Modelo C# disponible | Evidencia Unity aún necesaria |
|---|---|---|---|
| M1. Ecos del Pacífico | Interceptar patrullera / muere Huáscar | `EliminateObjective`, `Mission` y `GameRoot.StartMission(1)` | Menú, combate, HUD, victoria/derrota y reintento ejecutados |
| M2. Convoy bajo fuego | Convoy llega vivo / muere convoy o Huáscar | `EscortObjective`, `Mission` y `GameRoot.StartMission(2)` | Ruta de convoy, segundo enemigo, llegada y pérdida reproducibles |
| M3. Silencio en la costa | Destruir todas las baterías / muere Huáscar | `EliminateManyObjective`, `Mission` y `GameRoot.StartMission(3)` | Escenario de costa, torretas y condición terminal reproducibles |

Los tres niveles están **escritos para crearse dinámicamente dentro de la única escena versionada `Assets/_LRH/Scenes/Main.unity`**; ello no prueba que sean jugables. Medir cada objetivo y enlazar su captura y registro en [pruebas](04_pruebas.md).

## 4. Arquitectura, POO y patrones: evidencia separada de intención

La arquitectura contiene `LRH.Core` en `Assets/_LRH/Scripts/Core/`: salud encapsulada, facciones, estrategias, objetivos, eventos, `Registry<T>`, `Progress`, `ProgressStore` y `CombatReporter`. En `Assets/_LRH/Scripts/Runtime/` **ya existen** `GameRoot.cs`, `ShipActor.cs`, `ProjectilePool.cs`, `GameUi.cs`, `InputAdapter.cs`, `ActorFactory.cs`, `ProgressRepository.cs`, `Visuals.cs` y `SoundFx.cs`; **aún no se han ejecutado aquí en Unity**. `GameRoot` usa `Registry<ShipActor>` para IDs activos y `ShipActor.Spawn` acepta `IInputSource` opcional para inyección. El balance usa `Assets/_LRH/Resources/Configs/*.json` cargado por `ShipConfig.Load`, **no ScriptableObjects**; los SVG son originales y los PNG sprites derivados. Ver [arquitectura](02_arquitectura.md) y [patrones/POO](03_patrones_y_poo.md).

| Concepto del sílabo | Evidencia ya localizada | Prueba para reclamarlo en el producto final |
|---|---|---|
| Encapsulación y contrato | `Health.Current` con setter privado, daño validado e implementación de `IDamageable` | T01–T04/T33 .NET; reducción visible de vida en escena pendiente |
| Interfaces y polimorfismo | `IAttackStrategy` y `IMissionObjective` con implementaciones diferentes | T05–T07, T10–T14 y conductas distintas ejecutadas |
| Strategy · GoF | `ChaseStrategy` / `KeepDistanceStrategy` | Enemigos usando cada estrategia en Play |
| Observer · GoF | Eventos `Changed`, `Died`, `Finished`; `GameUi` enlaza vida de jugador, convoy y enemigos | HUD y misión escuchan; desuscripción sin fugas |
| Genéricos/colecciones | `Registry<T>`, `Dictionary`, `HashSet`; `Registry<ShipActor>` en `GameRoot` para IDs/idempotencia | T20/T14 y ejecución Unity del uso de actores |
| Factory Method | `EnemyFactory.Create` abstracto, redefinido en `PatrolFactory`/`GunboatFactory` | Código presente; test de creación en Unity pendiente |
| Adapter · GoF / MVC adaptado | `IInputSource`/`UnityInputAdapter`, `ShipActor.Spawn` con inyección opcional, `GameRoot`/`GameUi` escritos | Existe test PlayMode de input falso, sin ejecutar; modelo → control → vista por verificar en Unity |
| Persistencia/volumen | `ProgressStore` con T21–T24/T30 y `Progress.TryFromJson`; `Progress.SetVolume` con T25–T26; `JsonProgressRepository` y slider presentes | Archivo en `persistentDataPath`, reinicio y slider/audición en Editor pendientes |
| Concurrencia | `CombatReporter.AnalyzeAsync` copia datos, usa `Task.Run` e identifica `TopSource` | T18–T19/T27–T29 + escena con cancelación/retorno a hilo principal |
| Derrota de convoy/jugador | `Mission.FailureReason` distingue causa y prioriza muerte del jugador | T31–T32 .NET + pantalla/misión 2 en Unity |
| Narrativa, créditos, indicadores y audio | `GameUi.ShowIntro/ShowCredits/ShowResult/ShowHud`, `SoundFx` genera cañón/impacto y `AudioListener.volume` se ajusta | Verificar visual/audio en Unity; cambiar DEV-1…DEV-4 por nombres reales antes de presentar |

`EnemyFactory.Create` sí se redefine en creadores concretos; no confundirlo con un `switch` de factoría simple. Hay enlace modelo/UI **en código**, pero aún falta verlo funcionar en Editor. `Task.Run` no debe manipular `UnityEngine` desde el trabajador; la copia de datos del núcleo no sustituye la prueba del ciclo de vida de Unity.

## 5. Protocolo de validación final y resultados reales

1. Repetir las comprobaciones .NET y conservar salida fechada: `dotnet run --project Tests/CoreChecks/CoreChecks.csproj` con .NET SDK 10.
2. Abrir con **Unity 6000.3.0f1**; en **Edit > Project Settings > Player > Active Input Handling** seleccionar **Input System Package (New)** o **Both**, aceptar reinicio, versionar `ProjectSettings/ProjectSettings.asset`/`Packages/packages-lock.json` generados y abrir `Assets/_LRH/Scenes/Main.unity`. El fallback de entrada legacy cuando falta `ENABLE_INPUT_SYSTEM` **no sustituye** la demostración del paquete. Registrar errores de importación/consola antes de corregirlos.
3. Existen `Assets/_LRH/Tests/EditMode/CoreTests.cs` y `Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs` (**tres pruebas PlayMode: input falso, impacto y derrota; ninguna de victoria**); ejecutarlos en **Window > General > Test Runner**, registrando número **real** por suite. Probar **victoria manualmente**, intro/créditos, vida enemiga y audio/volumen; completar U-01…U-13 del [plan de pruebas](04_pruebas.md).
4. Validar misión 1, misión 2 y misión 3, cada una con victoria y derrota, desbloqueo/reinicio, controles y pausa. Registrar máquina, fecha, versión, pasos, resultado e issue.
5. Si se genera build Windows, ejecutar en otro PC compatible (y sin depender de la carpeta de proyecto) y conservar versión, captura/log y ruta. Si falla o no se produce, **no** indicar «build final lista».

| Modalidad | Estado actualmente documentable | Fecha/equipo/soporte |
|---|---|---|
| `CoreChecks` .NET | **33/33 PASS verificados** en copia aislada | 2026-11-06 · SDK 10.0.114; log/captura [pendiente] |
| Unity Editor, importación y ejecución | No comprobado en este entorno | [pendiente] |
| EditMode y PlayMode en Unity Test Runner | Archivos presentes; **tres casos PlayMode**, sin ejecución comprobada | [pendiente] |
| Tres misiones end-to-end | Sin resultados Unity comprobados | [pendiente] |
| Guardado local y recuperación | T21–T24/T30 prueban IO temporal .NET; guardado Unity sin verificar | [pendiente] |
| Build Windows externa y perfilado | Sin build/medición comprobada | [pendiente] |

**Porcentaje final verificable según rúbrica interna:** [pendiente de evaluación por criterios y enlaces]. No escribir «100 % logrado» ni «60 FPS» sin prueba medible. Si un caso queda abierto, describirlo con impacto y posible mitigación, aunque la fecha de entrega se acerque.

## 6. Recuadros para figuras de la entrega final — **ninguna captura adjunta**

**Instrucción:** reemplazar cada recuadro solo por imagen obtenida del proyecto o del repositorio **real**, enlazada con sintaxis Markdown a un archivo **ya existente** bajo `docs/evidencias/`; registrar autor de captura, fecha **[pendiente]**, versión, máquina y vínculo con caso de prueba. Si no se verifica, dejar el espacio pendiente. Los recuadros son marcadores tipográficos, **no** imágenes ni pruebas.

### Figura 1. Menú y selección de misiones

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│              MENÚ / SELECTOR: CAPTURA REAL [PENDIENTE]                      │
│                                                                              │
│          Mostrar acceso y desbloqueo observados, no una maqueta.             │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 1.** Menú principal y selección de misión. Fecha: **[pendiente]**. Fuente/autor: **[pendiente]**. Versión y caso U-02/U-09: **[pendiente]**.

### Figura 2. Combate y enemigos

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│               COMBATE NAVAL: CAPTURA REAL [PENDIENTE]                       │
│                                                                              │
│        Huáscar, patrullera/cañonera y proyectiles en una partida.            │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 2.** Combate en la campaña. Fecha: **[pendiente]**. Fuente/autor: **[pendiente]**. Misión y caso U-04/U-06: **[pendiente]**.

### Figura 3. HUD durante combate

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│                HUD Y DAÑO: CAPTURA REAL [PENDIENTE]                          │
│                                                                              │
│        Mostrar vida/objetivo actualizado tras una interacción real.          │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 3.** HUD con estado de juego actualizado. Fecha: **[pendiente]**. Fuente/autor: **[pendiente]**. Condición observada/U-05: **[pendiente]**.

### Figura 4. Victoria

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│                 VICTORIA: CAPTURA REAL [PENDIENTE]                           │
│                                                                              │
│       Registrar misión, condición cumplida y transición posterior.           │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 4.** Victoria tras cumplir el objetivo. Fecha: **[pendiente]**. Fuente/autor: **[pendiente]**. Misión y prueba: **[pendiente]**.

### Figura 5. Derrota

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│                 DERROTA: CAPTURA REAL [PENDIENTE]                            │
│                                                                              │
│        Registrar muerte del Huáscar/convoy y opción de reintento.             │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 5.** Derrota y navegación de resultados. Fecha: **[pendiente]**. Fuente/autor: **[pendiente]**. Misión y prueba: **[pendiente]**.

### Figura 6. Código POO y patrón aplicado

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│              CÓDIGO POO: CAPTURA REAL [PENDIENTE]                           │
│                                                                              │
│         Mostrar ruta, clases, contrato y evidencia de uso real.              │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 6.** Interfaz, Strategy, Observer o encapsulación en código propio verificado. Fecha: **[pendiente]**. Archivo/líneas y autoría real: **[pendiente]**.

### Figura 7. Unity Test Runner

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│              UNITY TEST RUNNER: CAPTURA REAL [PENDIENTE]                    │
│                                                                              │
│      Mostrar suites existentes, conteo real y modo Edit/Play.                │
│            CoreChecks .NET NO es el Test Runner de Unity.                    │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 7.** Pruebas Unity ejecutadas. Fecha: **[pendiente]**. Fuente/autor, Editor, suite y resultados reales: **[pendiente]**.

### Figura 8. GitHub y participación verificable

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│                GITHUB: CAPTURA REAL [PENDIENTE]                             │
│                                                                              │
│      Enlazar issues, commits y PR de los integrantes reales.                 │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 8.** Actividad y revisiones reales en GitHub. Fecha: **[pendiente]**. URL/autoría comprobada: **[pendiente]**.

### Figura 9. Misión 2: escolta

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│              CONVOY EN MISIÓN 2: CAPTURA REAL [PENDIENTE]                   │
│                                                                              │
│     Mostrar convoy en ruta y estado del objetivo durante Play.               │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 9.** Escolta y condición de llegada/pérdida. Fecha: **[pendiente]**. Fuente/autor: **[pendiente]**. Caso U-07: **[pendiente]**.

### Figura 10. Misión 3: artillería costera

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│           ARTILLERÍA EN MISIÓN 3: CAPTURA REAL [PENDIENTE]                  │
│                                                                              │
│      Mostrar baterías y objetivo real en juego, no solo sprites.             │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 10.** Neutralización de baterías costeras. Fecha: **[pendiente]**. Fuente/autor: **[pendiente]**. Caso U-08: **[pendiente]**.

## 7. Autorías, limitaciones y conclusiones

| Integrante real | Issues / PR verificados | Módulo y aportes propios | Revisión y prueba vinculadas |
|---|---|---|---|
| [DEV-1 pendiente] | [pendiente] | [pendiente] | [pendiente] |
| [DEV-2 pendiente] | [pendiente] | [pendiente] | [pendiente] |
| [DEV-3 pendiente] | [pendiente] | [pendiente] | [pendiente] |
| [DEV-4 pendiente] | [pendiente] | [pendiente] | [pendiente] |

**Limitaciones a declarar hasta resolverlas:** Editor no instalado/ejecutado aquí; ejecución de escenas, resultados y guardado a disco sin registro; build/capturas/PR sin evidencia. Después de ensayar en otro equipo, reemplazar estas notas únicamente con datos reales, sin borrar los fallos importantes o cambiar autorías para equilibrar la tabla. Consultar [guía de GitHub](GUIA_COMMITS.md) para el procedimiento ético de contribución.

**Conclusión provisional:** existe una base verificable de reglas C# y una ruta explícita para obtener evidencia del producto; el juego completo y su calidad final **no pueden inferirse de las 33 pruebas del núcleo**. La conclusión de entrega, el porcentaje interno y la aceptación docente deben redactarse tras ejecutar cada misión, las suites y la build en condiciones registradas.

**Fuentes:** [plan maestro original aportado](PLAN_MAESTRO_ORIGINAL.md) (**copia literal de la especificación, no evidencia de implementación**), [requisitos](01_requisitos.md), [arquitectura](02_arquitectura.md), [POO y patrones](03_patrones_y_poo.md), [plan de pruebas](04_pruebas.md), [Unity Manual](https://docs.unity3d.com/), [Microsoft Learn: C#](https://learn.microsoft.com/dotnet/csharp/). Añadir solo bibliografía realmente consultada, sílabo y fecha oficiales en la versión entregada.
