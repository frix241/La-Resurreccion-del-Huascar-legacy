# Albert: Integración, CI y flujo del equipo

**Responsable:** `frix241` · **Issue:** [#1](https://github.com/frix241/La-Resurreccion-del-Huascar/issues/1) · **Estimación inicial:** 3 puntos.  
**Rama:** `chore/project-baseline` · **Revisor previsto:** `SrEstrada` · **Base del PR:** `master`.

## Resultado que debe entregar la tarea

Como integrante del equipo quiero una base fuente revisable y comprobaciones reproducibles para integrar el trabajo por tareas.

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

La integración inicial está en el [PR #5](https://github.com/frix241/La-Resurreccion-del-Huascar/pull/5). Para un clon nuevo:

```bash
git clone https://github.com/frix241/La-Resurreccion-del-Huascar.git
cd La-Resurreccion-del-Huascar
git fetch origin
git switch --track origin/chore/project-baseline
```

Si ya tienes la rama local:

```bash
git switch chore/project-baseline
git pull --ff-only origin chore/project-baseline
git log --oneline -8
```

Los commits de la importación ya publicados se revisan tal como están. Los bloques de esta guía son ajustes sucesivos sobre esa rama; no hay que volver a importar los archivos.

## 2. Plan de commits

| Bloque | Resultado | Mensaje propuesto |
|---|---|---|
| 1 | Checks del núcleo en Actions | `ci(core): ejecutar y publicar comprobaciones del núcleo` |
| 2 | Validación del árbol y recursos Unity | `ci(repo): validar archivos y metadatos de Unity` |
| 3 | Exclusiones y atributos de archivos | `chore(git): mantener exclusiones y atributos del proyecto` |
| 4 | Revisores y plantillas del equipo | `chore(github): definir revisión y plantillas del equipo` |
| 5 | Portada y documentación de uso | `docs(repo): explicar inicio y flujo de contribución` |
| 6 | Guías de commits y evidencia de integración | `docs(team): detallar commits y revisión por desarrollador` |

Los mensajes siguientes describen el alcance previsto. Adáptalos si el diff final resuelve un caso más específico. Código y regresión del mismo caso viajan juntos.

### Commit 1: Checks del núcleo en Actions

**Objetivo:** Que cualquier PR ejecute CoreChecks y conserve el resultado.

**Trabajo antes del stage:** Ajusta únicamente el job .NET, restore, código de salida y log. El archivo de tests solo se cambia si la tarea requiere corregir su configuración.

**Comprobaciones de este bloque:**

- Ejecuta CoreChecks localmente para validar un ajuste de configuración.
- Después del push revisa el job Core checks (.NET 10) y su log descargable.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  .github/workflows/ci.yml \
  Tests/CoreChecks/CoreChecks.csproj
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
git commit -m "ci(core): ejecutar y publicar comprobaciones del núcleo"
git show --stat --oneline HEAD
```

### Commit 2: Validación del árbol y recursos Unity

**Objetivo:** Que el CI detecte JSON roto, conflictos, salidas generadas y metadatos inconsistentes.

**Trabajo antes del stage:** Modifica el validador y, si hace falta, solo el job Repository checks del workflow. Comprueba que una entrada inválida se rechace en un entorno temporal.

**Comprobaciones de este bloque:**

- Ejecuta Repository checks sobre el stage.
- Comprueba con archivos temporales que se rechacen casos inválidos sin tocar el proyecto real.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  Tools/check_repository.py \
  .github/workflows/ci.yml
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "ci(repo): validar archivos y metadatos de Unity"
git show --stat --oneline HEAD
```

### Commit 3: Exclusiones y atributos de archivos

**Objetivo:** Que los commits contengan fuente y mantengan la serialización de Unity.

**Trabajo antes del stage:** Ajusta .gitignore y .gitattributes sin quitar licencias ni recursos fuente. Los builds y ZIPs quedan fuera de Git.

**Comprobaciones de este bloque:**

- Inspecciona git status --short e identifica los ignorados con git check-ignore cuando haga falta.
- Verifica git diff --cached --check y Repository checks.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  .gitignore \
  .gitattributes
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "chore(git): mantener exclusiones y atributos del proyecto"
git show --stat --oneline HEAD
```

### Commit 4: Revisores y plantillas del equipo

**Objetivo:** Que los futuros PR e issues pidan alcance, aceptación y pruebas.

**Trabajo antes del stage:** Ajusta CODEOWNERS y plantillas sin mezclar cambios del juego. El mapeo de responsables debe coincidir con los issues #1–#4.

**Comprobaciones de este bloque:**

- Revisa nombres de cuenta y rutas de CODEOWNERS.
- Comprueba que cada formulario pida criterios y pruebas, y que los bloques YAML sigan válidos.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  .github/CODEOWNERS \
  .github/PULL_REQUEST_TEMPLATE.md \
  .github/ISSUE_TEMPLATE/tarea.yml \
  .github/ISSUE_TEMPLATE/historia.yml \
  .github/dependabot.yml
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "chore(github): definir revisión y plantillas del equipo"
git show --stat --oneline HEAD
```

### Commit 5: Portada y documentación de uso

**Objetivo:** Que quien abre el repo sepa ejecutar el juego y encontrar sus tareas.

**Trabajo antes del stage:** Actualiza README, CONTRIBUTING y el índice de guías. Revisa que las rutas enlazadas existan y que Unity figure como 6000.6.8f1.

**Comprobaciones de este bloque:**

- Comprueba vínculos relativos y captura de portada.
- Confirma que las instrucciones apunten a master y conserven las ramas acordadas.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  README.md \
  CONTRIBUTING.md \
  docs/GUIA_COMMITS.md
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "docs(repo): explicar inicio y flujo de contribución"
git show --stat --oneline HEAD
```

### Commit 6: Guías de commits y evidencia de integración

**Objetivo:** Que cada miembro tenga bloques revisables y el revisor vea pruebas reales.

**Trabajo antes del stage:** Actualiza cada guía y el informe del repo con los enlaces del PR, checks y reglas. Conserva el historial de commits al integrar.

**Comprobaciones de este bloque:**

- Valida comandos, rutas, nombres de rama y correspondencia issue/responsable.
- Comprueba los dos checks del último commit antes de pedir aprobación.

**Stage y commit:** ejecuta después de realizar y comprobar este bloque. Las rutas de `git add -p` requieren seleccionar los fragmentos correspondientes.

```bash
git status --short
git add -p -- \
  docs/equipo/00-REVISION-Y-FLUJO.md \
  docs/equipo/01-MAURO.md \
  docs/equipo/02-SERGIO.md \
  docs/equipo/03-CHRISTIAN.md \
  docs/equipo/04-ALBERT-INTEGRACION.md \
  docs/equipo/05-KANBAN-Y-PR.md \
  docs/REPOSITORIO.md
git diff --cached --name-only
git diff --cached --check
git diff --cached
python3 Tools/check_repository.py
git commit -m "docs(team): detallar commits y revisión por desarrollador"
git show --stat --oneline HEAD
```

## 3. Separar cambios que ya están mezclados

El PR #5 ya existe. Sus commits publicados se conservan. Los bloques siguientes sirven para preparar ajustes reales por separado; revisa primero qué está resuelto en el PR. Las nuevas tareas posteriores usarán feature/albert-integracion-ci desde master y su issue propio.

En `git add -p`, `y` incluye un fragmento, `n` lo deja para el siguiente commit, `s` intenta dividirlo y `q` termina. Si un fragmento mezcla tareas y no se puede dividir limpiamente, reorganiza el cambio en el editor y vuelve a seleccionar. Revisa siempre `git diff --cached` antes del commit.

Para retirar del stage un archivo sin perder la edición:

```bash
git restore --staged -- .github/workflows/ci.yml
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
git push -u origin chore/project-baseline
```

También puedes hacer push después de cada commit. La rama sigue siendo la misma y el PR acumula la serie. El PR #5 ya existe y recibe los commits nuevos automáticamente:

```bash
gh pr view 5 --repo frix241/La-Resurreccion-del-Huascar
gh pr checks 5 --repo frix241/La-Resurreccion-del-Huascar --required
```

Si trabajas después en otra tarea, crea una rama nueva desde `master` actualizado y abre el PR de su issue, en lugar de reutilizar esta importación.

## 6. Evidencia, revisión e integración

En [el issue #1](https://github.com/frix241/La-Resurreccion-del-Huascar/issues/1) marca solo las tareas efectivamente realizadas. Añade el PR, el commit probado, el enlace a Actions y el resultado de cada caso del plan de pruebas. Mueve la tarjeta a **Revisión** cuando haya evidencia y un revisor solicitado. **Aprobado** requiere una revisión real de un compañero.

Cada corrección pedida por el revisor debe ser otro commit pequeño con sus archivos y su prueba. Publica con `git push origin chore/project-baseline`; GitHub volverá a ejecutar CI. El PR debe tener **Core checks (.NET 10)** y **Repository checks** aprobados en su último commit, conversaciones resueltas y una aprobación independiente.

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
