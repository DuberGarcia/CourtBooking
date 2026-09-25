#!/usr/bin/env bash
set -e

set -a          # a partir de aquí, toda variable que se defina se exporta
source .env     # lee el .env y define sus variables
set +a          # deja de exportar automáticamente

dotnet ef "$@" \
  --project src/CourtBooking.Infrastructure \
  --startup-project src/CourtBooking.Api