#!/bin/sh

cmake -DCMAKE_BUILD_TYPE=Release -DLEVELDB32=1 -S . -B ".build32"
cmake --build .build32 --config Release
cp .build32/libleveldb.so runtimes/linux-x86/native/

cmake -DCMAKE_BUILD_TYPE=Release -S . -B ".build64"
cmake --build .build64 --config Release
cp .build64/libleveldb.so runtimes/linux-x64/native/

cmake -DCMAKE_BUILD_TYPE=Release -DLEVELDB_ARM=1 -S . -B ".build-arm"
cmake --build .build-arm --config Release
cp .build-arm/libleveldb.so runtimes/linux-arm/native/

cmake -DCMAKE_BUILD_TYPE=Release -DLEVELDB_AARCH64=1 -S . -B ".build-aarch64"
cmake --build .build-aarch64 --config Release
cp .build-aarch64/libleveldb.so runtimes/linux-arm64/native/
