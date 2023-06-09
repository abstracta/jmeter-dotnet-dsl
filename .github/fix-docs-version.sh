#!/bin/bash
#
# This script takes care of setting proper JMeter DSL version in docs.
#
set -eo pipefail

VERSION=$1

update_file_versions() {
  local VERSION="$1"
  local FILE="$2"
  sed -i "s/--version [0-9.]\+/--version ${VERSION}/g" "${FILE}"
}

update_file_versions ${VERSION} README.md

find docs -name "*.md" -not -path "*/node_modules/*" | while read DOC_FILE; do
  update_file_versions ${VERSION} ${DOC_FILE}
done