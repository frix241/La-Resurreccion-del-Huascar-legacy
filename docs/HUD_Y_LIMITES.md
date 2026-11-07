# HUD, límites y créditos — 2026-11-07

## Cambios

- Región limitada a X −48…48, Y −32…32: cuatro paredes físicas, margen de movimiento según la huella del casco y cámara restringida a la región. Una línea de operaciones señala el contorno.
- El objetivo indicado se calcula de nuevo comparando distancias al Huáscar: enemigos vivos y convoy aliado cuando existe. No depende del orden de creación. Los destruidos se descartan y se retiran sus marcadores y halos.
- HUD dividido en flota aliada, objetivos, contactos enemigos, misión actual, carta táctica y controles. Barras proporcionales a vida real, con colores para jugador, convoy y enemigos; vida baja del Huáscar en rojo.
- Objetivos con recuento de enemigos neutralizados y casillas; misión 2 muestra avance de ruta y requisitos de escolta. Los nombres de objetivos se conservan aunque desaparezca su actor.
- Superficies redondeadas con degradado y borde discreto compartidas por botones, tarjetas, barras y volumen. Volumen con pista, relleno, control arrastrable y porcentaje, conectado al ajuste real.
- Tipografía DejaVu Sans incluida localmente bajo su licencia en `Resources/Fonts/LICENSE.txt`. Canvas usa escalado Expand para conservar el área de diseño; textos con ajuste y truncamiento vertical dentro de su rectángulo.
- Créditos: MauroSullcaMamani, SrEstrada, ChristianAlexYana y frix241. Sin correos, identificadores provisionales ni la frase sobre ilustraciones SVG.
- Batería reemplazada por un sprite cenital de un emplazamiento circular; se retiró el cañón rectangular superpuesto. Se conserva el sprite anterior.

## Asset generado

Herramienta integrada `image_gen`, sin CLI. Fuente: `Art/GeneratedPixel/battery-v2-source.png`; recurso de juego: `Resources/Pixel/battery-v2.png`.

Prompt: un único emplazamiento circular de artillería costera, vista estrictamente cenital, anillo de piedra ocre/gris con cañón de hierro y montaje de latón apuntando arriba. Silueta compacta, formas legibles, pixel art pintado con sombreado suave y detalle moderado. Fondo transparente, objeto completo y sin paisaje, texto, bandera ni interfaz. Preparación a 512 × 512 con alfa preservado.

## Verificación

- Unity PlayMode: **18/18 PASS**, en `Build/Artifacts/hud-playmode.xml`. Cobertura nueva: casco detenido en el límite, encuadre de cámara limitado, cambio de objetivo por proximidad y muerte, progreso del checklist, relleno de vida tras daño, interacción por clic del volumen y créditos sin correos ni SVG.
- Build actual: `Build/Artifacts/LRH-terrain.x86_64`; lanzador `Build/JUGAR_LRH.sh`.
- Registro de compilación: `Build/Artifacts/unity-hud-build.log`.
- Capturas: `Build/Artifacts/hud-captures/`, incluido menú, créditos, combate, escolta y batería.

La navegación básica de IA alrededor de obstáculos mantiene los límites documentados del sistema anterior. Los nombres de usuario no implican asignación de funciones o autorías individuales que el equipo no haya confirmado.

Se revisaron capturas reales de menú, combate, baterías y créditos: barras visibles y proporcionales, textos dentro de tarjetas y control de volumen con porcentaje. Se corrigió además la posición inicial del indicador al reanudar y se reservó un margen dinámico para evitar los paneles laterales. Compilación final: `Build Finished, Result: Success.`.
