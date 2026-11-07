# Investigación — terreno modular y editor de mapas

**Proyecto:** La Resurrección del Huáscar.  
**Consulta:** 2026-11-07.  
**Motor local:** Unity 6000.6.8f1.  
**Estado:** investigación y recomendaciones; no constituye evidencia de una implementación ni de pruebas aprobadas.

## 1. Qué significa la referencia a Warcraft

Blizzard distingue el Terrain Editor, que modifica geometría y coloca unidades, objetos ambientales y regiones, del Object Editor, que configura objetos y unidades. Sus herramientas permiten pintar terreno y colocar elementos persistentes, y recomiendan comprobar que los espacios y pasos permiten el movimiento. Esa es la referencia funcional relevante para este proyecto: un mapa compuesto por terreno y objetos con propiedades, no una única ilustración ampliada. Fuente primaria: [Blizzard, Revisiting the Warcraft III Editor](https://news.blizzard.com/en-us/article/23395649/revisiting-the-warcraft-iii-editor).

Blizzard también documenta que los tipos de terreno se agrupan en tilesets, con posibilidad de combinaciones personalizadas. No se infiere de ello el formato interno exacto de navegación de Warcraft III ni se copian sus assets. Fuente: [World Editor FAQ de Blizzard](https://classic.battle.net/war3/faq/worldeditor.shtml).

## 2. Componentes Unity verificados

| Necesidad | API / componente | Aplicación propuesta |
|---|---|---|
| Posiciones de terreno estables | `Grid` + `Tilemap` | Agua, playa y tierra en celdas del mismo espacio mundial |
| Carga o edición de celdas | `SetTile`, `SetTiles`, `SetTilesBlock`, `GetTile` | Construir desde datos guardados y aplicar pincel de editor |
| Colisión según material | `Tile.ColliderType.None`, `.Grid`, `.Sprite` | Agua navegable, tierra sólida, formas costeras específicas |
| Colisión de la capa | `TilemapCollider2D` | Derivar geometría del terreno colocado |
| Unión de celdas sólidas | `CompositeCollider2D` + cuerpo estático | Reducir superficies internas entre celdas contiguas |
| Conversión de coordenadas | `WorldToCell`, `GetCellCenterWorld` | Colocar objetos, validar spawns y proyectar marcadores |

`Tilemap` almacena sprites en posiciones de una cuadrícula y expone las operaciones de lectura y escritura anteriores. Fuente: [Unity 6000.6, Tilemap API](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/tilemaps/tilemap).

La colisión puede desactivarse por tile o utilizar la forma física del sprite. Unity recomienda componer formas adyacentes y explica que los cambios de collider se agrupan en `LateUpdate`; `ProcessTilemapChanges()` fuerza su actualización antes de una consulta o prueba que necesite la geometría nueva. Fuente: [Unity 6000.6, Enable collision detection for tiles](https://docs.unity.com/en-us/engine/6000.6/manual/unity2d/tilemaps/tilemap-collider-2d/2d).

Los modos `Grid`, `None` y `Sprite` especifican respectivamente contorno de celda, ausencia de colisión y geometría del sprite. Fuente: [Unity, Tile.ColliderType](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Tilemaps.Tile.ColliderType.html).

Además se comprobaron los XML de referencia instalados junto al Editor 6000.6.8f1: `UnityEngine.TilemapModule.xml` y `UnityEngine.Physics2DModule.xml`, en `Editor/Data/Managed/UnityEngine/`. Existen `Collider2D.compositeOperation` y `Collider2D.CompositeOperation.Merge` (unión geométrica). Conviene usar esas APIs de Unity 6 y no copiar sin comprobar el antiguo `usedByComposite`. La DLL `UnityEngine.TilemapModule.dll` está instalada; declarar el módulo/paquete necesario en el proyecto sigue siendo un paso de integración.

## 3. Arquitectura recomendada para LRH

Lo siguiente es una propuesta propia, basada en los componentes anteriores:

1. **Datos del mapa:** dimensiones, tamaño de celda, materiales y lista de objetos fijos con coordenadas. Un recurso serializado debe conservar la composición entre ejecuciones.
2. **Terreno:** capas de agua, tierra y transiciones de costa. Una paleta reutiliza texturas locales; una costa no debe ser una foto que decide por separado dónde chocan los barcos.
3. **Objetos:** roca, isla, muelle y batería con sprite, pivot y huella de colisión. Un objeto decorativo debe declarar explícitamente si bloquea navegación o disparos.
4. **Minimapa:** dibujarlo desde las mismas celdas y huellas, o con una cámara que observe esas capas. No mantener una ilustración del minimapa diferente al mundo. Esto es una decisión de arquitectura, no una función automática de `Tilemap`.
5. **Navegación:** validar que inicios, ruta de escolta y objetivos pertenecen a zonas alcanzables. La física impide atravesar tierra; no proporciona por sí sola búsqueda de rutas alrededor de islas para la IA.
6. **Editor:** pinceles de material y colocación de objetos sobre el recurso; guardar, regenerar y probar el mapa. Evitar modificar solo instancias temporales que desaparecen al salir de Play.

## 4. Errores que se deben eliminar y comprobaciones

En la implementación previa inspeccionada, `OpenSea` dibuja una imagen de región de 96 × 64 unidades y añade círculos/polígono aproximados. Por tanto la colisión y el terreno visible proceden de definiciones distintas; compartir esa imagen con el minimapa no corrige la separación.

La nueva implementación debe verificar: agua sin colisión, terreno sólido en su celda visible, roca que detiene un barco, proyectil que impacta una barrera, pasos navegables del ancho suficiente, minimapa alineado con objetos, edición persistida y spawns sin superposición con tierra. La revisión visual debe usar la build recién compilada y registrar su ruta: una captura de una build anterior no demuestra el estado actual.
