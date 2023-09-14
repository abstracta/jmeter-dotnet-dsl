#!/bin/bash
#
# This script takes care of updating JMeter DSL version in sample project.
#
set -eo pipefail

VERSION=$1

USER_EMAIL="$(git log --format='%ae' HEAD^!)"
USER_NAME="$(git log --format='%an' HEAD^!)"

cd jmeter-dotnet-dsl-sample
sed -i "s/Include=\"Abstracta.JmeterDsl\" Version=\"[0-9.]\+\"/Include=\"Abstracta.JmeterDsl\" Version=\"${VERSION}\"/g" Abstracta.JmeterDsl.Sample/Abstracta.JmeterDsl.Sample.csproj

git add .
git config --local user.email "$USER_EMAIL"
git config --local user.name "$USER_NAME"
git commit -m "Updated Abstracta.JmeterDsl version"
git push origin HEAD:master
cd ..
rm -rf jmeter-dotnet-dsl-sample
