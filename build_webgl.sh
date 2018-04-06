#!/usr/bin/env bash

set -ex

tankmania_dir="$( cd "$(dirname "$0")" ; pwd -P )"

rm -rfv "./Build"
powershell "cmd /C build-webgl.bat"
