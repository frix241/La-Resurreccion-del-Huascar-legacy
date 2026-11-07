# Christian Yana: Arte, interfaz visual y audio

**Responsable:** `ChristianAlexYana` · **Issue:** [#4](https://github.com/frix241/La-Resurreccion-del-Huascar/issues/4) · **Estimación inicial:** 8 puntos.  
**Rama:** `feature/christian-arte-audio` · **Revisor previsto:** `SrEstrada` · **Base del PR:** `master`.

## Resultado que debe entregar la tarea

Como jugador quiero leer la interfaz y reconocer el mundo con un acabado ilustrado consistente, acompañado por sonido que responda a cada acción.

Esta guía divide el trabajo en **7 bloques de commit**. En cada bloque: realiza el cambio indicado, ejecuta sus comprobaciones, revisa el stage y registra ese bloque antes de empezar el siguiente. La lista es un plan de separación: si un comportamiento ya está correcto y no hay diff, registra su comprobación en el issue y continúa. Cada commit debe contener un cambio real y mantener el proyecto utilizable.

## 1. Preparar el entorno y la rama

Necesitas Git, GitHub CLI autenticado con tu cuenta, .NET SDK 10, Python 3 y Unity **6000.6.8f1** para los casos de Editor. Ejecuta los bloques Bash desde la raíz del clon. Comprueba primero identidad y estado:

```bash
git status --short --branch
git config user.name
git config user.email
gh auth status
```

La identidad debe ser la tuya. Si hay trabajo sin confirmar, termínalo o consérvalo antes de cambiar de rama; estos comandos no descartan archivos.

Empieza cuando el PR #5 esté integrado y #1 desbloquee esta tarea. Para un clon nuevo:

```bash
git clone https://github.com/frix241/La-Resurreccion-del-Huascar.git
cd La-Resurreccion-del-Huascar
```

Crea la rama una sola vez:

```bash
git fetch origin
git switch master
git pull --ff-only origin master
git switch -c feature/christian-arte-audio
```

Si ya trabajabas en esa rama, vuelve a ella con `git switch feature/christian-arte-audio`; evita intentar crearla de nuevo.

## 2. Plan de commits

| Bloque | Resultado | Mensaje propuesto |
|---|---|---|
| 1 | Silueta y acabado de la flota | `fix(art): mejorar lectura de barcos y baterías` |
| 2 | Terreno y objetos del mapa | `fix(art): unificar terreno y objetos del mapa` |
| 3 | Paneles, botones y tipografía | `fix(ui): unificar bordes y estados de los paneles` |
| 4 | Fondos de menú y resultados | `feat(art): ajustar fondos de menú y resultados` |
| 5 | Recursos de sonido y música | `feat(audio): ajustar música y efectos de campaña` |
| 6 | Eventos de audio y presentación Runtime | `fix(audio): sincronizar efectos y transiciones de campaña` |
| 7 | Registro de arte y validación | `docs(art): registrar recursos y validación audiovisual` |

Los mensajes siguientes describen el alcance previsto. Adáptalos si el diff final resuelve un caso más específico. Código y regresión del mismo caso viajan juntos.

### Commit 1: Silueta y acabado de la flota

**Objetivo:** Que Huascar, enemigos, convoy y baterías se reconozcan sin manchas o huecos accidentales.

**Trabajo antes del stage:** Trabaja las hojas de barcos/batería y sus exportaciones correspondientes. Comprueba cada sprite dentro del mapa, con rotación y escala reales.

**Comprobaciones de este bloque:**

- Abre Unity y revisa los cinco tipos de actor a su escala de juego.
- Revisa borde del casco, torretas y transparencia; comprueba GUID con Repository checks.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Art/RetroSource/ships.png \
  Assets/_LRH/Art/RetroSource/ships.png.meta \
  Assets/_LRH/Art/RetroSource/battery.png \
  Assets/_LRH/Art/RetroSource/battery.png.meta \
  Assets/_LRH/Resources/Retro/huascar.png \
  Assets/_LRH/Resources/Retro/huascar.png.meta \
  Assets/_LRH/Resources/Retro/patrol.png \
  Assets/_LRH/Resources/Retro/patrol.png.meta \
  Assets/_LRH/Resources/Retro/gunboat.png \
  Assets/_LRH/Resources/Retro/gunboat.png.meta \
  Assets/_LRH/Resources/Retro/convoy.png \
  Assets/_LRH/Resources/Retro/convoy.png.meta \
  Assets/_LRH/Resources/Retro/battery.png \
  Assets/_LRH/Resources/Retro/battery.png.meta
git add -p -- \
  Tools/build_retro_art.py
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "fix(art): mejorar lectura de barcos y baterías"
git show --stat --oneline HEAD
```

### Commit 2: Terreno y objetos del mapa

**Objetivo:** Que la costa, agua y objetos mantengan material y escala coherentes.

**Trabajo antes del stage:** Exporta solo los recursos de terreno que cambiaste y conserva sus metadatos. Revisa uniones entre tiles y bordes de transparencia.

**Comprobaciones de este bloque:**

- Inspecciona uniones de tiles en el mar y la costa.
- Revisa objetos colocados en mapa; confirma que el arte coincide con el área que bloquean.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Art/RetroSource/terrain.png \
  Assets/_LRH/Art/RetroSource/terrain.png.meta \
  Assets/_LRH/Art/RetroSource/props.png \
  Assets/_LRH/Art/RetroSource/props.png.meta \
  Assets/_LRH/Art/RetroSource/ocean-tile.png \
  Assets/_LRH/Art/RetroSource/ocean-tile.png.meta \
  Assets/_LRH/Resources/Retro/water0.png \
  Assets/_LRH/Resources/Retro/water0.png.meta \
  Assets/_LRH/Resources/Retro/water1.png \
  Assets/_LRH/Resources/Retro/water1.png.meta \
  Assets/_LRH/Resources/Retro/sand0.png \
  Assets/_LRH/Resources/Retro/sand0.png.meta \
  Assets/_LRH/Resources/Retro/sand1.png \
  Assets/_LRH/Resources/Retro/sand1.png.meta \
  Assets/_LRH/Resources/Retro/shallow0.png \
  Assets/_LRH/Resources/Retro/shallow0.png.meta \
  Assets/_LRH/Resources/Retro/shallow1.png \
  Assets/_LRH/Resources/Retro/shallow1.png.meta \
  Assets/_LRH/Resources/Retro/stone0.png \
  Assets/_LRH/Resources/Retro/stone0.png.meta \
  Assets/_LRH/Resources/Retro/stone1.png \
  Assets/_LRH/Resources/Retro/stone1.png.meta \
  Assets/_LRH/Resources/Retro/cliff.png \
  Assets/_LRH/Resources/Retro/cliff.png.meta \
  Assets/_LRH/Resources/Retro/rocks.png \
  Assets/_LRH/Resources/Retro/rocks.png.meta \
  Assets/_LRH/Resources/Retro/palms.png \
  Assets/_LRH/Resources/Retro/palms.png.meta \
  Assets/_LRH/Resources/Retro/reef.png \
  Assets/_LRH/Resources/Retro/reef.png.meta \
  Assets/_LRH/Resources/Retro/pier.png \
  Assets/_LRH/Resources/Retro/pier.png.meta \
  Assets/_LRH/Resources/Retro/shrub.png \
  Assets/_LRH/Resources/Retro/shrub.png.meta \
  Assets/_LRH/Resources/Retro/ruins.png \
  Assets/_LRH/Resources/Retro/ruins.png.meta \
  Assets/_LRH/Resources/Retro/crates.png \
  Assets/_LRH/Resources/Retro/crates.png.meta
git add -p -- \
  Tools/build_retro_art.py
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "fix(art): unificar terreno y objetos del mapa"
git show --stat --oneline HEAD
```

### Commit 3: Paneles, botones y tipografía

**Objetivo:** Que la UI tenga bordes completos, estados legibles y una escala tipográfica común.

**Trabajo antes del stage:** Corrige los paneles exportados y el código de tema/presentación que los usa. Incluye fuentes solo si realmente las cambiaste y preserva su licencia.

**Comprobaciones de este bloque:**

- Revisa menú, briefing, HUD, pausa y créditos a 1920×1080.
- Prueba hover, pulsado y desactivado; confirma textos dentro de sus contenedores.
- Verifica que títulos y cuerpo usen las fuentes previstas en UiTheme.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Art/RetroSource/ui.png \
  Assets/_LRH/Art/RetroSource/ui.png.meta \
  Assets/_LRH/Resources/Retro/panel.png \
  Assets/_LRH/Resources/Retro/panel.png.meta \
  Assets/_LRH/Resources/Retro/header.png \
  Assets/_LRH/Resources/Retro/header.png.meta \
  Assets/_LRH/Resources/Retro/normal.png \
  Assets/_LRH/Resources/Retro/normal.png.meta \
  Assets/_LRH/Resources/Retro/hover.png \
  Assets/_LRH/Resources/Retro/hover.png.meta \
  Assets/_LRH/Resources/Retro/pressed.png \
  Assets/_LRH/Resources/Retro/pressed.png.meta \
  Assets/_LRH/Resources/Retro/disabled.png \
  Assets/_LRH/Resources/Retro/disabled.png.meta
git add -p -- \
  Assets/_LRH/Scripts/Runtime/UiTheme.cs \
  Assets/_LRH/Scripts/Runtime/UiVisuals.cs \
  Tools/build_retro_ui.py
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "fix(ui): unificar bordes y estados de los paneles"
git show --stat --oneline HEAD
```

### Commit 4: Fondos de menú y resultados

**Objetivo:** Que victoria y derrota tengan composición y contraste útiles para leer sus opciones.

**Trabajo antes del stage:** Trabaja los tres fondos con sus fuentes. Revisa el área donde queda el texto y los botones; preserva el encuadre de 1920×1080.

**Comprobaciones de este bloque:**

- Captura menú, victoria y derrota en Unity.
- Comprueba que los fondos no dificulten leer continuar, repetir y volver al menú.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Art/RetroSource/menu.png \
  Assets/_LRH/Art/RetroSource/menu.png.meta \
  Assets/_LRH/Art/RetroSource/victory.png \
  Assets/_LRH/Art/RetroSource/victory.png.meta \
  Assets/_LRH/Art/RetroSource/defeat.png \
  Assets/_LRH/Art/RetroSource/defeat.png.meta \
  Assets/_LRH/Resources/Retro/menu-background.png \
  Assets/_LRH/Resources/Retro/menu-background.png.meta \
  Assets/_LRH/Resources/Retro/victory-background.png \
  Assets/_LRH/Resources/Retro/victory-background.png.meta \
  Assets/_LRH/Resources/Retro/defeat-background.png \
  Assets/_LRH/Resources/Retro/defeat-background.png.meta
git add -p -- \
  Tools/build_retro_art.py
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "feat(art): ajustar fondos de menú y resultados"
git show --stat --oneline HEAD
```

### Commit 5: Recursos de sonido y música

**Objetivo:** Que los archivos de audio tengan volumen y duración apropiados para su uso.

**Trabajo antes del stage:** Modifica generador y archivos que realmente regeneraste. Reproduce cada efecto y tema antes de integrarlo; comprueba licencia/procedencia aplicable.

**Comprobaciones de este bloque:**

- Escucha inicio/final de loops y efectos a volumen bajo y alto.
- Conserva los archivos originales si una regeneración no mejora el caso.
- Ejecuta Repository checks con los recursos y .meta staged.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Resources/Audio/cannon.wav \
  Assets/_LRH/Resources/Audio/cannon.wav.meta \
  Assets/_LRH/Resources/Audio/impact.wav \
  Assets/_LRH/Resources/Audio/impact.wav.meta \
  Assets/_LRH/Resources/Audio/explosion.wav \
  Assets/_LRH/Resources/Audio/explosion.wav.meta \
  Assets/_LRH/Resources/Audio/splash.wav \
  Assets/_LRH/Resources/Audio/splash.wav.meta \
  Assets/_LRH/Resources/Audio/ui_click.wav \
  Assets/_LRH/Resources/Audio/ui_click.wav.meta \
  Assets/_LRH/Resources/Audio/victory.wav \
  Assets/_LRH/Resources/Audio/victory.wav.meta \
  Assets/_LRH/Resources/Audio/defeat.wav \
  Assets/_LRH/Resources/Audio/defeat.wav.meta \
  Assets/_LRH/Resources/Audio/menu_theme.ogg \
  Assets/_LRH/Resources/Audio/menu_theme.ogg.meta \
  Assets/_LRH/Resources/Audio/battle_theme.ogg \
  Assets/_LRH/Resources/Audio/battle_theme.ogg.meta \
  Assets/_LRH/Resources/Audio/ocean_ambience.ogg \
  Assets/_LRH/Resources/Audio/ocean_ambience.ogg.meta
git add -p -- \
  Tools/generate_audio.py
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "feat(audio): ajustar música y efectos de campaña"
git show --stat --oneline HEAD
```

### Commit 6: Eventos de audio y presentación Runtime

**Objetivo:** Que música y efectos respondan a menú, combate, pausa y resultado sin duplicarse.

**Trabajo antes del stage:** Corrige SoundFx y solo los puntos visuales de Runtime necesarios para integrar esos recursos. Incluye los casos PlayMode relacionados.

**Comprobaciones de este bloque:**

- Prueba disparo, impacto, pausa, victoria, derrota y retorno al menú.
- Comprueba una única música activa y volumen cero; revisa T25–T26 si afecta a persistencia.
- PlayMode debe registrar su resultado real; CoreChecks por sí solo no reproduce audio.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Runtime/SoundFx.cs
git add -p -- \
  Assets/_LRH/Scripts/Runtime/Visuals.cs \
  Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "fix(audio): sincronizar efectos y transiciones de campaña"
git show --stat --oneline HEAD
```

### Commit 7: Registro de arte y validación

**Objetivo:** Que las capturas y créditos permitan revisar los recursos cambiados.

**Trabajo antes del stage:** Actualiza dirección visual, resultados y atribución aplicable. Incluye en el PR las comparaciones y resultados de audio.

**Comprobaciones de este bloque:**

- Adjunta comparativas a 1920×1080 y pasos reproducibles de audio.
- Revisa la procedencia y licencia de cualquier recurso externo nuevo.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  docs/ARTE_ILUSTRADO.md \
  docs/ESTILO_Y_RESULTADOS.md \
  docs/creditos_assets.md \
  docs/AUDIO_Y_CONTACTOS.md
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "docs(art): registrar recursos y validación audiovisual"
git show --stat --oneline HEAD
```

## 3. Separar cambios que ya están mezclados

Mantén el acabado ilustrado actual a 1920×1080. Los scripts se llaman Retro por historia del proyecto; ese nombre no exige volver a pixel art. Coordina GameUi con Albert y Sergio. Stagea únicamente los recursos exportados para el bloque actual.

En `git add -p`, `y` incluye un fragmento, `n` lo deja para el siguiente commit, `s` intenta dividirlo y `q` termina. Si un fragmento mezcla tareas y no se puede dividir limpiamente, reorganiza el cambio en el editor y vuelve a seleccionar. Revisa siempre `git diff --cached` antes del commit.

Para retirar del stage un archivo sin perder la edición:

```bash
git restore --staged -- Assets/_LRH/Scripts/Runtime/UiTheme.cs
```

Ese comando solo retira ese archivo del stage. Para otro archivo, usa su ruta real. En assets binarios, prepara la exportación de cada bloque por separado: `git add -p` no divide una imagen o un audio. Conserva su `.meta` con el recurso y revisa qué archivos cambió el generador antes de stagearlos.

Si Unity crea un archivo nuevo, inclúyelo junto con su `.meta` en el bloque que lo introduce. Los `.meta` existentes solo cambiarán si el ajuste de importación lo requiere.

## 4. Revisión completa de la rama

Los resultados locales se refieren al árbol de trabajo completo. Si dejaste editados bloques futuros sin incluirlos en el stage, esos resultados no prueban por separado el commit seleccionado. Trabaja un bloque cada vez; para revisar una entrega intermedia, publica ese commit y consulta su run de CI una vez abierto el PR.

Después de los bloques que hayan requerido cambios:

```bash
git fetch origin
git log --oneline --reverse origin/master..HEAD
git diff --stat origin/master...HEAD
git diff --check origin/master...HEAD
git status --short
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
```

El log debe mostrar los cambios separados que acabas de registrar. Comprueba los casos del issue y anota resultado, fecha, entorno y commit. Las pruebas Unity se ejecutan desde **Window > General > Test Runner**, pestañas EditMode y PlayMode. Adjunta el XML o la salida pertinente al PR. Los checks .NET y del repositorio cubren sus propios casos; UI, cámara y audio necesitan los casos de Editor/manuales indicados arriba.

Si `master` avanzó y GitHub solicita actualizar tu rama, con el árbol de trabajo limpio integra su estado:

```bash
git fetch origin
git merge origin/master
```

Resuelve cada conflicto en el editor, stagea las rutas resueltas explícitamente y completa el merge con `git commit`. Vuelve a ejecutar las comprobaciones antes de publicar. Mantén el historial compartido sin sobrescribirlo con force push.

## 5. Publicar los commits y abrir o actualizar el PR

Publica la serie; este push conserva cada commit:

```bash
git push -u origin feature/christian-arte-audio
```

También puedes hacer push después de cada commit. La rama sigue siendo la misma y el PR acumula la serie. Antes de crear el PR, prepara su descripción en un archivo. Completa los resultados reales y los casos pendientes antes de ejecutar `gh pr create`:

```bash
cat > /tmp/huasca-christianalexyana-pr.md <<'MD'
## Cambio

Closes #4.

Como jugador quiero leer la interfaz y reconocer el mundo con un acabado ilustrado consistente, acompañado por sonido que responda a cada acción.

## Serie de cambios

- Silueta y acabado de la flota.
- Terreno y objetos del mapa.
- Paneles, botones y tipografía.
- Fondos de menú y resultados.
- Recursos de sonido y música.
- Eventos de audio y presentación Runtime.
- Registro de arte y validación.

## Evidencia

- Commit probado: completar con `git rev-parse --short HEAD`.
- CoreChecks: indicar conteo y resultado real.
- Repository checks: indicar resultado real.
- Unity/manuales: enumerar casos realizados, resultado y casos pendientes.
- Adjuntar capturas o XML aplicables y enlace a Actions cuando exista.

## Revisión

Revisor previsto: @SrEstrada. Verificar los criterios del issue antes de aprobar.
MD

gh pr create \
  --repo frix241/La-Resurreccion-del-Huascar \
  --base master \
  --head feature/christian-arte-audio \
  --assignee ChristianAlexYana \
  --reviewer SrEstrada \
  --title "Arte, interfaz visual y audio" \
  --body-file /tmp/huasca-christianalexyana-pr.md \
  --project "Huasca Rebirth · Desarrollo"
```

`--project` necesita acceso de tu cuenta al Project y el scope `project` de GitHub CLI. Si falta ese scope, `gh auth refresh -s project` lo solicita. Si ya existe el PR de esta rama, actualízalo con el push y `gh pr edit`; revisa primero la lista:

```bash
gh pr list --repo frix241/La-Resurreccion-del-Huascar --head feature/christian-arte-audio
gh pr checks --repo frix241/La-Resurreccion-del-Huascar --required
```

## 6. Evidencia, revisión e integración

En [el issue #4](https://github.com/frix241/La-Resurreccion-del-Huascar/issues/4) marca solo las tareas efectivamente realizadas. Añade el PR, el commit probado, el enlace a Actions y el resultado de cada caso del plan de pruebas. Mueve la tarjeta a **Revisión** cuando haya evidencia y un revisor solicitado. **Aprobado** requiere una revisión real de un compañero.

Cada corrección pedida por el revisor debe ser otro commit pequeño con sus archivos y su prueba. Publica con `git push origin feature/christian-arte-audio`; GitHub volverá a ejecutar CI. El PR debe tener **Core checks (.NET 10)** y **Repository checks** aprobados en su último commit, conversaciones resueltas y una aprobación independiente.

La integración está configurada para **Create a merge commit**, conservando los commits del desarrollador. La rama integrada se elimina del remoto; el historial permanece accesible desde `master` y el PR. Después del merge y con tu trabajo local guardado:

```bash
git switch master
git pull --ff-only origin master
```

## 7. Criterio para terminar

- Cada cambio tiene un propósito entendible y su commit muestra solo los archivos de ese bloque.
- El PR conserva la serie de commits y enlaza el issue correcto.
- Los casos del issue tienen resultados verificables; los pendientes están identificados.
- Los checks del último commit están correctos y hay aprobación de otra persona.
- Los builds, ZIPs, cachés, logs y archivos temporales quedan fuera del historial fuente.
