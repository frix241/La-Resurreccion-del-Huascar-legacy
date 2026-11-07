# Terreno modular y editor — 2026-11-07

## Cambio de construcción

La imagen panorámica con colliders aproximados se sustituyó por un mapa serializado de celdas y objetos. `Resources/Maps/pacific.json` contiene 96 × 64 celdas de una unidad (`~` mar, `:` bajío, `.` arena sólida, `#` roca sólida), origen y objetos con posición, escala y bloqueo. El terreno permanece fijo en coordenadas del mundo. El agua continúa fuera de la región con textura repetible y UV fijas.

`OpenSea` construye un Grid y Tilemap reales. Cada material tiene dos texturas raster. La superficie visible mezcla mar, bajío, arena y roca mediante un shader alimentado por un mapa de pesos generado desde las mismas celdas; evita los bordes de cuadrícula abruptos. El Tilemap conserva la geometría y las colisiones, y su renderer se desactiva para no duplicar la superficie. Agua y bajíos no colisionan; arena y roca usan geometría de celda recortada para su forma física. La transición visual ocupa parte de la celda vecina para representar la orilla húmeda. TilemapCollider2D y CompositeCollider2D fusionan las celdas sólidas. Los objetos decorativos tienen sprites propios; los bloqueantes añaden collider estático. La huella física de estos objetos es una forma simple configurable por tipo, no una máscara exacta por píxel.

El minimapa se genera de las mismas celdas y posiciones de objetos. Es una representación táctica por colores, sin ilustración geográfica independiente. Las baterías son cinemáticas; los proyectiles vuelan sobre el suelo del Tilemap y pueden impactar las baterías. Los objetos sólidos como rocas y muelles sí detienen proyectiles.

## Editar

1. Abrir Unity y elegir **LRH > Editor de terreno**.
2. Pintar mar, bajío, arena o roca con clic/arrastre.
3. En **Objetos fijos**, elegir roca, arrecife, arbusto, palmera, muelle, ruinas, acantilado o cajas; configurar escala y colisión y colocar con clic. Clic derecho elimina objetos próximos.
4. **Guardar mapa** escribe el JSON fuente. **Vista previa en Scene** reconstruye el terreno desde ese archivo. La vista previa es temporal; el JSON es el entregable que se guarda y del que se reconstruye cada misión.
5. Iniciar Play o volver a compilar. El minimapa se reconstruye desde el mismo mapa guardado.

Esto es un editor de terreno 2D específico de LRH; no replica las herramientas de triggers, scripting, altura 3D ni toda la funcionalidad de Warcraft III. La IA conserva sus estrategias actuales y no incluye búsqueda de rutas alrededor de todos los obstáculos. No se afirma que todos los mapas pintados sean completables: el autor debe conservar rutas y zonas de aparición navegables.

## Assets nuevos

Herramienta integrada de generación de imágenes de OpenAI, sin CLI y sin imágenes externas como referencia. Fuentes en `Art/GeneratedPixel/terrain-materials.png` y `terrain-props.png`; recortes en `Resources/Terrain/`.

- Prompt de materiales: atlas 1536×1024, cuatro columnas y dos filas, materiales cenitales repetibles sin bordes ni letras: dos mares oscuros teal, dos bajíos turquesa, dos arenas ocres y dos rocas gris/marrón. Estilo RTS pintado, detalle moderado, sin escenarios ni objetos.
- Prompt de objetos: atlas transparente 1536×1024, cuatro columnas y dos filas; rocas, arrecife arenoso, arbustos, palmeras, muelle vertical, muro en ruinas, afloramiento rocoso y cajas. Siluetas claras, sombreado suave, sin texto ni fondo de suelo. Preparación con ImageMagick preservando alfa.

## Verificación

Unity PlayMode: **13/13 PASS** (`Build/Artifacts/terrain-playmode.xml`), incluyendo detención real de un casco que avanza contra una isla, coincidencia de las celdas sólidas con el CompositeCollider y terreno inmóvil cuando se mueve la cámara. Se conservan las pruebas anteriores de combate, pausa, progreso e indicador del convoy.

Ejecutable actual: `Build/Artifacts/LRH-terrain.x86_64`. Usar **`Build/JUGAR_LRH.sh`** para evitar abrir las builds anteriores. Registro: `Build/Artifacts/unity-terrain-build.log`. Capturas: `Build/Artifacts/terrain-captures/`.

Investigación y fuentes oficiales: [research_terrain_editor.md](research_terrain_editor.md). No se copió arte de Warcraft.

Revisión visual final: se verificó la costa con el ejecutable real, incluyendo muelle y vegetación; se corrigió el orden de dibujo para que el mar no tape el suelo y se usa repetición reflejada de materiales para disimular juntas de textura. Última repetición de PlayMode: 13/13 PASS. La vista previa del editor se elimina antes de entrar a Play y no se guarda en las escenas.
