# Interfaz suave y mar abierto — 2026-11-07

## Cambios

- Nueva imagen de menú: Huáscar protagonista, contraluz cálido, mar agitado; botones y logo colocados a la izquierda. Se retiró el barco pequeño flotante del menú.
- Nuevo atlas UI de paneles redondeados, borde fino de latón y cuatro estados de botón. HUD compacto arriba a la izquierda, pausa separada, minimapa arriba a la derecha y controles centrados dentro del pie. Se retiraron remates cuadrados de botones y HUD.
- Barcos con filtrado bilineal, interpolación de movimiento y sombras menos opacas para suavizar bordes y giros.
- Región costera de 96 × 64 unidades compartida literalmente por el mundo y minimapa, con costa e islas con colisión aproximada. Los barcos ya no se restringen al antiguo rectángulo. Fuera de la región continúa el mar; el minimapa es una carta regional, no un mapa infinito generado.
- Cámara centrada en el Huáscar con seguimiento amortiguado y encuadre más cercano. Se reinicia al iniciar una misión.
- Flecha y distancia al objetivo, especialmente al convoy de la misión 2. Dentro de cámara marca su posición; fuera de cámara queda dentro de márgenes de pantalla y apunta hacia él. El convoy también conserva su marcador en la carta. Su ruta de escolta y condición de llegada se mantienen.
- Las baterías de la misión 3 se trasladan a la zona oriental de la región costera.

## Imágenes y prompts

Herramienta integrada `image_gen`, sin CLI. Nuevos PNG en `Assets/_LRH/Resources/Pixel/*-v2.png`; atlas original en `Assets/_LRH/Art/GeneratedPixel/ui-atlas-v2.png`. Se conservan las versiones anteriores. Especificaciones de los tres prompts:

1. Menú 1536×1024: Huáscar épico grande en mitad derecha, torreta circular, chimenea, dos mástiles, bandera peruana, espuma marfil y rayos de luz dorada bajo nubes de tormenta; mitad izquierda azul oscuro despejada para UI. Ilustración de píxeles con iluminación suave, sin texto ni interfaz.
2. UI 1536×1024 transparente, tres columnas y dos filas: panel, botón normal, hover, pulsado, deshabilitado y HUD; rectángulos de esquinas redondeadas de 48 px, borde fino de latón, azul/teal; centros despejados sin letras ni iconos. Se recortó con ImageMagick preservando alfa.
3. Región 1536×1024: vista cenital con mar azul/teal a la izquierda, costa desértica peruana a la derecha y tres islas; espuma costera marfil, sombreado suave en píxeles. Sin cuadrícula, barcos, etiquetas, brújula ni bordes. La misma imagen se carga en mundo y minimapa.

## Verificación

- Núcleo .NET: **33/33 PASS**.
- Unity PlayMode: **11/11 PASS**, incluidos movimiento más allá del límite anterior, seguimiento de cámara y etiqueta del convoy fuera de pantalla. Evidencia: `Build/Artifacts/open-sea-playmode.xml`.
- Build: `Build/Artifacts/LRH-open-sea.x86_64`; registro `Build/Artifacts/unity-open-sea-build.log`.
- Capturas con repositorio de progreso en memoria, sin alterar el guardado del usuario: menú, briefing, pausa, combate, convoy cercano y convoy fuera de cámara mediante `--lrh-capture-dir`.

Pendiente de aceptación manual: jugar las tres misiones completas, verificar duración de la escolta y recorrer toda la costa. La colisión del terreno aproxima la ilustración; no es una máscara por píxel. La carta regional no se desplaza al salir de sus límites y mantiene los marcadores en el borde.

Revisión de capturas reales: menú con barco protagonista y controles a la izquierda; escolta fuera de cámara con flecha y distancia visibles. Se corrigieron adicionalmente el pie de pantalla para anclarlo al borde inferior y la sombra desalineada del minimapa; la carta ahora se recorta mediante una máscara redondeada. Capturas finales: `Build/Artifacts/open-sea-captures/`.
