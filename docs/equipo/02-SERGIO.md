# Sergio: Navegación, colisiones y objetivos

**Responsable:** `SrEstrada` · **Issue:** [#3](https://github.com/frix241/La-Resurreccion-del-Huascar/issues/3) · **Estimación inicial:** 8 puntos.  
**Rama:** `feature/sergio-combate-mapa` · **Revisor previsto:** `MauroSullcaMamani` · **Base del PR:** `master`.

## Resultado que debe entregar la tarea

Como jugador quiero navegar dentro del mapa sin empujar otros cascos y localizar el objetivo vigente para saber hacia dónde avanzar.

Esta guía divide el trabajo en **6 bloques de commit**. En cada bloque: realiza el cambio indicado, ejecuta sus comprobaciones, revisa el stage y registra ese bloque antes de empezar el siguiente. La lista es un plan de separación: si un comportamiento ya está correcto y no hay diff, registra su comprobación en el issue y continúa. Cada commit debe contener un cambio real y mantener el proyecto utilizable.

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
git switch -c feature/sergio-combate-mapa
```

Si ya trabajabas en esa rama, vuelve a ella con `git switch feature/sergio-combate-mapa`; evita intentar crearla de nuevo.

## 2. Plan de commits

| Bloque | Resultado | Mensaje propuesto |
|---|---|---|
| 1 | Contratos de daño y fuego entre facciones | `fix(combat): respetar daño y reglas entre facciones` |
| 2 | Contactos entre cascos | `fix(ships): impedir empujes indebidos entre cascos` |
| 3 | Límites y colisiones del mapa | `fix(map): respetar límites y obstáculos navegables` |
| 4 | Seguimiento de cámara y carta táctica | `fix(camera): seguir al Huascar dentro de los límites` |
| 5 | Actualización del objetivo cercano | `fix(objectives): actualizar el objetivo activo más cercano` |
| 6 | Evidencia de navegación y objetivos | `docs(gameplay): registrar pruebas de navegación y objetivos` |

Los mensajes siguientes describen el alcance previsto. Adáptalos si el diff final resuelve un caso más específico. Código y regresión del mismo caso viajan juntos.

### Commit 1: Contratos de daño y fuego entre facciones

**Objetivo:** Que los contactos y disparos respeten salud y reglas de facción.

**Trabajo antes del stage:** Revisa daño, fuego aliado y registro de actores. Acompaña cualquier corrección con el caso mínimo que la reproduce.

**Comprobaciones de este bloque:**

- Comprueba T01–T09, T20 y T33.
- Distingue un fallo de daño de un empuje físico antes de cambiar colisiones.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Core/Health.cs \
  Assets/_LRH/Scripts/Core/Combat.cs \
  Assets/_LRH/Scripts/Core/Registry.cs
git add -p -- \
  Tests/CoreChecks/Program.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "fix(combat): respetar daño y reglas entre facciones"
git show --stat --oneline HEAD
```

### Commit 2: Contactos entre cascos

**Objetivo:** Que el Huascar no desplace de forma indebida a aliados o enemigos.

**Trabajo antes del stage:** Corrige la respuesta de `ShipActor` y la configuración de actores que la afecte. Añade regresión de contacto frontal y lateral.

**Comprobaciones de este bloque:**

- Ejecuta PlayMode con un barco aliado y otro enemigo.
- Comprueba contactos de frente y de costado, avance sostenido y separación posterior.
- Registra si ocurre solapamiento o deriva después del contacto.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Runtime/ShipActor.cs \
  Assets/_LRH/Scripts/Runtime/ActorFactory.cs
git add -p -- \
  Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "fix(ships): impedir empujes indebidos entre cascos"
git show --stat --oneline HEAD
```

### Commit 3: Límites y colisiones del mapa

**Objetivo:** Que todos los bordes y los objetos fijos conserven el barco en zona navegable.

**Trabajo antes del stage:** Ajusta TerrainMap/OpenSea y el mapa guardado solo donde el caso reproducible lo requiera. Conserva el JSON y su metadata en el mismo bloque.

**Comprobaciones de este bloque:**

- Recorre los cuatro bordes y sus esquinas en Unity.
- Comprueba costa y objetos con colisión sin quedar atrapado.
- Ejecuta Repository checks después del stage para validar el JSON.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Runtime/TerrainMap.cs \
  Assets/_LRH/Scripts/Runtime/OpenSea.cs \
  Assets/_LRH/Resources/Maps/pacific.json \
  Assets/_LRH/Resources/Maps/pacific.json.meta
git add -p -- \
  Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "fix(map): respetar límites y obstáculos navegables"
git show --stat --oneline HEAD
```

### Commit 4: Seguimiento de cámara y carta táctica

**Objetivo:** Que el barco siga visible y el minimapa represente la posición del mismo mundo.

**Trabajo antes del stage:** Edita los bloques de cámara en GameRoot y los marcadores cartográficos. Comprueba centro del mapa y extremos sin mezclar cambios de UI decorativa.

**Comprobaciones de este bloque:**

- Recorre el mapa y confirma que la cámara sigue al jugador sin salir del área prevista.
- Contrasta la posición del minimapa con el mundo y revisa las esquinas.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Runtime/CartographyVisuals.cs
git add -p -- \
  Assets/_LRH/Scripts/Runtime/GameRoot.cs \
  Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "fix(camera): seguir al Huascar dentro de los límites"
git show --stat --oneline HEAD
```

### Commit 5: Actualización del objetivo cercano

**Objetivo:** Que el indicador cambie cuando cambia el objetivo activo más próximo.

**Trabajo antes del stage:** Revisa selección y refresco en GameUi/GameRoot. Cubre cambio de distancias, destrucción del objetivo actual y ausencia de objetivos.

**Comprobaciones de este bloque:**

- En misión 2 verifica dirección del convoy y su estado de vida.
- En misión 3 alterna distancia entre baterías y destruye la señalada.
- Comprueba que el indicador se actualice en los siguientes frames y desaparezca cuando corresponda.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  Assets/_LRH/Scripts/Runtime/GameUi.cs \
  Assets/_LRH/Scripts/Runtime/GameRoot.cs \
  Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "fix(objectives): actualizar el objetivo activo más cercano"
git show --stat --oneline HEAD
```

### Commit 6: Evidencia de navegación y objetivos

**Objetivo:** Que el revisor pueda repetir choques, límites, cámara e indicador.

**Trabajo antes del stage:** Documenta los pasos, casos, versión de Unity y resultados del issue #3.

**Comprobaciones de este bloque:**

- Adjunta XML de PlayMode y capturas mediante el PR, sin versionar Build.
- Identifica el commit probado y cualquier caso pendiente.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  docs/HUD_Y_LIMITES.md \
  docs/MEJORAS_MAR_ABIERTO.md
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "docs(gameplay): registrar pruebas de navegación y objetivos"
git show --stat --oneline HEAD
```

## 3. Separar cambios que ya están mezclados

GameRoot comparte integración con Albert y Mauro; GameUi comparte presentación con Albert y Christian. Acordad el alcance antes de editarlos. Usa `git add -p` en esos archivos y en las pruebas compartidas.

En `git add -p`, `y` incluye un fragmento, `n` lo deja para el siguiente commit, `s` intenta dividirlo y `q` termina. Si un fragmento mezcla tareas y no se puede dividir limpiamente, reorganiza el cambio en el editor y vuelve a seleccionar. Revisa siempre `git diff --cached` antes del commit.

Para retirar del stage un archivo sin perder la edición:

```bash
git restore --staged -- Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs
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
git push -u origin feature/sergio-combate-mapa
```

También puedes hacer push después de cada commit. La rama sigue siendo la misma y el PR acumula la serie. Antes de crear el PR, prepara su descripción en un archivo. Completa los resultados reales y los casos pendientes antes de ejecutar `gh pr create`:

```bash
cat > /tmp/huasca-srestrada-pr.md <<'MD'
## Cambio

Closes #3.

Como jugador quiero navegar dentro del mapa sin empujar otros cascos y localizar el objetivo vigente para saber hacia dónde avanzar.

## Serie de cambios

- Contratos de daño y fuego entre facciones.
- Contactos entre cascos.
- Límites y colisiones del mapa.
- Seguimiento de cámara y carta táctica.
- Actualización del objetivo cercano.
- Evidencia de navegación y objetivos.

## Evidencia

- Commit probado: completar con `git rev-parse --short HEAD`.
- CoreChecks: indicar conteo y resultado real.
- Repository checks: indicar resultado real.
- Unity/manuales: enumerar casos realizados, resultado y casos pendientes.
- Adjuntar capturas o XML aplicables y enlace a Actions cuando exista.

## Revisión

Revisor previsto: @MauroSullcaMamani. Verificar los criterios del issue antes de aprobar.
MD

gh pr create \
  --repo frix241/La-Resurreccion-del-Huascar \
  --base master \
  --head feature/sergio-combate-mapa \
  --assignee SrEstrada \
  --reviewer MauroSullcaMamani \
  --title "Navegación, colisiones y objetivos" \
  --body-file /tmp/huasca-srestrada-pr.md \
  --project "Huasca Rebirth · Desarrollo"
```

`--project` necesita acceso de tu cuenta al Project y el scope `project` de GitHub CLI. Si falta ese scope, `gh auth refresh -s project` lo solicita. Si ya existe el PR de esta rama, actualízalo con el push y `gh pr edit`; revisa primero la lista:

```bash
gh pr list --repo frix241/La-Resurreccion-del-Huascar --head feature/sergio-combate-mapa
gh pr checks --repo frix241/La-Resurreccion-del-Huascar --required
```

## 6. Evidencia, revisión e integración

En [el issue #3](https://github.com/frix241/La-Resurreccion-del-Huascar/issues/3) marca solo las tareas efectivamente realizadas. Añade el PR, el commit probado, el enlace a Actions y el resultado de cada caso del plan de pruebas. Mueve la tarjeta a **Revisión** cuando haya evidencia y un revisor solicitado. **Aprobado** requiere una revisión real de un compañero.

Cada corrección pedida por el revisor debe ser otro commit pequeño con sus archivos y su prueba. Publica con `git push origin feature/sergio-combate-mapa`; GitHub volverá a ejecutar CI. El PR debe tener **Core checks (.NET 10)** y **Repository checks** aprobados en su último commit, conversaciones resueltas y una aprobación independiente.

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
