@echo off

cmake -DCMAKE_BUILD_TYPE=Release -G "Visual Studio 16 2019" -A Win32 -S . -B ".build32"
cmake --build .build32 --config Release
xcopy .build32\Release\*.dll runtimes\win-x86\native\ /R /Y

cmake -DCMAKE_BUILD_TYPE=Release -G "Visual Studio 16 2019" -A x64 -S . -B ".build64"
cmake --build .build64 --config Release
xcopy .build64\Release\*.dll runtimes\win-x64\native\ /R /Y
