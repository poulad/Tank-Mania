#!/usr/bin/env bash

set -ex

tankmania_dir="$( cd "$(dirname "$0")" ; pwd -P )"

if [ -d "./Build" ]
then
    rm -rv "./Build"
fi
"C:\Program Files\Unity\Editor\Unity.exe" -quit -batchmode -logFile Temp/build-webgl-logs.txt -executeMethod WebGLBuilder.Build

cd "../Tank-Mania_gh-pages"
rm -rv ./*
cp -rv ../Tank-Mania/Build/* .


git add .
git config commit.gpgSign false
git commit -m "Build for github pages"
git config commit.gpgSign true
git push
