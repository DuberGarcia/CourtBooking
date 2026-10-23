#!/usr/bin/env bash
# ef.sh — carga .env y ejecuta dotnet ef contra Infrastructure
set -euo pipefail

set -a          # todo lo que se defina a partir de aquí se exporta como variable de entorno
source .env
set +a

dotnet ef "$@" --project src/Infrastructure --startup-project src/Infrastructure