# Créditos, procedencia y licencias de recursos — LRH

Este registro se completará **antes** de presentar una build, publicar capturas o distribuir arte. Los SVG en `Assets/_LRH/Art/SVG/` se conservan como fuentes editables, mientras que los PNG en `Assets/_LRH/Resources/Sprites/` son raster RGBA de alta resolución con gradientes, iluminación, ruido/material, reflejos y sombras generados localmente; no se incorporó material externo ni se copió una obra concreta. La autoría nominal, la herramienta y la fecha formal de producción siguen pendientes de confirmación con el equipo. No atribuirlos a una persona por su rol propuesto.

Esta iteración añade dos paquetes PNG proporcionados por el usuario: el paquete naval (`naval_strategy_assets_9_png.zip`) y el kit de UI (`naval_ui_kit_unity.zip`). Se copiaron a `Assets/_LRH/Resources/Sprites/`, `Assets/_LRH/Resources/UI/` y `Assets/_LRH/Art/ImportedNaval/` conservando sus README de procedencia. La integración usa los PNG suministrados como recursos runtime; no se atribuyen a terceros ni se redistribuyen fuera de este proyecto sin confirmar la autorización del equipo.

| Recurso / uso previsto | SVG fuente en el proyecto | PNG de trabajo en el proyecto | Autor real y licencia | Fecha / método de creación o conversión |
|---|---|---|---|---|
| Monitor Huáscar | `Assets/_LRH/Art/SVG/huascar.svg` | `Assets/_LRH/Resources/Sprites/huascar.png` | PNG suministrado por el usuario; autorización/procedencia formal pendiente | PNG RGBA 2048×2048; fuente SVG editable conservada |
| Patrullera | `Assets/_LRH/Art/SVG/patrol.svg` | `Assets/_LRH/Resources/Sprites/patrol.png` | PNG suministrado por el usuario; autorización/procedencia formal pendiente | PNG RGBA 2048×2048; fuente SVG editable conservada |
| Cañonera | `Assets/_LRH/Art/SVG/gunboat.svg` | `Assets/_LRH/Resources/Sprites/gunboat.png` | PNG suministrado por el usuario; autorización/procedencia formal pendiente | PNG RGBA 2048×2048; fuente SVG editable conservada |
| Convoy | `Assets/_LRH/Art/SVG/convoy.svg` | `Assets/_LRH/Resources/Sprites/convoy.png` | PNG suministrado por el usuario; autorización/procedencia formal pendiente | PNG RGBA 2048×2048; fuente SVG editable conservada |
| Batería costera | `Assets/_LRH/Art/SVG/battery.svg` | `Assets/_LRH/Resources/Sprites/battery.png` | PNG suministrado por el usuario; autorización/procedencia formal pendiente | PNG RGBA 2048×2048; fuente SVG editable conservada |
| Isla/costa | `Assets/_LRH/Art/SVG/island.svg` | `Assets/_LRH/Resources/Sprites/island.png` | PNG suministrado por el usuario; autorización/procedencia formal pendiente | PNG RGBA 2048×2048; fuente SVG editable conservada |
| Proyectil | `Assets/_LRH/Art/SVG/projectile.svg` | `Assets/_LRH/Resources/Sprites/projectile.png` | PNG suministrado por el usuario; autorización/procedencia formal pendiente | PNG RGBA 2048×2048; fuente SVG editable conservada |

**Audio procedural:** `Assets/_LRH/Scripts/Runtime/SoundFx.cs` sintetiza en memoria tonos de cañón e impacto mediante `AudioClip.Create`; **no hay muestra externa en estos efectos**. Autor/a real del código y fecha: **[pendientes de confirmar]**. No afirmar que el sonido se oyó, que su volumen funciona o que el resultado está libre de problemas hasta probarlo en Unity Editor. La pantalla `GameUi.ShowCredits` todavía muestra DEV-1…DEV-4: reemplazar únicamente con personas y aportes comprobados antes de publicar.

## Ficha que debe completar quien incorpora un recurso

- **Ruta local y uso:** [ruta, misión, pantalla].
- **Creador y participantes reales:** [nombres o usuarios autorizados]. Si es trabajo propio en equipo, identificar quién dibujó y quién convirtió; obtener autorización del autor para distribución del repositorio/build.
- **Si procede de terceros:** [nombre del autor], [enlace permanente al recurso], [licencia exacta y enlace], [fecha de consulta], [requisitos de atribución] y [cambios efectuados]. Una imagen histórica o encontrada en Internet **no** es automáticamente libre de derechos.
- **Proceso raster actual:** generación local de PNG RGBA de alta resolución con gradientes, iluminación, textura y sombras; los SVG se conservan como fuentes editables de referencia. Confirmar herramienta y fecha formal con el equipo antes de publicar.
- **Audio, fuentes, logos y paquetes adicionales:** registrar con la misma ficha **antes de añadirlos**; no se listan aquí recursos ajenos no comprobados.

**Aviso de ficción para créditos del juego:** «Obra de ficción inspirada en un contexto histórico. Los eventos y enfrentamientos del juego no son acontecimientos históricos reales». El aviso tampoco reemplaza los permisos sobre imágenes o música.

## Set pixel generado (2026-11-07)

Imágenes generadas con la herramienta integrada de generación de imágenes de OpenAI para este proyecto: flota, costa, proyectil, mar, menú, carta táctica, logo y superficies UI. Preparación con ImageMagick e integración Unity. No se usaron imágenes externas como referencia en esta generación. Rutas, prompts y límites: [ARTE_PIXEL_GENERADO.md](ARTE_PIXEL_GENERADO.md). Los assets previos se conservan y mantienen sus créditos originales.

### Revisión suave / mar abierto

Tres imágenes adicionales generadas con la herramienta integrada de OpenAI (menú épico, atlas UI redondeado y región costera); sin referencias externas. Integración y prompts en [MEJORAS_MAR_ABIERTO.md](MEJORAS_MAR_ABIERTO.md).

### Terreno modular RTS

Materiales de mar/bajío/arena/roca y ocho objetos generados con la herramienta integrada de OpenAI, preparados para el mapa por celdas. Sin arte de Warcraft ni referencias visuales externas. Rutas y especificaciones de prompts: [TERRENO_MODULAR.md](TERRENO_MODULAR.md).

### Batería cenital y tipografía de interfaz

Batería circular generada con la herramienta integrada de OpenAI, sin referencias externas; ruta y prompt en [HUD_Y_LIMITES.md](HUD_Y_LIMITES.md). Fuente DejaVu Sans incluida bajo su licencia original, conservada en `Assets/_LRH/Resources/Fonts/LICENSE.txt`.

### Estilo pintado y fondos de resultado

Fondos de victoria, derrota y menú, atlas de barcos y atlas UI generados con la herramienta integrada de imágenes de OpenAI para LRH, sin imágenes externas como referencia. Recursos, especificaciones de prompts y consumo: [ESTILO_Y_RESULTADOS.md](ESTILO_Y_RESULTADOS.md). La variante DejaVu Serif Bold usa la licencia conservada en `Resources/Fonts/LICENSE.txt`.

### Rediseño pixel art de 16 bits

Conjunto activo de 33 imágenes generado con la herramienta integrada de imágenes, normalizado a una paleta común y tamaños nativos de píxel. Fuente Silkscreen del repositorio oficial Google Fonts, bajo la licencia OFL conservada junto al recurso. Prompts, tamaños, rutas y verificación: [ARTE_16_BITS.md](ARTE_16_BITS.md). No se añadieron menciones de audio generado a la pantalla de créditos.
