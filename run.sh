#!/usr/bin/env bash
# run.sh — carga .env y corre la API
set -euo pipefail
set -a; source .env; set +a
dotnet run --project src/Api "$@"