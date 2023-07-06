#!/bin/bash
#
# This script checks that the version number of the release is an expected one, and avoid erroneous releases which don't follow semver
set -eo pipefail

git fetch --tags --quiet
VERSION="$1"
PREV_VERSION=$(git tag --sort=-creatordate | head -1)
PREV_VERSION=${PREV_VERSION:-v0.0}
PREV_VERSION=${PREV_VERSION#v}
PREV_MAJOR="${PREV_VERSION%%.*}"
PREV_VERSION="${PREV_VERSION#*.}"
PREV_MINOR="${PREV_VERSION%%.*}"
PREV_PATCH="${PREV_VERSION#*.}"
if [[ "$PREV_VERSION" == "$PREV_PATCH" ]]; then
   PREV_PATCH="0"
fi

[[ "$VERSION" == "$PREV_MAJOR.$PREV_MINOR.$((PREV_PATCH + 1))" || "$VERSION" == "$PREV_MAJOR.$((PREV_MINOR + 1))" || "$VERSION" == "$((PREV_MAJOR + 1)).0" ]]
