#!/bin/bash
#
# This script takes care of building and deploying the vuepress documentation to github pages.
#
set -eo pipefail

cd docs

pnpm install && pnpm build

cd .vuepress/dist

EMAIL="$(git log --format='%ae' HEAD^!)"
USERNAME="$(git log --format='%an' HEAD^!)"
git init
git config --local user.email "$EMAIL"
git config --local user.name  "$USERNAME"
git add .
git commit -m '[skip ci] Deploy docs to GitHub pages'

git push -f https://git:${ACCESS_TOKEN}@github.com/abstracta/jmeter-dotnet-dsl.git master:gh-pages

cd $GITHUB_WORKSPACE
