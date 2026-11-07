# Mauro Sullca Mamani: Progreso y persistencia de campaña

**Responsable:** `MauroSullcaMamani` · **Issue:** [#2](https://github.com/frix241/La-Resurreccion-del-Huascar/issues/2) · **Estimación inicial:** 5 puntos.  
**Rama:** `feature/mauro-progresion` · **Revisor previsto:** `frix241` · **Base del PR:** `master`.

## Resultado que debe entregar la tarea

Como jugador quiero conservar mis avances y desbloqueos para continuar la campaña después de cerrar el juego.

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
git switch -c feature/mauro-progresion
```

Si ya trabajabas en esa rama, vuelve a ella con `git switch feature/mauro-progresion`; evita intentar crearla de nuevo.

## 2. Plan de commits

| Bloque | Resultado | Mensaje propuesto |
|---|---|---|
| 1 | Reglas de victoria y derrota | `fix(missions): resolver victoria y derrota de forma consistente` |
| 2 | Desbloqueo secuencial de campaña | `fix(progress): mantener desbloqueos en orden de campaña` |
| 3 | Lectura y serialización de progreso | `fix(progress): validar y restaurar el estado serializado` |
| 4 | Guardado y recuperación de archivos | `fix(save): conservar respaldo y recuperar progreso corrupto` |
| 5 | Integración del progreso con Unity | `fix(save): restaurar progreso al iniciar la campaña` |
| 6 | Registro de validación de campaña | `docs(progress): registrar validación de guardado y campaña` |

Los mensajes siguientes describen el alcance previsto. Adáptalos si el diff final resuelve un caso más específico. Código y regresión del mismo caso viajan juntos.

### Commit 1: Reglas de victoria y derrota

**Objetivo:** Que una misión termine una sola vez y respete la prioridad de las causas de derrota.

**Trabajo antes del stage:** Revisa los casos de patrullera, escolta y baterías. Si corriges una regla, incorpora el caso de regresión en el mismo commit.

**Comprobaciones de este bloque:**

- Ejecuta CoreChecks y comprueba T10–T14 y T31–T32.
- En Unity verifica que el resultado de la misión no vuelva a cambiar después de finalizar.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Core/Mission.cs
git add -p -- \
  Tests/CoreChecks/Program.cs \
  Assets/_LRH/Tests/EditMode/CoreTests.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "fix(missions): resolver victoria y derrota de forma consistente"
git show --stat --oneline HEAD
```

### Commit 2: Desbloqueo secuencial de campaña

**Objetivo:** Que completar una misión habilite la siguiente sin saltar misiones.

**Trabajo antes del stage:** Trabaja las reglas de `Complete`, `CanPlay` e `IsCompleted`; cubre completar dos veces y un número fuera del orden válido.

**Comprobaciones de este bloque:**

- Comprueba T15 y añade los casos nuevos que justifique el cambio.
- Verifica que una campaña recién creada solo permita la primera misión.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Core/Progress.cs
git add -p -- \
  Tests/CoreChecks/Program.cs \
  Assets/_LRH/Tests/EditMode/CoreTests.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "fix(progress): mantener desbloqueos en orden de campaña"
git show --stat --oneline HEAD
```

### Commit 3: Lectura y serialización de progreso

**Objetivo:** Que un guardado válido conserve avance y configuración al volver a cargarlo.

**Trabajo antes del stage:** Modifica únicamente serialización y lectura; contempla contenido formateado y entradas inválidas sin mezclar todavía escritura de archivos.

**Comprobaciones de este bloque:**

- Comprueba T16, T17 y T30.
- Valida ida y vuelta del estado y qué sucede con JSON inválido.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Core/Progress.cs
git add -p -- \
  Tests/CoreChecks/Program.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "fix(progress): validar y restaurar el estado serializado"
git show --stat --oneline HEAD
```

### Commit 4: Guardado y recuperación de archivos

**Objetivo:** Que guardar una segunda vez conserve un respaldo y que un archivo corrupto pueda recuperarse.

**Trabajo antes del stage:** Trabaja `ProgressStore`; comprueba archivo ausente, sobrescritura y conservación del archivo corrupto con un directorio temporal de prueba.

**Comprobaciones de este bloque:**

- Comprueba T21–T24 y T30.
- Usa archivos temporales de prueba y revisa que el guardado real del jugador no intervenga.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Core/ProgressStore.cs
git add -p -- \
  Tests/CoreChecks/Program.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "fix(save): conservar respaldo y recuperar progreso corrupto"
git show --stat --oneline HEAD
```

### Commit 5: Integración del progreso con Unity

**Objetivo:** Que reiniciar el juego restaure el mismo estado que utiliza la campaña.

**Trabajo antes del stage:** Conecta el repositorio Runtime con las reglas del núcleo; modifica en GameRoot solo los puntos de carga/guardado que requiera esta integración.

**Comprobaciones de este bloque:**

- Ejecuta las pruebas PlayMode relacionadas en Unity 6000.6.8f1.
- Completa misión 1, cierra y abre: misión 2 debe seguir disponible.
- Revisa también T25–T26 si el cambio afecta al volumen persistido.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -- \
  Assets/_LRH/Scripts/Runtime/ProgressRepository.cs
git add -p -- \
  Assets/_LRH/Scripts/Runtime/GameRoot.cs \
  Assets/_LRH/Tests/PlayMode/MissionPlayTests.cs
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "fix(save): restaurar progreso al iniciar la campaña"
git show --stat --oneline HEAD
```

### Commit 6: Registro de validación de campaña

**Objetivo:** Que el revisor pueda repetir los casos y conocer el resultado real.

**Trabajo antes del stage:** Actualiza el informe de pruebas con commit, entorno, casos ejecutados y pendientes de esta tarea.

**Comprobaciones de este bloque:**

- Incluye el resultado real de CoreChecks y los casos manuales del issue #2.
- Deja los XML y logs generados fuera del commit; enlázalos como evidencia del PR.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  docs/04_pruebas.md
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "docs(progress): registrar validación de guardado y campaña"
git show --stat --oneline HEAD
```

## 3. Separar cambios que ya están mezclados

Coordina `Mission.cs` con Sergio si cambia reglas de objetivo. `Tests/CoreChecks/Program.cs` y las pruebas Unity son compartidos: selecciona solo los casos de este bloque con `git add -p`.

En `git add -p`, `y` incluye un fragmento, `n` lo deja para el siguiente commit, `s` intenta dividirlo y `q` termina. Si un fragmento mezcla tareas y no se puede dividir limpiamente, reorganiza el cambio en el editor y vuelve a seleccionar. Revisa siempre `git diff --cached` antes del commit.

Para retirar del stage un archivo sin perder la edición:

```bash
git restore --staged -- Tests/CoreChecks/Program.cs
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
git push -u origin feature/mauro-progresion
```

También puedes hacer push después de cada commit. La rama sigue siendo la misma y el PR acumula la serie. Antes de crear el PR, prepara su descripción en un archivo. Completa los resultados reales y los casos pendientes antes de ejecutar `gh pr create`:

```bash
cat > /tmp/huasca-maurosullcamamani-pr.md <<'MD'
## Cambio

Closes #2.

Como jugador quiero conservar mis avances y desbloqueos para continuar la campaña después de cerrar el juego.

## Serie de cambios

- Reglas de victoria y derrota.
- Desbloqueo secuencial de campaña.
- Lectura y serialización de progreso.
- Guardado y recuperación de archivos.
- Integración del progreso con Unity.
- Registro de validación de campaña.

## Evidencia

- Commit probado: completar con `git rev-parse --short HEAD`.
- CoreChecks: indicar conteo y resultado real.
- Repository checks: indicar resultado real.
- Unity/manuales: enumerar casos realizados, resultado y casos pendientes.
- Adjuntar capturas o XML aplicables y enlace a Actions cuando exista.

## Revisión

Revisor previsto: @frix241. Verificar los criterios del issue antes de aprobar.
MD

gh pr create \
  --repo frix241/La-Resurreccion-del-Huascar \
  --base master \
  --head feature/mauro-progresion \
  --assignee MauroSullcaMamani \
  --reviewer frix241 \
  --title "Progreso y persistencia de campaña" \
  --body-file /tmp/huasca-maurosullcamamani-pr.md \
  --project "Huasca Rebirth · Desarrollo"
```

`--project` necesita acceso de tu cuenta al Project y el scope `project` de GitHub CLI. Si falta ese scope, `gh auth refresh -s project` lo solicita. Si ya existe el PR de esta rama, actualízalo con el push y `gh pr edit`; revisa primero la lista:

```bash
gh pr list --repo frix241/La-Resurreccion-del-Huascar --head feature/mauro-progresion
gh pr checks --repo frix241/La-Resurreccion-del-Huascar --required
```

## 6. Evidencia, revisión e integración

En [el issue #2](https://github.com/frix241/La-Resurreccion-del-Huascar/issues/2) marca solo las tareas efectivamente realizadas. Añade el PR, el commit probado, el enlace a Actions y el resultado de cada caso del plan de pruebas. Mueve la tarjeta a **Revisión** cuando haya evidencia y un revisor solicitado. **Aprobado** requiere una revisión real de un compañero.

Cada corrección pedida por el revisor debe ser otro commit pequeño con sus archivos y su prueba. Publica con `git push origin feature/mauro-progresion`; GitHub volverá a ejecutar CI. El PR debe tener **Core checks (.NET 10)** y **Repository checks** aprobados en su último commit, conversaciones resueltas y una aprobación independiente.

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
