#!/bin/bash
#
# This script takes care of deploying packages to nuget
#
# Required environment variables: NUGET_API_KEY

set -xeo pipefail

push_package() {
  dotnet nuget push "$1" --api-key "${NUGET_API_KEY}" --source https://api.nuget.org/v3/index.json --skip-duplicate
}

find . -name "*.nupkg" -or -name "*.snupkg" | sort | while read PACKAGE; do
  push_package ${PACKAGE}
done
