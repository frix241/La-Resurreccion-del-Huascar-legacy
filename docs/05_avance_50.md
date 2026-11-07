# Informe académico — primer avance propuesto (50 %)

> **Borrador para completar con evidencia real.** «50 %» identifica el **hito previsto**, no un porcentaje alcanzado. El núcleo C# obtuvo **33/33 PASS verificados** al ejecutar una copia aislada de fuentes actuales. Existen scripts Runtime y suites Unity, pero **Unity Editor no está disponible en el entorno de preparación**: no se han verificado aquí escena, GUI, Test Runner, capturas ni build. No presentar este borrador como informe final sin ejecutar y registrar esas comprobaciones.

**Universidad Nacional de San Agustín de Arequipa**  
**Facultad de Ingeniería de Producción y Servicios**  
**Escuela Profesional de Ingeniería de Sistemas**  
**Asignatura:** Tecnología de Objetos (1703240) · **Periodo:** 2026-B  
**Proyecto:** *La Resurrección del Huáscar: Una Historia Alternativa Después de la Guerra del Pacífico*  
**Documento:** Informe del primer avance, hito del 50 % (rúbrica interna propuesta)  
**Sección:** [pendiente] · **Docente:** [pendiente de confirmar]  
**Fecha oficial de entrega:** [pendiente] · **Versión/tag real:** [pendiente; no declarar `v0.5.0` sin crearlo]  
**Repositorio GitHub real:** [pendiente]  
**Lugar:** Arequipa, Perú.

| Integrante | Código / usuario GitHub | Responsabilidad y evidencia real |
|---|---|---|
| DEV-1: [nombre pendiente] | [pendiente] | [issues y PR propios, pendientes] |
| DEV-2: [nombre pendiente] | [pendiente] | [issues y PR propios, pendientes] |
| DEV-3: [nombre pendiente] | [pendiente] | [issues y PR propios, pendientes] |
| DEV-4: [nombre pendiente] | [pendiente] | [issues y PR propios, pendientes] |

## 1. Resumen ejecutivo

LRH propone aplicar orientación a objetos a un videojuego naval 2D mediante un núcleo de reglas independiente de Unity y código de presentación/adaptación separado. Existen clases de vida (`Health : IDamageable`), facciones, estrategias, objetivos de tres misiones (incluida causa de derrota), guardado de progreso/volumen con JSON formateado, balance JSON y reporte asíncrono que identifica el origen con más impactos; `GameRoot` codifica el menú y los tres niveles **creados durante la ejecución** desde la escena única `Main.unity`. El comprobador `Tests/CoreChecks/Program.cs` obtuvo **33/33 PASS** en una copia aislada (SDK 10.0.114, 2026-11-06; log no archivado aquí). Esto constituye evidencia del **núcleo**, no del primer corte vertical jugable: menú, controles, colisiones, interfaz y resultado de la misión 1 siguen sin prueba documentada en Unity. Por ello, el porcentaje del hito **no se declara alcanzado**.

**Palabras clave:** C#, Unity, orientación a objetos, Strategy, eventos, pruebas, juego naval.

## 2. Introducción, problema y objetivos

Un juego necesita coordinar entidades con estados, daño, objetivos y comunicación con la interfaz. Si todo se concentra en un único componente visual, es difícil verificar reglas aisladas y ampliar el producto sin regresiones. La pregunta de trabajo es: **¿cómo separar las decisiones del juego de sus mecanismos de entrada, física y presentación, manteniendo reglas comprobables?** La temática naval facilita diferenciar un jugador, adversarios, convoy y artillería sin presentar la ficción como historia real.

**Objetivo general:** diseñar y validar un videojuego 2D en Unity/C# que integre POO, patrones justificados, interfaz, pruebas y un ejemplo seguro de concurrencia. Para este hito se propone una **Misión 1 jugable de extremo a extremo**; no basta con poseer clases aisladas. Se requiere confirmar con la docente que Unity/C# y la rúbrica interna son admisibles frente al énfasis del sílabo en C++/Qt.

