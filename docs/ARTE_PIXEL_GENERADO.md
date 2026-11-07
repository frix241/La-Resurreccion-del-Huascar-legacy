# Arte pixel generado — 7 de noviembre de 2026

Se retoma el alcance del plan maestro: tres misiones, Huáscar, patrullera, cañonera, convoy y baterías costeras. La nueva dirección sustituye la presentación de los elementos principales por imágenes raster con píxeles definidos, paleta naval azul/teal, marfil y latón. No añade mecánicas ni cambia objetivos.

## Recursos y consumo

`Assets/_LRH/Resources/Pixel/` contiene barcos cenitales con proa hacia arriba, batería, isla, proyectil, mar, fondo de menú, logo, carta táctica y seis superficies de interfaz. `Visuals`, `UiVisuals` y `OceanVisuals` cargan este set en ejecución. El kit de botones incluye normal, hover, pulsado y deshabilitado, con bordes 9-slice de 64 px. Los textos funcionales continúan siendo texto de Unity para conservar legibilidad. Se conservan los recursos anteriores; no se borraron ni sobrescribieron.

Los atlas originales están en `Assets/_LRH/Art/GeneratedPixel/`. Se separaron con ImageMagick; se conservó el canal alfa y se ajustaron los sprites al tamaño de juego. Los barcos usan lienzo 256 × 512 y el mar/menú/carta 1536 × 1024. Filtro Point para conservar bordes definidos. La escala de los barcos está ajustada a la vista superior y a sus colliders existentes. La espuma generada se usa en la estela y proa, y las sombras usan la silueta del barco. Se retiró el marco antiguo que ocultaba la carta táctica. Algunos adornos anteriores continúan utilizándose.

## Generación y prompts

Modo utilizado: herramienta integrada `image_gen`, sin CLI ni clave API. Seis generaciones independientes; especificaciones de los prompts:

1. **Flota / stylized-concept:** atlas transparente 1536×1024, cuatro columnas y dos filas. Huáscar monitor con torreta frontal redonda, patrullera, cañonera, carguero; segunda fila batería costera, isla árida peruana, proyectil dorado, estela. Barcos cenitales con proa arriba. Píxeles nítidos, detalle medio, hierro carbón, cubierta marfil, latón y teal; sin letras ni marcas.
2. **UI / stylized-concept:** atlas 1536×1024 en tres columnas y dos filas, celdas iguales sin separación: panel azul oscuro, botón teal normal, hover más luminoso, pulsado oscuro, deshabilitado pizarra y HUD azul. Marcos escalonados de latón, centros despejados para texto vivo, sin letras ni iconos.
3. **Menú / stylized-concept:** ilustración pixel art del Huáscar en el tercio derecho, mar Pacífico teal, costa desértica ocre, luz de latón y espuma marfil. Mitad izquierda y parte superior oscuras y tranquilas para interfaz. Sin texto ni logo.
4. **Mar / stylized-concept:** textura cenital 1536×1024 de océano Pacífico azul teal oscuro, corrientes anchas discretas y escasa espuma en trazos escalonados. Sin horizonte, costa, objetos ni punto focal; bajo contraste para barcos y proyectiles.
5. **Carta / stylized-concept:** mapa táctico cenital 1536×1024, mar azul teal, cuadrícula de latón tenue, costa árida peruana a la derecha, islas pequeñas y rosa de los vientos arriba a la izquierda. Sin etiquetas, barcos ni marcadores de objetivo; estos se dibujan en ejecución.
6. **Logo / logo-brand:** emblema transparente con texto exacto «LA RESURRECCIÓN» y «DEL HUÁSCAR», letras serif de píxeles marfil con borde latón y sombra azul, pequeña brújula encima. Sin palabras adicionales.

No atribuir estos diseños generados a ilustradores humanos ni describirlos como reconstrucciones históricas exactas.

## Verificación

- Núcleo: `dotnet run --project Tests/CoreChecks/CoreChecks.csproj`: **33/33 PASS**.
- Build Linux: `Build/Artifacts/LRH-generated-pixel.x86_64`; registro `Build/Artifacts/unity-generated-pixel-build.log`.
- Captura reproducible: ejecutar esa build con `--lrh-capture-dir /ruta/absoluta`. Recorre menú, briefing, pausa y combate de misión 1, guarda PNG y sale. El modo normal no activa ese recorrido.
- Esto no acredita una partida completa de las tres misiones ni una revisión manual de todos los estados y resoluciones.

Resultado de la revisión visual: se capturaron las cuatro pantallas con el ejecutable real. La revisión detectó y corrigió el marco superpuesto del minimapa, el tinte excesivo del HUD y las sombras rectangulares. La compilación final registró `Build Finished, Result: Success.`. Capturas en `Build/Artifacts/generated-pixel-captures/`. No se repitieron los tests Unity EditMode/PlayMode en esta sesión.
