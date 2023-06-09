#!/bin/bash
#
# This script takes care of setting proper JMeter DSL version in projects.
#
set -eo pipefail

VERSION=$1

update_file_versions() {
  local VERSION="$1"
  local FILE="$2"
  local ASSEMBLY_VERSION="${VERSION}.0"
  if [ $(echo "${VERSION}" | awk -F"." '{print NF-1}') == 1 ]; then
    ASSEMBLY_VERSION="${ASSEMBLY_VERSION}.0"
  fi
  sed -i "s/<AssemblyVersion>[0-9.]\+<\/AssemblyVersion>/<AssemblyVersion>${ASSEMBLY_VERSION}<\/AssemblyVersion>/g" "${FILE}"
  sed -i "s/<Version>[0-9.]\+<\/Version>/<Version>${VERSION}<\/Version>/g" "${FILE}"
  sed -i "s/<FileVersion>[0-9.]\+<\/FileVersion>/<FileVersion>${VERSION}<\/FileVersion>/g" "${FILE}"
}

find . -name "*.csproj" | while read DOC_FILE; do
  update_file_versions ${VERSION} ${DOC_FILE}
done