**Objetivos específicos de este corte:** encapsular vida y daño; probar decisiones de combate y misión; integrar movimiento, apuntado y proyectiles; mostrar HUD y resultados; conservar trazabilidad entre clase, requisito, prueba y autoría real. Los tres primeros objetivos de dominio cuentan con código/comprobaciones .NET; la integración visual necesita verificación adicional.

## 3. Alcance y estado verificable

| Entregable del primer corte | Evidencia disponible | Pendiente antes de declararlo cumplido |
|---|---|---|
| Núcleo de salud, reglas de facción, estrategias, misión y persistencia | Archivos en `Assets/_LRH/Scripts/Core/` y **33/33** `CoreChecks` verificados en copia aislada | Archivar salida de ejecución sobre el árbol de entrega |
| Escena de inicio y niveles por código | `Assets/_LRH/Scenes/Main.unity`; `GameRoot.StartMission(1..3)` | Importación, compilación y ejecución en Unity 6000.3.0f1 |
| Menú → introducción → misión 1 → combate → victoria/derrota → menú | `GameUi.ShowIntro`/`GameRoot`/cierres narrativos escritos; sin evidencia de ejecución en Editor | Prueba end-to-end con resultados y capturas reales |
| Patrullera, disparo, colisiones, HUD y audio | Código en `ShipActor`, `ProjectilePool`, `ActorFactory`, `GameUi` (indicadores enemigos por eventos) y `SoundFx` (tonos procedurales); PNG derivados de SVG originales | IA, colisiones, GUI y efectos audibles comprobados en PlayMode/manual |
| Balance | `Resources/Configs/*.json` + `ShipConfig.Load`, **no ScriptableObjects** | Validar carga/configuraciones al entrar en Play |
| Unity Test Runner y build | Existen archivos de tests EditMode/PlayMode; ninguna ejecución/build comprobada | Ejecutar suites y/o reproducción controlada; build solo si se genera y prueba |
| GitHub y trabajo de cuatro personas | Roles propuestos en la guía | Crear repo/PR/issues reales y atribuirlos a quienes trabajaron |

**Criterio del hito:** la rúbrica interna del plan maestro reserva 50 puntos de 100 para requisitos/arquitectura (10), POO y patrones iniciales (10), mecánica central (20), GUI (5) y pruebas/GitHub/integración (5). **Puntos comprobados: [pendiente de evaluar]**; no asignar puntos a criterios no demostrados ni confundir 20 tests con 20 puntos. Fecha/rúbrica docente: **[pendiente]**.

## 4. Arquitectura y trazabilidad POO

`LRH.Core` contiene `Health : IDamageable`, `DamageRules`, `IAttackStrategy`, `Mission`, `Progress`, `ProgressStore`, `CombatReporter` y `Registry<T>` sin `UnityEngine`. `Health` protege `Current` y notifica `Died` una sola vez; T33 verifica despacho del daño por interfaz. Dos estrategias implementan `IAttackStrategy`, y `Mission` recibe un `IMissionObjective`, lo cual permite cambiar el criterio de victoria. `Mission.Resolve` prioriza la derrota cuando concurren hechos terminales en una misma resolución. `ShipActor.Receive` consume `IDamageable` en código Runtime, cuya ejecución requiere Unity. `LRH.Runtime` incluye además `EnemyFactory` con creadores concretos, adaptador de input y `GameUi`; todo ello requiere validación en Unity. El balance se carga desde JSON en `Resources/Configs/` mediante `ShipConfig.Load`. Estos contratos se describen en [arquitectura](02_arquitectura.md) y [POO/patrones](03_patrones_y_poo.md).

Se pueden defender **Strategy**, eventos del modelo (base de **Observer**) y colecciones genéricas en C# puro. **Factory Method** (`EnemyFactory`), **Adapter** (`IInputSource`/`UnityInputAdapter`) y separación modelo-vista-controlador tienen clases concretas escritas; **aún no están demostrados funcionando en Editor**. No atribuir al juego una jerarquía de barcos ausente. El plan del curso es una propuesta conceptual; los nombres y ubicaciones del repositorio actual son la fuente de verdad.

