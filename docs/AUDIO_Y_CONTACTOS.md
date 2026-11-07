# Audio y contacto entre barcos — 2026-11-07

## Corrección de contacto

La prueba `PlayerCannotPushAlliedOrEnemyHull` reprodujo la incidencia antes de modificar la física: desplazamiento vertical del enemigo de 6,14 unidades y del aliado de 2,52. Evidencia roja: `Build/Artifacts/no-push-red.xml`.

Los barcos utilizaban cuerpos dinámicos: los contactos transmitían desplazamiento al casco vecino. Ahora cada casco es cinemático, mantiene contactos cinemáticos y comprueba el recorrido con un cast antes de mover. El giro comprueba la huella resultante. Un contacto bloquea el avance en lugar de empujar otro barco. El convoy aplica la misma comprobación a su ruta. Se mantienen límites, colisiones de terreno y proyectiles.

La prueba de regresión conserva ambos casos (aliado en ruta y enemigo estacionario) y exige menos de 0,08 unidades de desviación provocada por contacto.

## Audio integrado

Se sustituyeron los dos tonos de ejecución por diez archivos de audio en `Assets/_LRH/Resources/Audio/`:

- `cannon.wav`, `impact.wav`, `explosion.wav`, `splash.wav`, `ui_click.wav`.
- `victory.wav`, `defeat.wav`.
- `menu_theme.ogg`: tema de menú, 53,33 s, estéreo.
- `battle_theme.ogg`: tema de combate, 41,74 s, estéreo.
- `ocean_ambience.ogg`: ambiente marino, 12 s, estéreo.

`SoundFx` carga estos recursos. El menú y combate eligen temas distintos con fundido; pausa reduce la música; victoria/derrota sustituyen el tema por un aviso musical. El mar suena en bucle. Cañón, daño, destrucción, proyectil que cae al agua al agotarse y botones disparan efectos. El volumen general existente controla todas las fuentes mediante AudioListener.

El generador reproducible está en `Tools/generate_audio.py` (Python, NumPy y ffmpeg). Los archivos se produjeron por síntesis propia, sin muestras de terceros. No se añadieron menciones de audio a la pantalla de créditos ni al archivo de créditos de assets, siguiendo la petición del usuario.

Se consultaron los paquetes oficiales [Impact Sounds](https://kenney.nl/assets/impact-sounds) e [Interface Sounds](https://kenney.nl/assets/interface-sounds), publicados como CC0. La descarga oficial devolvió 403; no se incorporó ningún archivo de esos paquetes y no se atribuye a Kenney el audio usado.

## Validación

- **21/21 PlayMode PASS**, `Build/Artifacts/audio-no-push-playmode.xml`: regresión de empujón, colisiones, combate, HUD y nuevos casos de clips con muestras finitas/no silenciosas y transiciones de música entre pantallas.
- Análisis de los diez archivos decodificados: muestras finitas, energía no nula y picos por debajo de 1. Manifiesto con duración, canales, pico, RMS y SHA-256: `Build/Artifacts/audio-manifest.json`.
- Build: `Build/Artifacts/LRH-terrain.x86_64`, lanzador `Build/JUGAR_LRH.sh`; compilación registrada en `Build/Artifacts/unity-audio-no-push-build.log`.

Los contactos no añaden búsqueda de rutas a la IA. Un barco detenido por otro debe rodearlo; el empujón deja de ser un mecanismo para abrir paso.

La build se ejecutó con el recorrido automático y terminó con código 0. Registro: `Build/Artifacts/audio-no-push-player.log`. No se detectaron fallos de carga de clips o inicialización de audio; el registro conserva avisos anteriores de generación de contornos de sprites que no pertenecen a este cambio de audio.
