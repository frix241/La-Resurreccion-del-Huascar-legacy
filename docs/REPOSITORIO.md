# Flujo de integración

Orden de integración: #1 Albert, #2 Mauro, #3 Sergio, #4 Christian. Cada paquete incorpora archivos existentes, verifica sus hashes y conserva sus metadatos Unity. La importación completa del PR #5 queda como referencia del snapshot, reemplazada por el flujo por etapas.

Master exige ambos checks, aprobación independiente y conversaciones resueltas. Merge commit conserva la serie. Antes de incorporar Tests/CoreChecks, el job informa explícitamente que las pruebas .NET no están disponibles; después ejecuta la suite. La validación completa Unity corresponde a la cuarta entrega.
