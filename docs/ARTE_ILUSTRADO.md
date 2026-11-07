# Dirección de arte ilustrada

## Lenguaje visual

- Salida final a **1920 × 1080** sin reducción a una cuadrícula de píxeles.
- Ilustración 2D pintada: contornos entintados, formas legibles, pinceladas amplias y textura contenida en mar, costa, barcos y objetos.
- Paleta común de azul noche, verde mar, pizarra, ocre, latón y crema para mantener unidos el escenario, el menú y la interfaz.
- Una sola fuente activa: `Resources/Fonts/Interface.ttf`. Los textos de menú, HUD, misión y resultados usan el mismo tipo y jerarquía.
- Paneles y botones usan sprites raster redondeados de nueve sectores. Los fondos de menú, victoria y derrota dejan espacio oscuro a la izquierda para los textos.

## Recursos

Los **33 PNG** finales están en `Assets/_LRH/Resources/Retro/`; las fuentes y hojas originales, en `Assets/_LRH/Art/RetroSource/`.

| Familia | Cantidad | Dimensiones finales |
|---|---:|---|
| Fondos de menú, victoria y derrota | 3 | 1920 × 1080 |
| Huáscar, patrullera, cañonera y convoy | 4 | 192 × 384 |
| Batería e isla | 2 | 256 × 256 |
| Proyectil y estela | 2 | 96 × 96 y 192 × 192 |
| Agua, bajío, arena y roca, dos variantes | 8 | 384 × 384 |
| Rocas, arrecife, arbusto, palmeras, muelle, ruinas, acantilado y cajas | 8 | 256 × 256 |
| Panel y cinco estados de botones | 6 | 256 × 256 |

`Tools/build_retro_art.py` recorta las hojas, conserva proporciones y prepara imágenes a tamaño de uso. `Tools/build_retro_ui.py` crea el atlas de seis estados con bordes y ornamentos coherentes. Ambos mantienen los GUID de Unity y usan filtrado bilinear.

## Revisión

- La compilación Linux termina correctamente y el juego captura menú y combate a 1920 × 1080 en `Build/Artifacts/retro-captures/`.
- Las **23 pruebas PlayMode pasan** (`Build/Artifacts/illustrated-playmode.xml`).
- El HUD muestra vida, objetivos, contactos, carta táctica y controles; el tamaño de los barcos se ajustó para que el Huáscar destaque en la vista de juego.
- El mapa conserva su terreno, límites y colisiones; la cámara sigue al Huáscar y el indicador apunta al objetivo cercano.
