# Investigación — dirección visual 2D para estrategia naval

**Estado:** referencias primarias y criterios de diseño; no es un art bible final ni autoriza copiar assets.  
**Proyecto:** *La Resurrección del Huáscar: Una Historia Alternativa Después de la Guerra del Pacífico* (*HuascaRebirth*).  
**Fecha de consulta:** 2026-11-06.  
**Convención observada:** la carpeta `docs/` usa Markdown en español, títulos numerados descriptivos, tablas de trazabilidad y advertencias explícitas cuando algo es una recomendación o todavía no está verificado. Este documento sigue esa convención.

## 1. Resumen ejecutivo

La dirección recomendada es una **carta naval 2D moderna y pulida**: cámara ortográfica, siluetas de barcos legibles, mapa de bajo contraste, jerarquía clara entre mundo/HUD/marcadores y acentos luminosos reservados para decisiones tácticas. La documentación oficial de Unity respalda separar la interfaz en Canvas con modos de renderizado distintos, controlar el escalado mediante una resolución de referencia, usar `Camera.orthographicSize` como una variable de diseño del zoom y organizar sprites mediante Sorting Layers, `Order in Layer`, tintes y pivotes.

La iluminación 2D de URP debe tratarse como **señal de estado y atmósfera**, no como una capa decorativa constante: Unity permite filtrar luces por Sorting Layers o Rendering Layers, pero advierte de costes adicionales cuando las luces afectan a varias capas. Las fuentes first-party con licencia clara encontradas son repositorios técnicos de Unity (`2d-extras` y `2d-techdemos`), útiles para estudiar implementación y flujo, no para tomar sprites, texturas, capturas, logos o identidad visual.

## 2. Principios visuales aplicables a HuascaRebirth

| Principio | Decisión aplicable | Criterio de aceptación |
|---|---|---|
| Lectura táctica antes que ornamento | La silueta, facción, estado y orden activa deben distinguirse antes que el detalle material | Un barco se identifica en el zoom operativo sin depender de una textura pequeña |
| Separación de planos | HUD persistente, marcadores anclados al mundo y escena naval no deben competir en el mismo plano | El jugador puede leer recursos/turno y seleccionar una unidad sin confundirlos con el mapa |
| Color como canal, no como único lenguaje | Combinar color con icono, contorno, valor, forma o animación | Daño, selección, amenaza y objetivo siguen siendo distinguibles con color reducido |
| Iluminación con propósito | Usar luces locales para peligro, detección, puerto, clima y eventos | La escena sigue siendo legible al desactivar luces de evento |
| Consistencia de escala | Diseñar UI y zoom con referencias explícitas | No aparecen paneles recortados ni etiquetas tácticas ilegibles en relaciones de aspecto soportadas |

## 3. Canvas, UI y composición de capas

### 3.1. Hechos documentados por Unity

