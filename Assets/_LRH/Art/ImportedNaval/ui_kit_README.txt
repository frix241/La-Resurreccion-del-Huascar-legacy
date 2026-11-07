LA RESURRECCIÓN DEL HUÁSCAR — NAVAL UI KIT
============================================
Sprites PNG RGBA con transparencia real, derivados de tres atlas ilustrados en el estilo naval solicitado.

CARPETAS
01_botones: cuatro estados por control — normal, hover, pressed y disabled; un sprite_strip de cuatro estados en ese orden.
02_paneles: tarjetas de misión, marco de previsualización, minimapa, tooltip, pausa/créditos y otras superficies.
03_hud: barra superior, barras de vida del jugador y convoy (completa, media, crítica, vacía), sliders y marcadores.
04_iconos: 20 iconos tácticos más variantes suplementarias.
05_marca_y_fondos: logo textual y fondo panorámico 2048x1152.
06_atlases: los tres atlas originales, 1536x1024 con canal alfa real.

INTEGRACIÓN UNITY
- Texture Type: Sprite (2D and UI).
- Alpha Is Transparency: activado.
- Para usar un botón: crea un GameObject UI Button y asigna su sprite normal; en Sprite Swap asigna las variantes hover, pressed y disabled.
- Para escalar botones y paneles: Image Type: Sliced; define Borders manualmente en Sprite Editor, respetando ornamentos/cantos; deja la parte central deformable. Como inicio prueba bordes 12-18% del ancho y 20-28% del alto.
- Agrega TextMeshPro como elemento hijo; el texto NO está impreso en los botones.
- Canvas Scaler: Scale With Screen Size, Reference Resolution 1920x1080.
- Fondo de menú: Raw Image o UI Image en 2048x1152 y Preserve Aspect.
- Los estados de barras de vida pueden intercambiarse o usarse como referencia estética para una barra con Fill Amount.
- El marco de minimapa y el marco de previsualización con ventana transparente admiten un RawImage/RenderTexture detrás.

NOTA DE RESOLUCIÓN
Los originales ilustrados son atlas de 1536x1024; las piezas recortadas se reescalaron para producir assets de interfaz de hasta 2048px y los iconos hasta 512px. Este reescalado no añade detalle nativo. Algunos controles fueron compuestos para hacerlos útiles en Unity. El logo y el fondo son composiciones originales preparadas a 2048px. No se garantiza que todas las piezas puedan usarse como 9-slice sin ajustar sus bordes.
