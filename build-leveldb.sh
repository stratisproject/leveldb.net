#!/bin/sh

cmake -DCMAKE_BUILD_TYPE=Release -DLEVELDB32=1 -S . -B ".build32"
cmake --build .build32 --config Release
cp .build32/libleveldb.so runtimes/linux-x86/native/

cmake -DCMAKE_BUILD_TYPE=Release -S . -B ".build64"
cmake --build .build64 --config Release
cp .build64/libleveldb.so runtimes/linux-x64/native/
