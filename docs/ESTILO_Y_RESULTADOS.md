# Estilo unificado y pantallas de resultado — 2026-11-07

## Dirección visual

Se adopta el lenguaje dominante del terreno y sus objetos: RTS ilustrado de detalle medio, pincelada visible, sombras suaves y paleta azul/teal, ocre, marfil y latón. Se retiran del consumo del menú el logo de píxeles duros y la ilustración anterior de mayor detalle. Los cuatro barcos ahora tienen siluetas pintadas simples, acordes con las rocas y muelles del mapa. No cambian sus tamaños de lienzo, unidades físicas, colliders o balance.

Los paneles y estados de botones usan texturas pintadas con borde de latón, centros oscuros y grano discreto. Las barras conservan los colores funcionales y reciben textura sutil para integrarse con las superficies. La tipografía mantiene DejaVu Sans para opciones y datos; títulos de campaña y resultado usan DejaVu Serif Bold, bajo la licencia ya incluida en `Resources/Fonts/LICENSE.txt`.

## Resultado y opciones

- Victoria: fondo propio de llegada al puerto con luz cálida; título, explicación e informe de combate en una tarjeta de la izquierda. «Continuar campaña» abre la introducción de la siguiente misión desbloqueada; también se puede repetir o volver al menú.
- Derrota: fondo propio del Huáscar averiado en mar de tormenta. El mensaje distingue destrucción del Huáscar y pérdida del convoy. Opciones «Reintentar misión» y «Volver al menú».
- Menú: títulos de misiones más breves, opciones en estilo oración («Salir del juego», «Créditos», «Iniciar misión», «Continuar partida»). Los botones reservan espacio interior para que sus textos no toquen los marcos.
- Los textos funcionales siguen siendo texto real, no letras incrustadas en imágenes.

## Imágenes generadas

Modo: herramienta integrada `image_gen`, sin CLI ni referencias externas. Todas las imágenes seleccionadas están guardadas en el proyecto. Las versiones anteriores se conservan.

| Recurso en `Assets/_LRH/Resources/Artstyle/` | Uso |
|---|---|
| `victory-background.png` | Fondo de victoria |
| `defeat-background.png` | Fondo de derrota |
| `menu-background.png` | Fondo de menú y pantallas de campaña |
| `huascar.png`, `patrol.png`, `gunboat.png`, `convoy.png` | Barcos cenitales pintados |
| `panel.png`, `normal.png`, `hover.png`, `pressed.png`, `disabled.png`, `wood.png` | Kit de UI; madera conservada como variante disponible |

Fuentes de los dos atlas: `Assets/_LRH/Art/GeneratedPixel/painted-ships-atlas.png` y `painted-ui-atlas.png`. Los recortes preservan alfa; los barcos conservan lienzo 256 × 512 y 256 píxeles por unidad. Los paneles son 512 × 512 con bordes 9-slice de 48 px y 200 píxeles por unidad para controlar la escala del marco.

## Prompts

Especificaciones del conjunto generado, con el mismo estilo y paleta en las cinco llamadas:

1. **Victoria:** fondo 1536×1024, RTS ilustrado pintado, Huáscar con torreta circular/chimenea/dos mástiles a la derecha llegando a puerto del Pacífico con un convoy distante y luz cálida de amanecer. Mitad izquierda de mar azul oscuro tranquila para UI. Sin texto ni logo.
2. **Derrota:** fondo 1536×1024, mismo acabado y paleta; Huáscar averiado y escorado a la derecha, mástil roto, humo y brasas discretas, mar de tormenta y costa ocre distante. Mitad izquierda oscura y despejada. Sin texto, personas ni gore.
3. **Menú:** fondo 1536×1024; Huáscar protagonista en mitad derecha, silueta simple y épica, oleaje teal, luz de latón y costa peruana árida. Pincelada visible, sin fotorealismo ni microdetalle; mitad izquierda oscura para título y opciones.
4. **Barcos:** atlas transparente 1536×1024 de 2 × 2 celdas: Huáscar, patrullera, cañonera y convoy. Vista estrictamente cenital con proa arriba; formas claras, sombra suave, cubiertas marfil, cascos carbón y herrajes de latón. Sin mar, estelas, sombras exteriores, texto ni bordes. Detalle moderado y sin contornos de píxel negro grueso.
5. **UI:** atlas transparente 1536×1024 de 3 × 2 celdas: panel azul oscuro, botón normal teal, hover más claro, pulsado oscuro, deshabilitado pizarra y variante de madera. Rectángulos redondeados, borde fino de latón envejecido, centros oscuros con grano pintado discreto; sin texto ni iconos.

## Verificación

**23/23 PlayMode PASS** (`Build/Artifacts/artstyle-results-playmode.xml`), incluyendo fondos distintos de victoria/derrota, continuación de campaña hacia misión 2 desbloqueada y reintento que restaura la misión. Se mantienen las regresiones anteriores de vida, límites, colisiones sin empujón, objetivos y audio.

Build actual: `Build/Artifacts/LRH-terrain.x86_64`, lanzador `Build/JUGAR_LRH.sh`. Registro: `Build/Artifacts/unity-artstyle-results-build.log`. El recorrido visual ahora provoca victoria y derrota reales, además de pérdida del convoy, con repositorio de progreso en memoria; capturas en `Build/Artifacts/artstyle-results-captures/`.