**Trazabilidad mínima:** RF-05 ↔ `Health : IDamageable`/`DamageRules` ↔ T01–T04/T08/T33; RF-06 ↔ `IAttackStrategy` ↔ T05–T07; RF-07/09 ↔ `Mission` ↔ T10–T11; RF-08 (HUD) ↔ **prueba Unity pendiente**. Ver matriz completa en [requisitos](01_requisitos.md).

## 5. Metodología y resultados de pruebas

En el primer corte se priorizan reglas deterministas comprobadas fuera de escenas y luego se exige una validación en Unity del flujo real. Ejecutar desde raíz:

```bash
dotnet run --project Tests/CoreChecks/CoreChecks.csproj
```

| Modalidad | Resultado que puede consignarse | Fecha / soporte de evidencia |
|---|---|---|
| `CoreChecks` .NET (33 casos) | **33/33 PASS verificados** en copia aislada | 2026-11-06 · SDK 10.0.114; log/captura [pendiente] |
| Unity Editor 6000.3.0f1: importación y Play | **No ejecutado en este entorno** | [pendiente] |
| Unity Test Runner: EditMode / PlayMode | Archivos presentes; **3 pruebas PlayMode (input falso, impacto y derrota), no ejecutadas** | [pendiente] |
| Test end-to-end de misión 1 | **No ejecutado en este entorno** | [pendiente] |
| Build/ejecución fuera del Editor | **No construida ni verificada aquí** | [pendiente] |

Para declarar el corte vertical, documentar tanto una **victoria manual en Unity** (no hay test PlayMode de victoria) como una derrota reproducible, los controles W/S, A/D, mouse, clic izquierdo y Escape, daños y actualización del HUD de jugador/enemigo. El test PlayMode de input falso **existe, no se ha ejecutado**. Ajustar Active Input Handling a **Input System Package (New)** o **Both** y versionar los ajustes/lock generados; el fallback legacy solo facilita abrir el proyecto. Probar también intro, audio/volumen y créditos con nombres reales. Si un caso falla, consignar error e issue, no sustituir la prueba por imagen de maqueta. Casos detallados en [plan de pruebas](04_pruebas.md).

## 6. Recuadros para evidencia visual — **sin imágenes insertadas**

**Instrucción de edición:** una vez tomada cada captura del proyecto **en ejecución real** o del código/repositorio real, sustituir el recuadro por una imagen Markdown enlazada a un archivo **ya existente** bajo `docs/evidencias/` y rellenar fecha, fuente y entorno. Si no se obtiene, conservar «pendiente»; no presentar el recuadro como captura.

### Figura 1. Menú e inicio de Misión 1

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│               CAPTURA REAL DEL MENÚ: [PENDIENTE]                             │
│                                                                              │
│          Mostrar título, botón de inicio y escena visible en Unity.          │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 1.** Menú principal e inicio de Misión 1. Fecha: **[pendiente]**. Fuente/autor de captura: **[pendiente]**. Versión/archivo: **[pendiente]**.

### Figura 2. Combate naval

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│          CAPTURA REAL DEL HUÁSCAR, PATRULLERA Y DISPARO: [PENDIENTE]        │
│                                                                              │
│         Mostrar interacción observada, no solo sprites en el proyecto.       │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 2.** Combate durante la Misión 1. Fecha: **[pendiente]**. Fuente/autor de captura: **[pendiente]**. Resultado observado: **[pendiente]**.

### Figura 3. HUD y daño

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│                 CAPTURA REAL DEL HUD ACTUALIZADO: [PENDIENTE]                │
│                                                                              │
│          Mostrar vida y objetivo con cambios verificables en Play.           │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 3.** HUD de vida y objetivo durante combate. Fecha: **[pendiente]**. Fuente/autor de captura: **[pendiente]**. Estado mostrado: **[pendiente]**.

### Figura 4. Victoria

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│                 CAPTURA REAL DEL RESULTADO VICTORIA: [PENDIENTE]            │
│                                                                              │
│             Mostrar pantalla posterior a destruir patrullera.                │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 4.** Victoria de Misión 1 y navegación posterior. Fecha: **[pendiente]**. Fuente/autor de captura: **[pendiente]**. Evidencia de ejecución: **[pendiente]**.

