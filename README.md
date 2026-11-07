# La Resurrección del Huáscar

Campaña naval 2D en Unity 6000.6.8f1. Presentación ilustrada a 1920×1080.

## Reconstrucción de la entrega existente

Este historial incorpora por etapas el juego ya implementado. [Tablero](https://github.com/users/frix241/projects/2) · [Guías](docs/equipo/00-LEEME.md). No contiene tareas de desarrollo futuro.

| Orden | Responsable | Entrega | Issue |
|---|---|---|---|
| 1 | Albert | Configuración, CI y documentación | #1 |
| 2 | Mauro | Núcleo, progreso y tests .NET/EditMode | #2 |
| 3 | Sergio | Runtime, mapa, escena y PlayMode | #3 |
| 4 | Christian | Arte, UI raster, audio y herramientas | #4 |

Cada fase se integra antes de crear la siguiente rama desde master. El proyecto Unity completo se obtiene después de la fase 4. Las fases parciales no constituyen una build jugable. Los archivos del juego reproducen el snapshot dbdce75.

## Ejecutar y comprobar

Después de la fase 4, abre la raíz con Unity 6000.6.8f1, entra a Assets/_LRH/Scenes/Main.unity y pulsa Play. W/S avanzan, A/D giran, ratón apunta y clic dispara.

Desde la fase 2:

```bash
dotnet run --configuration Release --project Tests/CoreChecks/CoreChecks.csproj
python3 Tools/check_repository.py
```

La suite .NET contiene 33 comprobaciones. Las pruebas Unity requieren el Editor; consulta el plan del issue y adjunta sus resultados reales. Builds, cachés y ZIPs se distribuyen fuera de Git.
