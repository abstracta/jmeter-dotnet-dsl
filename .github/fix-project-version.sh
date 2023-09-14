#!/bin/bash
#
# This script takes care of setting proper JMeter DSL version in projects.
#
set -eo pipefail

VERSION=$1

update_file_versions() {
  local VERSION="$1"
  local FILE="$2"
  local BASE_VERSION="${VERSION%%-*}"
  local ASSEMBLY_VERSION="${BASE_VERSION}.0"
  if [ $(echo "${BASE_VERSION}" | awk -F"." '{print NF-1}') == 1 ]; then
    ASSEMBLY_VERSION="${ASSEMBLY_VERSION}.0"
  fi
  sed -i "s/<AssemblyVersion>[^<]\+<\/AssemblyVersion>/<AssemblyVersion>${ASSEMBLY_VERSION}<\/AssemblyVersion>/g" "${FILE}"
  sed -i "s/<Version>[^<]\+<\/Version>/<Version>${VERSION}<\/Version>/g" "${FILE}"
  sed -i "s/<FileVersion>[^<]\+<\/FileVersion>/<FileVersion>${BASE_VERSION}<\/FileVersion>/g" "${FILE}"
}

find . -name "*.csproj" | while read DOC_FILE; do
  update_file_versions ${VERSION} ${DOC_FILE}
done