Unity define Canvas como el espacio abstracto donde se dispone y renderiza la UI, y establece que todos los elementos UI deben ser hijos de un GameObject con componente Canvas. El manual documenta tres modos: `Screen Space - Overlay`, `Screen Space - Camera` y `World Space` ([Canvas, Unity 2022.3](https://docs.unity3d.com/2022.3/Documentation/Manual/class-Canvas.html), secciones “Canvas”, “Render Mode” y “Details”).

La referencia de UGUI precisa que los elementos se dibujan en el mismo orden en que aparecen en la jerarquía: el primer hijo se dibuja primero y el posterior queda encima si se superponen ([UGUI Canvas](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/UICanvas.html), sección “Draw order of elements”).

> “All UI elements must be children of a GameObject that has a Canvas component attached.”  
> — Unity, [Canvas](https://docs.unity3d.com/2022.3/Documentation/Manual/class-Canvas.html)

> “UI elements in the Canvas are drawn in the same order they appear in the Hierarchy.”  
> — Unity, [UGUI Canvas](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/manual/UICanvas.html)

El manual describe que `Screen Space - Overlay` se dibuja directamente sobre la pantalla sin referencia a una cámara; `Screen Space - Camera` se renderiza como un plano delante de una cámara; y `World Space` se comporta como un objeto de la escena, con tamaño y orientación dependientes del mundo ([Canvas, Unity 2022.3](https://docs.unity3d.com/2022.3/Documentation/Manual/class-Canvas.html), subsecciones “Screen Space - Overlay”, “Screen Space - Camera” y “World Space”).

### 3.2. Aplicación propuesta

No conviene mezclar todo en un Canvas único. La separación propuesta es:

| Plano | Modo sugerido | Contenido |
|---|---|---|
| HUD | `Screen Space - Overlay` | Turno, recursos, objetivos, pausa, minimapa y paneles persistentes |
| Interfaz dependiente de cámara | `Screen Space - Camera` | Elementos que deban compartir frustum o profundidad visual con una vista concreta |
| Marcadores tácticos | `World Space` | Nombre/estado del barco, radio de alcance, selección, ruta, orden y detección anclados al mapa |
| Escena | SpriteRenderers y Tilemap | Mar, costa, islas, barcos, proyectiles y efectos |

Orden inicial de la jerarquía del HUD:

```text
HUD_Background
HUD_MapFrame
HUD_Resources
HUD_Objectives
HUD_Selection
HUD_Alerts
HUD_Pause
```

La jerarquía anterior es una propuesta de composición, no una exigencia de Unity. Debe mantenerse un orden explícito porque la documentación de UGUI hace que el orden de hermanos determine qué elemento queda encima.

## 4. Escalado de UI y resolución de referencia

Unity describe `Canvas Scaler` como el componente que controla la escala global y la densidad de píxeles de los elementos UI, incluidos tamaños tipográficos y bordes de imágenes ([Canvas Scaler, Unity 2022.3](https://docs.unity3d.com/2022.3/Documentation/Manual/script-CanvasScaler.html), introducción).

En `Scale With Screen Size`, `Reference Resolution` es la resolución para la que se diseña el layout. Unity documenta además:

- `Match Width or Height`: mezcla ancho y alto como referencia.
- `Expand`: amplía el área para que no sea menor que la referencia.
- `Shrink`: recorta el área para que no sea mayor que la referencia.

Fuentes: [Canvas Scaler, Unity 2022.3](https://docs.unity3d.com/2022.3/Documentation/Manual/script-CanvasScaler.html), tabla “Settings for Scale With Screen Size” y sección “Scale With Screen Size”.

### Criterios para el proyecto

1. Usar `Scale With Screen Size` en el HUD.
2. Elegir y registrar una resolución de referencia horizontal en el proyecto; no cambiarla informalmente entre escenas.
3. Empezar con `Expand` para que una relación de aspecto más ancha no elimine información táctica.
4. Probar explícitamente relaciones de aspecto estrechas antes de aceptar el layout.
5. Validar tamaños de fuente, botones, etiquetas de barcos, panel de objetivo y minimapa, no sólo el panel principal.

El diseño visual debe tratar tipografía, márgenes, grosor de líneas, radios, contraste y colores de estado como un mismo sistema. La resolución de referencia no sustituye las pruebas de legibilidad.

## 5. Cámara ortográfica y bandas de zoom

### 5.1. Referencias primarias

La API de Unity define `Camera.orthographic` como la propiedad que indica si la cámara es ortográfica (`true`) o perspectiva (`false`). También advierte que el renderizado diferido no está soportado con proyección ortográfica y que se utiliza Forward rendering ([Camera.orthographic, Unity 6](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/camera/orthographic)).

`Camera.orthographicSize` es la mitad del tamaño de la cámara en modo ortográfico. Unity especifica que la altura del volumen visible es `orthographicSize * 2` y que el ancho se calcula con `orthographicSize` y `Camera.aspect` ([Camera.orthographicSize, Unity 6](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/camera/orthographicsize), definición y ejemplo).

> “Camera's half-size when in orthographic mode.”  
> — Unity, [Camera.orthographicSize](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityengine/camera/orthographicsize)

### 5.2. Aplicación artística

La proyección ortográfica es una buena base para un mapa naval táctico porque mantiene la lectura de siluetas, símbolos y rutas sin introducir la convergencia visual de una perspectiva dominante. Esto es un criterio de dirección para HuascaRebirth, no una afirmación de que Unity prescriba un género artístico.

Definir tres bandas de lectura, cada una con una prueba de composición:

| Banda | Debe dominar visualmente | Elementos que pueden aparecer |
|---|---|---|
| Estratégica | Regiones, rutas, puertos y flotas | Etiquetas resumidas, estados globales y objetivos |
| Operativa | Barcos, formaciones, alcance y órdenes | Silueta, facción, daño, selección y trayectorias |
| Presentación | Puerto, tormenta, evento o detalle narrativo | Materiales, efectos, iluminación local y texto contextual |

El zoom debe interpolar dentro de límites definidos, no revelar detalles que vuelvan ilegibles los marcadores ni ocultar la información necesaria para decidir. Registrar los límites junto con la cámara y probarlos en la escena real.

## 6. SpriteRenderer: lenguaje de barcos y orden espacial

La referencia oficial de `SpriteRenderer` documenta propiedades directamente útiles para la dirección visual:

- `Color` aplica un tinte al sprite.
- `Flip` refleja la textura en X o Y sin mover el GameObject.
- `Draw Mode` ofrece `Simple`, `Sliced` y `Tiled` para distintas formas de escalado.
- `Sprite Sort Point` puede usar `Center` o `Pivot`.
- El material predeterminado documentado es `Sprite-Lit-Default`.
- `Sorting Layer` determina la capa de renderizado y `Order in Layer` el orden dentro de ella.

Fuente: [Sprite Renderer component reference, Unity 6.0.6](https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/sprite/sprite-renderer-reference), tabla principal y sección “Additional Settings”.

### 6.1. Criterios de silueta

El barco debe reconocerse por, en este orden:

1. silueta general y proporción;
2. perfil de proa y popa;
3. superestructura o armamento;
4. bandera/facción y valor cromático;
5. iconos de estado y selección.

El detalle de madera, metal, reflejo o humo puede enriquecer la presentación, pero no debe ser necesario para identificar la unidad durante la decisión táctica. El tinte de `SpriteRenderer` es útil para facción, selección, daño, niebla de guerra o desactivación, pero cada estado importante debe tener también una señal no cromática.

### 6.2. Sorting Layers propuestas

```text
OceanBase
OceanTexture
Terrain
Structures
Ships
ShipEffects
Selection
WorldMarkers
```

Esta lista es una propuesta para HuascaRebirth. `Order in Layer` debe resolver excepciones locales; no usar órdenes numéricos arbitrarios como sustituto de una gramática de capas.

Para barcos largos o formaciones, revisar `Sprite Sort Point`: un pivote artístico situado en el centro táctico puede ser más estable que el centro geométrico del bitmap. La decisión debe probarse con solapamientos reales.

## 7. Iluminación 2D en URP

### 7.1. Capacidades y límites documentados

La guía oficial de Unity presenta la iluminación 2D de URP junto con tipos de luces, configuración del 2D Renderer, preparación de sprites, modos de mezcla, sombras, Shader Graph, compatibilidad con VFX y optimización ([2D lighting in URP, Unity 6.0.1](https://docs.unity3d.com/6000.1/Documentation/Manual/urp/2d-index.html)).

La guía de creación de luces 2D confirma que los sprites y tiles nuevos usan por defecto el shader `Sprite-Lit-Default`, que las luces y sombras 2D no dependen de mover la luz hacia delante o atrás y que una luz afecta por defecto a todos los GameObjects 2D. También documenta el filtrado mediante Sorting Layers o Rendering Layers, la posibilidad de iluminación volumétrica y la necesidad de evitar auto-intersecciones en luces Freeform ([Create a 2D light in URP, Unity 6.0.6](https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/2d-urp/2d-index/2d-light-properties-explained), secciones “Configure which lights affect which GameObjects”, “Add volumetric lighting” y “Avoid self-intersection in Freeform lights”).

> “By default, a 2D light affects all 2D GameObjects in the scene.”  
> — Unity, [Create a 2D light in URP](https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/2d-urp/2d-index/2d-light-properties-explained)

> “When lights affect different sorting layers, performance can decrease because Unity renders more light and shadow textures.”  
> — Unity, [Create a 2D light in URP](https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/2d-urp/2d-index/2d-light-properties-explained)

### 7.2. Uso visual propuesto

| Uso | Tratamiento recomendado |
|---|---|
| Base del mapa | Luz contenida y contraste suficiente sin efectos dinámicos |
| Selección/objetivo | Halo o luz local breve, combinada con contorno/icono |
| Detección o zona de control | Gradiente o luz filtrada, con leyenda explícita |
| Artillería/impacto | Flash corto y de alto contraste, sin cambiar la lectura del mapa |
| Tormenta, amanecer o noche | Variación global moderada; revisar que HUD y siluetas sobrevivan |
| Volumetría | Reservada para momentos narrativos o atmosféricos, no para cada unidad |

No añadir luces a cada objeto por defecto. Filtrar por capas cuando proceda y medir el coste si una luz atraviesa muchas Sorting Layers. Antes de cerrar una escena, comprobarla en modo sin iluminación adicional para confirmar que la composición fundamental funciona.

## 8. Referencias first-party con licencia clara

### 8.1. Unity 2D Tilemap Extras

Repositorio: [Unity-Technologies/2d-extras](https://github.com/Unity-Technologies/2d-extras).  
Licencia: [LICENSE.md](https://github.com/Unity-Technologies/2d-extras/blob/master/LICENSE.md).  
Licencia asociada: [Unity Companion License v1.4](https://unity.com/legal/licenses/unity-companion-license).

El archivo de licencia del repositorio identifica “2D Tilemap Extras copyright © 2020 Unity Technologies ApS” y remite a la Unity Companion License. Esta licencia concede permiso mundial, no exclusivo y sin regalías para reproducir, modificar, mostrar y distribuir el Work **en conexión con** contenido creado bajo una licencia válida de Unity; exige conservar los avisos y advierte que los avisos de terceros pueden tener términos independientes ([Unity Companion License v1.4](https://unity.com/legal/licenses/unity-companion-license), cláusulas 1, 3 y 5).

**Uso recomendado:** estudiar organización técnica de Tilemaps y herramientas 2D. No asumir que cada textura, sprite u otro contenido acompañado tiene la misma licencia sin revisar sus avisos.

### 8.2. Unity 2D Tech Demos

Repositorio: [Unity-Technologies/2d-techdemos](https://github.com/Unity-Technologies/2d-techdemos).  
Licencia: [MIT License](https://github.com/Unity-Technologies/2d-techdemos/blob/master/LICENSE.md).

El `LICENSE.md` concede permiso para usar, copiar, modificar, publicar, distribuir, sublicenciar y vender el software, con la condición de conservar el aviso de copyright y la licencia en copias o partes sustanciales. La licencia del repositorio no demuestra por sí sola que assets externos (audio, fuentes, texturas u otros contenidos de terceros) estén cubiertos por MIT.

**Uso recomendado:** estudiar implementación y estructura de demostraciones 2D, no copiar su identidad visual.

### 8.3. Fuentes que no deben tratarse como biblioteca de assets

Unity Learn puede servir para estudiar conceptos, pero sus materiales no deben incorporarse automáticamente a una build comercial. La referencia de Creative Core Lighting enlaza a [Unity Learn — Creative Core: Lighting](https://learn.unity.com/project/creative-core-lighting); antes de reutilizar cualquier material debe revisarse la licencia/condiciones aplicables de [Learning Products and Services](https://unity.com/legal/terms-of-service/learning).

No se encontró una guía pública first-party de un juego naval que ofrezca simultáneamente un art bible completo, imágenes reutilizables y una licencia clara para incorporarlas a HuascaRebirth. Por ello, las obras comerciales deben limitarse a inspiración conceptual independiente, nunca a extracción de capturas, logos, sprites o identidad.

## 9. Checklist de dirección artística

- [ ] La cámara usa ortográfica y tiene bandas de zoom documentadas.
- [ ] El barco se reconoce por silueta a zoom operativo.
- [ ] El mapa mantiene bajo contraste detrás de unidades, rutas y órdenes.
- [ ] El HUD está separado del mundo y tiene Canvas Scaler configurado con resolución de referencia.
- [ ] La jerarquía de UI determina deliberadamente el orden de superposición.
- [ ] Facción, daño, selección y amenaza tienen señales cromáticas y no cromáticas.
- [ ] Las Sorting Layers están definidas antes de resolver excepciones con `Order in Layer`.
- [ ] Las luces de evento están filtradas y no son necesarias para leer el mapa.
- [ ] Se mide el impacto de luces que afectan a múltiples capas.
- [ ] Se revisa cada recurso externo, aviso de terceros y licencia antes de incorporarlo.
- [ ] No se copian assets, capturas, logos ni rasgos identificables de un juego ajeno.

## 10. Alcance y cautelas

Estas referencias describen capacidades de Unity y términos de licencia; no prueban todavía el resultado visual de HuascaRebirth. La validación pendiente debe hacerse en la escena real: tamaños y pivotes de sprites, límites de cámara, resolución objetivo, contraste, accesibilidad, rendimiento de URP y lectura de HUD durante una misión completa.

La licencia del repositorio técnico no sustituye un registro de procedencia por archivo. Para los recursos originales del proyecto, conservar la trazabilidad ya establecida en [`creditos_assets.md`](creditos_assets.md). Para cualquier recurso nuevo, registrar creador, URL permanente, licencia exacta, fecha de consulta, cambios y obligaciones de atribución antes de distribuir una build.
