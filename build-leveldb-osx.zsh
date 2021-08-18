#!/bin/zsh

cmake -DCMAKE_BUILD_TYPE=Release -S . -B ".build-osx"
cmake --build .build-osx --config Release
cp .build-osx/libleveldb.dylib runtimes/osx-x64/native/
