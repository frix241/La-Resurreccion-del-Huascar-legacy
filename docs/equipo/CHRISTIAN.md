# Christian: incorporar la entrega existente del issue #4

Este ZIP contiene únicamente los archivos asignados a Christian. Son parte del juego ya implementado, snapshot `dbdce75`. El flujo reconstruye su incorporación en Git con fechas e identidades reales. No debes desarrollar nuevas funciones para aplicar este paquete.

**Cuenta:** ChristianAlexYana. **Rama:** `feature/christian-arte-audio`. **Revisor:** SrEstrada.

## Preparación

Primero comprueba si tu rama ya tiene un PR publicado:

```bash
gh pr list --repo frix241/La-Resurreccion-del-Huascar --head feature/christian-arte-audio
```

Si existe ese PR, su entrega puede estar ya incorporada: desde un clon usa `gh pr checkout feature/christian-arte-audio`, configura KIT y ejecuta la verificación del paquete. Si todos los hashes coinciden, los bloques ya están registrados; continúa con comprobaciones y revisión. La entrega inicial de Albert se publica preparada como parte de esta configuración. Los demás devs incorporan su ZIP tras el merge anterior.

Orden obligatorio de merge: Albert (#1) → Mauro (#2) → Sergio (#3) → Christian (#4). Cada dev espera a que la entrega anterior esté integrada en master. Conserva el ZIP extraído fuera del clon del proyecto. Los comandos se ejecutan con Bash, Git, Python 3 y gh autenticado con tu cuenta.

```bash
git clone https://github.com/frix241/La-Resurreccion-del-Huascar.git
cd La-Resurreccion-del-Huascar
git fetch origin
git switch master
git pull --ff-only origin master
git switch -c feature/christian-arte-audio
```

Si ya tienes el clon, comienza en `git fetch origin`. Si ya existe tu rama, usa `git switch feature/christian-arte-audio`. Configura KIT con la ruta absoluta a la carpeta extraída de tu ZIP (la que contiene aplicar.py):

```bash
read -r -p "Ruta absoluta de tu paquete extraído: " KIT
export KIT
python3 --version
gh auth status
git config user.name
git config user.email
```

La identidad debe ser tuya. El helper verifica archivos, pasos anteriores y dependencias; se detiene si el clon tiene cambios sin confirmar. No lo ejecutes dentro del proyecto original sin historial: usa el clon de GitHub.

## Incorporar los bloques

Cada manifiesto contiene la lista exacta de archivos del commit, incluidos los `.meta`. Puedes abrirlo antes de ejecutar. `aplicar.py` copia esos archivos al clon, sin modificar su contenido. Después `git add --pathspec-from-file` incorpora solo esa lista y `git commit` registra el bloque.

### Paso 1: fuentes

Archivos exactos: `manifiestos/01-fuentes.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 01-fuentes
cat "$KIT/manifiestos/01-fuentes.txt"
git add --pathspec-from-file="$KIT/manifiestos/01-fuentes.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(art): incorporar fuentes de arte y generadores"
```

### Paso 2: flota

Archivos exactos: `manifiestos/02-flota.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 02-flota
cat "$KIT/manifiestos/02-flota.txt"
git add --pathspec-from-file="$KIT/manifiestos/02-flota.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(art): incorporar sprites de la flota"
```

### Paso 3: terreno

Archivos exactos: `manifiestos/03-terreno.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 03-terreno
cat "$KIT/manifiestos/03-terreno.txt"
git add --pathspec-from-file="$KIT/manifiestos/03-terreno.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(art): incorporar recursos del terreno"
```

### Paso 4: interfaz

Archivos exactos: `manifiestos/04-interfaz.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 04-interfaz
cat "$KIT/manifiestos/04-interfaz.txt"
git add --pathspec-from-file="$KIT/manifiestos/04-interfaz.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(ui): incorporar fondos paneles y tipografía"
```

### Paso 5: audio

Archivos exactos: `manifiestos/05-audio.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 05-audio
cat "$KIT/manifiestos/05-audio.txt"
git add --pathspec-from-file="$KIT/manifiestos/05-audio.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(audio): incorporar música y efectos"
```

### Paso 6: referencias

Archivos exactos: `manifiestos/06-referencias.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 06-referencias
cat "$KIT/manifiestos/06-referencias.txt"
git add --pathspec-from-file="$KIT/manifiestos/06-referencias.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "docs(art): incorporar recursos y registros de la entrega"
```

## Comprobar la entrega completa

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --verificar
git status --short
git log --oneline --reverse origin/master..HEAD
python3 Tools/check_repository.py
```

El helper debe confirmar que todos los archivos del paquete coinciden. `git status` debe quedar limpio. Las fechas, autor y hashes de los commits serán los de tu publicación; la coincidencia se comprueba sobre los archivos del juego.

Ejecuta la suite del núcleo al completar los bloques:

```bash
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
```

Resultado esperado del snapshot: 33/33 comprobaciones. Si falla, conserva el error y detén la publicación; no declares aprobadas pruebas que no se ejecutaron.

## Validar el juego completo en Unity

Después de incorporar todos los bloques, abre el clon con Unity 6000.6.8f1, espera la importación y ejecuta EditMode y PlayMode en Test Runner. Los 23 casos PlayMode existentes cubren contactos, mapa, objetivo cercano, flujo de misión y audio. Registra el resultado real y adjunta el XML. Comprueba menú, HUD, pausa, créditos, victoria y derrota a 1920×1080; adjunta capturas. Comprueba música, disparo, impacto y cambio de volumen. Los hashes verifican la copia; estas comprobaciones validan la ejecución.

Para ejecutar PlayMode por terminal, indica la ruta real a tu Editor y cierra ese proyecto en el Editor interactivo:

```bash
read -r -p "Ruta al ejecutable de Unity 6000.6.8f1: " UNITY_EDITOR
mkdir -p Build/Revision
"$UNITY_EDITOR" -batchmode -projectPath "$PWD" -runTests -testPlatform PlayMode -testResults "$PWD/Build/Revision/playmode.xml" -logFile "$PWD/Build/Revision/playmode.log"
```

Abre el XML y confirma el resultado. `Build/Revision` se excluye de Git. Si falla un caso, el issue permanece abierto hasta resolverlo o acordar su alcance con el revisor.

## Publicar la serie y el PR

```bash
git push -u origin feature/christian-arte-audio
cat > /tmp/huasca-issue-4-pr.md <<'MD'
## Entrega

Closes #4.

Incorporación por etapas de los archivos existentes asignados a Christian. Reconstrucción del snapshot dbdce75; los bloques se conservan como commits separados.

## Evidencia

- Verificación del paquete: adjuntar salida real.
- Repository checks: adjuntar resultado.
- CoreChecks: adjuntar resultado o indicar que esta fase aún no incluye la suite.
- Unity y capturas: adjuntar cuando corresponda a la entrega completa.

## Revisión

Revisor previsto: @SrEstrada. Verificar los archivos del manifiesto y los criterios del issue antes del merge.
MD

gh pr create --repo frix241/La-Resurreccion-del-Huascar --base master --head feature/christian-arte-audio --assignee ChristianAlexYana --reviewer SrEstrada --title "Reconstrucción: entrega de Christian" --body-file /tmp/huasca-issue-4-pr.md --label reconstruccion
```

Completa la evidencia real del archivo de descripción antes de abrir el PR. Si ya existe un PR de la rama, el push lo actualiza: usa `gh pr view` para consultarlo. El PR necesita los checks del último commit y aprobación independiente. Se integra mediante merge commit para conservar la serie. `Closes #4` cierra el issue cuando el PR se integra, no al abrirlo.

## Cuándo está cumplido el issue

Todos los bloques están incorporados; los hashes del paquete coinciden; el plan de pruebas de esta fase tiene evidencia real; el PR enlaza este issue, recibe revisión y se integra. En ese momento se desbloquea la entrega siguiente. Copiar archivos y abrir un PR no sustituye la aprobación del revisor.
