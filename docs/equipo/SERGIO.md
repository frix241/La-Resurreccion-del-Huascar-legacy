# Sergio: incorporar la entrega existente del issue #3

Este ZIP contiene únicamente los archivos asignados a Sergio. Son parte del juego ya implementado, snapshot `dbdce75`. El flujo reconstruye su incorporación en Git con fechas e identidades reales. No debes desarrollar nuevas funciones para aplicar este paquete.

**Cuenta:** SrEstrada. **Rama:** `feature/sergio-combate-mapa`. **Revisor:** MauroSullcaMamani.

## Preparación

Primero comprueba si tu rama ya tiene un PR publicado:

```bash
gh pr list --repo frix241/La-Resurreccion-del-Huascar --head feature/sergio-combate-mapa
```

Si existe ese PR, su entrega puede estar ya incorporada: desde un clon usa `gh pr checkout feature/sergio-combate-mapa`, configura KIT y ejecuta la verificación del paquete. Si todos los hashes coinciden, los bloques ya están registrados; continúa con comprobaciones y revisión. La entrega inicial de Albert se publica preparada como parte de esta configuración. Los demás devs incorporan su ZIP tras el merge anterior.

Orden obligatorio de merge: Albert (#1) → Mauro (#2) → Sergio (#3) → Christian (#4). Cada dev espera a que la entrega anterior esté integrada en master. Conserva el ZIP extraído fuera del clon del proyecto. Los comandos se ejecutan con Bash, Git, Python 3 y gh autenticado con tu cuenta.

```bash
git clone https://github.com/frix241/La-Resurreccion-del-Huascar.git
cd La-Resurreccion-del-Huascar
git fetch origin
git switch master
git pull --ff-only origin master
git switch -c feature/sergio-combate-mapa
```

Si ya tienes el clon, comienza en `git fetch origin`. Si ya existe tu rama, usa `git switch feature/sergio-combate-mapa`. Configura KIT con la ruta absoluta a la carpeta extraída de tu ZIP (la que contiene aplicar.py):

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

### Paso 1: actores

Archivos exactos: `manifiestos/01-actores.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 01-actores
cat "$KIT/manifiestos/01-actores.txt"
git add --pathspec-from-file="$KIT/manifiestos/01-actores.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(runtime): incorporar barcos entrada y proyectiles"
```

### Paso 2: mapa

Archivos exactos: `manifiestos/02-mapa.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 02-mapa
cat "$KIT/manifiestos/02-mapa.txt"
git add --pathspec-from-file="$KIT/manifiestos/02-mapa.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(map): incorporar terreno y carta táctica"
```

### Paso 3: flujo

Archivos exactos: `manifiestos/03-flujo.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 03-flujo
cat "$KIT/manifiestos/03-flujo.txt"
git add --pathspec-from-file="$KIT/manifiestos/03-flujo.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(game): incorporar flujo de campaña e interfaz"
```

### Paso 4: presentacion

Archivos exactos: `manifiestos/04-presentacion.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 04-presentacion
cat "$KIT/manifiestos/04-presentacion.txt"
git add --pathspec-from-file="$KIT/manifiestos/04-presentacion.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "feat(runtime): integrar presentación y sonido"
```

### Paso 5: escena

Archivos exactos: `manifiestos/05-escena.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 05-escena
cat "$KIT/manifiestos/05-escena.txt"
git add --pathspec-from-file="$KIT/manifiestos/05-escena.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "test(unity): incorporar escena y regresiones PlayMode"
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

## Publicar la serie y el PR

```bash
git push -u origin feature/sergio-combate-mapa
cat > /tmp/huasca-issue-3-pr.md <<'MD'
## Entrega

Closes #3.

Incorporación por etapas de los archivos existentes asignados a Sergio. Archivos de la versión de referencia dbdce75; los bloques se conservan como commits separados.

## Evidencia

- Verificación del paquete: adjuntar salida real.
- Repository checks: adjuntar resultado.
- CoreChecks: adjuntar resultado o indicar que esta fase aún no incluye la suite.
- Unity y capturas: adjuntar cuando corresponda a la entrega completa.

## Revisión

Revisor previsto: @MauroSullcaMamani. Verificar los archivos del manifiesto y los criterios del issue antes del merge.
MD

gh pr create --repo frix241/La-Resurreccion-del-Huascar --base master --head feature/sergio-combate-mapa --assignee SrEstrada --reviewer MauroSullcaMamani --title "Entrega de Sergio" --body-file /tmp/huasca-issue-3-pr.md --label entrega-actual
```

Completa la evidencia real del archivo de descripción antes de abrir el PR. Si ya existe un PR de la rama, el push lo actualiza: usa `gh pr view` para consultarlo. El PR necesita los checks del último commit y aprobación independiente. Se integra mediante merge commit para conservar la serie. `Closes #3` cierra el issue cuando el PR se integra, no al abrirlo.

## Cuándo está cumplido el issue

Todos los bloques están incorporados; los hashes del paquete coinciden; el plan de pruebas de esta fase tiene evidencia real; el PR enlaza este issue, recibe revisión y se integra. En ese momento se desbloquea la entrega siguiente. Copiar archivos y abrir un PR no sustituye la aprobación del revisor.
