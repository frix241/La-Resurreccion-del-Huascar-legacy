# Albert: incorporar la entrega existente del issue #1

Este ZIP contiene únicamente los archivos asignados a Albert. Son parte del juego ya implementado, snapshot `dbdce75`. El flujo reconstruye su incorporación en Git con fechas e identidades reales. No debes desarrollar nuevas funciones para aplicar este paquete.

**Cuenta:** frix241. **Rama:** `chore/base-equipo`. **Revisor:** SrEstrada.

## Preparación

Primero comprueba si tu rama ya tiene un PR publicado:

```bash
gh pr list --repo frix241/La-Resurreccion-del-Huascar --head chore/base-equipo
```

Si existe ese PR, su entrega puede estar ya incorporada: desde un clon usa `gh pr checkout chore/base-equipo`, configura KIT y ejecuta la verificación del paquete. Si todos los hashes coinciden, los bloques ya están registrados; continúa con comprobaciones y revisión. La entrega inicial de Albert se publica preparada como parte de esta configuración. Los demás devs incorporan su ZIP tras el merge anterior.

Orden obligatorio de merge: Albert (#1) → Mauro (#2) → Sergio (#3) → Christian (#4). Cada dev espera a que la entrega anterior esté integrada en master. Conserva el ZIP extraído fuera del clon del proyecto. Los comandos se ejecutan con Bash, Git, Python 3 y gh autenticado con tu cuenta.

```bash
git clone https://github.com/frix241/La-Resurreccion-del-Huascar.git
cd La-Resurreccion-del-Huascar
git fetch origin
git switch master
git pull --ff-only origin master
git switch -c chore/base-equipo
```

Si ya tienes el clon, comienza en `git fetch origin`. Si ya existe tu rama, usa `git switch chore/base-equipo`. Configura KIT con la ruta absoluta a la carpeta extraída de tu ZIP (la que contiene aplicar.py):

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

### Paso 1: base

Archivos exactos: `manifiestos/01-base.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 01-base
cat "$KIT/manifiestos/01-base.txt"
git add --pathspec-from-file="$KIT/manifiestos/01-base.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "chore(repo): incorporar exclusiones y validador"
```

### Paso 2: actions

Archivos exactos: `manifiestos/02-actions.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 02-actions
cat "$KIT/manifiestos/02-actions.txt"
git add --pathspec-from-file="$KIT/manifiestos/02-actions.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "ci: configurar comprobaciones por etapa"
```

### Paso 3: unity

Archivos exactos: `manifiestos/03-unity.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 03-unity
cat "$KIT/manifiestos/03-unity.txt"
git add --pathspec-from-file="$KIT/manifiestos/03-unity.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "chore(unity): incorporar paquetes y ajustes del proyecto"
```

### Paso 4: docs

Archivos exactos: `manifiestos/04-docs.txt`. Resultado: incorporar únicamente ese bloque de la entrega asignada.

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --paso 04-docs
cat "$KIT/manifiestos/04-docs.txt"
git add --pathspec-from-file="$KIT/manifiestos/04-docs.txt"
git diff --cached --check
git diff --cached --stat
python3 Tools/check_repository.py
git commit -m "docs: incorporar documentación y guías de entrega"
```

## Comprobar la entrega completa

```bash
python3 "$KIT/aplicar.py" --repo "$PWD" --verificar
git status --short
git log --oneline --reverse origin/master..HEAD
python3 Tools/check_repository.py
```

El helper debe confirmar que todos los archivos del paquete coinciden. `git status` debe quedar limpio. Las fechas, autor y hashes de los commits serán los de tu publicación; la coincidencia se comprueba sobre los archivos del juego.

Esta fase aún no contiene el núcleo ni Tests/CoreChecks. Actions lo informa explícitamente como pendiente de incorporar; no equivale a 33 pruebas aprobadas. La suite se incorpora en el último bloque de Mauro.

## Publicar la serie y el PR

```bash
git push -u origin chore/base-equipo
cat > /tmp/huasca-issue-1-pr.md <<'MD'
## Entrega

Closes #1.

Incorporación por etapas de los archivos existentes asignados a Albert. Reconstrucción del snapshot dbdce75; los bloques se conservan como commits separados.

## Evidencia

- Verificación del paquete: adjuntar salida real.
- Repository checks: adjuntar resultado.
- CoreChecks: adjuntar resultado o indicar que esta fase aún no incluye la suite.
- Unity y capturas: adjuntar cuando corresponda a la entrega completa.

## Revisión

Revisor previsto: @SrEstrada. Verificar los archivos del manifiesto y los criterios del issue antes del merge.
MD

gh pr create --repo frix241/La-Resurreccion-del-Huascar --base master --head chore/base-equipo --assignee frix241 --reviewer SrEstrada --title "Reconstrucción: entrega de Albert" --body-file /tmp/huasca-issue-1-pr.md --label reconstruccion
```

Completa la evidencia real del archivo de descripción antes de abrir el PR. Si ya existe un PR de la rama, el push lo actualiza: usa `gh pr view` para consultarlo. El PR necesita los checks del último commit y aprobación independiente. Se integra mediante merge commit para conservar la serie. `Closes #1` cierra el issue cuando el PR se integra, no al abrirlo.

## Cuándo está cumplido el issue

Todos los bloques están incorporados; los hashes del paquete coinciden; el plan de pruebas de esta fase tiene evidencia real; el PR enlaza este issue, recibe revisión y se integra. En ese momento se desbloquea la entrega siguiente. Copiar archivos y abrir un PR no sustituye la aprobación del revisor.