### Figura 5. Derrota

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│                  CAPTURA REAL DEL RESULTADO DERROTA: [PENDIENTE]            │
│                                                                              │
│            Mostrar pantalla tras destrucción del Huáscar.                    │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 5.** Derrota y opción de reintento/menú. Fecha: **[pendiente]**. Fuente/autor de captura: **[pendiente]**. Pasos para reproducir: **[pendiente]**.

### Figura 6. Código orientado a objetos

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│           CAPTURA REAL DEL CÓDIGO POO (RUTA VISIBLE): [PENDIENTE]           │
│                                                                              │
│      Sugerencia: Health.cs + evento Died o Combat.cs + IAttackStrategy.       │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 6.** Encapsulación/interfaz/polimorfismo en código real. Fecha: **[pendiente]**. Archivo, líneas y autoría comprobada: **[pendiente]**.

### Figura 7. Unity Test Runner

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│              CAPTURA REAL DEL UNITY TEST RUNNER: [PENDIENTE]                │
│                                                                              │
│   Mostrar EditMode/PlayMode y conteo solo si existen y se ejecutaron.        │
│         La salida de CoreChecks NO sustituye esta captura.                   │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 7.** Ejecución real de pruebas en Unity. Fecha: **[pendiente]**. Versión Editor, suites y resultados: **[pendiente]**.

### Figura 8. GitHub y contribuciones

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│                                                                              │
│          CAPTURA REAL DE GITHUB: COMMITS, ISSUES Y PR: [PENDIENTE]          │
│                                                                              │
│    Enlazar repositorio y aportes de las cuatro personas si son reales.      │
│                                                                              │
│                         NO ES UNA CAPTURA                                    │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
**Pie — Figura 8.** Actividad GitHub verificable del equipo. Fecha: **[pendiente]**. URL/autoría real de cada contribución: **[pendiente]**.

## 7. Aportes, riesgos y siguientes pasos

| Persona real | Issue y PR real | Archivos/cambio realizado | Prueba y fecha |
|---|---|---|---|
| [DEV-1 pendiente] | [pendiente] | [pendiente] | [pendiente] |
| [DEV-2 pendiente] | [pendiente] | [pendiente] | [pendiente] |
| [DEV-3 pendiente] | [pendiente] | [pendiente] | [pendiente] |
| [DEV-4 pendiente] | [pendiente] | [pendiente] | [pendiente] |

**Riesgos abiertos:** aprobación del stack por la docente; falta de `ProjectSettings/ProjectSettings.asset` con Active Input Handling y `Packages/packages-lock.json` generados por Editor; ausencia de Editor para validar escena y pruebas Unity; falta de evidencia visual; ausencia de una build verificada. **Acción inmediata:** instalar la versión fijada, configurar Input System, registrar U-01…U-05 de `docs/04_pruebas.md`, repetir y archivar los 33 casos .NET desde el árbol de entrega, verificar la misión 1 codificada en Runtime y obtener capturas genuinas. Luego probar niveles 2–3, slider/guardado en Unity y build si se solicita.

## 8. Conclusión provisional y fuentes

El código disponible hace defendibles ciertas reglas de POO por pruebas independientes; **todavía no demuestra una experiencia interactiva completa**. La conclusión del hito y su porcentaje se redactarán tras las pruebas del Editor y la revisión docente. No se declara ejecutable Windows ni release existente.

**Fuentes de consulta y trazabilidad:** [plan maestro original aportado](PLAN_MAESTRO_ORIGINAL.md) (copia literal; **planificación, no evidencia de implementación**); [requisitos](01_requisitos.md), [arquitectura](02_arquitectura.md), [POO](03_patrones_y_poo.md), [pruebas](04_pruebas.md), [Unity Manual](https://docs.unity3d.com/), [Microsoft Learn: C#](https://learn.microsoft.com/dotnet/csharp/). Añadir bibliografía efectivamente consultada y el sílabo/consigna oficial al presentar.
