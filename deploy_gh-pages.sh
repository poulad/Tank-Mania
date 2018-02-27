#!/usr/bin/env bash

set -ex

tankmania_dir="$( cd "$(dirname "$0")" ; pwd -P )"

# rm -rfv "./Build"
# powershell "cmd /C build-webgl.bat"


cd "../Tank-Mania_gh-pages"
rm -rfv ./*
cp -rv "$tankmania_dir"/Build/* .


git add .
git config commit.gpgSign false
git commit -m "Build for github pages"
git config commit.gpgSign true
git push