# Copyright 2017 The LevelDB Authors. All rights reserved.
# Use of this source code is governed by a BSD-style license that can be
# found in the LICENSE file. See the AUTHORS file for names of contributors.

cmake_minimum_required(VERSION 3.15)
# Keep the version below in sync with the one in db.h
project(leveldb VERSION 1.23.0 LANGUAGES C CXX)


#
option(BUILD_SHARED_LIBS "" ON)

if(LEVELDB_ARM)
  set(CMAKE_C_COMPILER arm-linux-gnueabihf-gcc)
  set(CMAKE_CXX_COMPILER arm-linux-gnueabihf-g++)
elseif(LEVELDB_AARCH64)
  set(CMAKE_C_COMPILER aarch64-linux-gnu-gcc)
  set(CMAKE_CXX_COMPILER aarch64-linux-gnu-g++)
endif()

set(LEVELDB_ROOT "./leveldb/")
include_directories(${LEVELDB_ROOT})

if(NOT APPLE)
  set(CMAKE_EXE_LINKER_FLAGS "-static-libgcc -static-libstdc++")
endif()


# C standard can be overridden when this is used as a sub-project.
if(NOT CMAKE_C_STANDARD)
  # This project can use C11, but will gracefully decay down to C89.
  set(CMAKE_C_STANDARD 11)
  set(CMAKE_C_STANDARD_REQUIRED OFF)
  set(CMAKE_C_EXTENSIONS OFF)
endif(NOT CMAKE_C_STANDARD)

# C++ standard can be overridden when this is used as a sub-project.
if(NOT CMAKE_CXX_STANDARD)
  # This project requires C++11.
  set(CMAKE_CXX_STANDARD 11)
  set(CMAKE_CXX_STANDARD_REQUIRED ON)
  set(CMAKE_CXX_EXTENSIONS OFF)
endif(NOT CMAKE_CXX_STANDARD)

if (WIN32)
  set(LEVELDB_PLATFORM_NAME LEVELDB_PLATFORM_WINDOWS)
  # TODO(cmumford): Make UNICODE configurable for Windows.
  add_definitions(-D_UNICODE -DUNICODE)
else (WIN32)
  set(LEVELDB_PLATFORM_NAME LEVELDB_PLATFORM_POSIX)
endif (WIN32)

option(LEVELDB_BUILD_TESTS "Build LevelDB's unit tests" ON)
option(LEVELDB_BUILD_BENCHMARKS "Build LevelDB's benchmarks" ON)
option(LEVELDB_INSTALL "Install LevelDB's header and library" ON)

include(CheckIncludeFile)
check_include_file("unistd.h" HAVE_UNISTD_H)

include(CheckCXXSymbolExists)
# Using check_cxx_symbol_exists() instead of check_c_symbol_exists() because
# we're including the header from C++, and feature detection should use the same
# compiler language that the project will use later. Principles aside, some
# versions of do not expose fdatasync() in <unistd.h> in standard C mode
# (-std=c11), but do expose the function in standard C++ mode (-std=c++11).
check_cxx_symbol_exists(fdatasync "unistd.h" HAVE_FDATASYNC)
check_cxx_symbol_exists(F_FULLFSYNC "fcntl.h" HAVE_FULLFSYNC)
check_cxx_symbol_exists(O_CLOEXEC "fcntl.h" HAVE_O_CLOEXEC)

if(CMAKE_CXX_COMPILER_ID STREQUAL "MSVC")
  # Disable C++ exceptions.
  string(REGEX REPLACE "/EH[a-z]+" "" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /EHs-c-")
  add_definitions(-D_HAS_EXCEPTIONS=0)

  # Disable RTTI.
  string(REGEX REPLACE "/GR" "" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /GR-")
else(CMAKE_CXX_COMPILER_ID STREQUAL "MSVC")
  # Enable strict prototype warnings for C code in clang and gcc.
  if(NOT CMAKE_C_FLAGS MATCHES "-Wstrict-prototypes")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -Wstrict-prototypes")
  endif(NOT CMAKE_C_FLAGS MATCHES "-Wstrict-prototypes")

  # Disable C++ exceptions.
  string(REGEX REPLACE "-fexceptions" "" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fno-exceptions")

  # Disable RTTI.
  string(REGEX REPLACE "-frtti" "" CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS}")
  set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fno-rtti")
endif(CMAKE_CXX_COMPILER_ID STREQUAL "MSVC")

# Test whether -Wthread-safety is available. See
# https://clang.llvm.org/docs/ThreadSafetyAnalysis.html
include(CheckCXXCompilerFlag)
check_cxx_compiler_flag(-Wthread-safety HAVE_CLANG_THREAD_SAFETY)

# Used by googletest.
check_cxx_compiler_flag(-Wno-missing-field-initializers
                        LEVELDB_HAVE_NO_MISSING_FIELD_INITIALIZERS)

include(CheckCXXSourceCompiles)

# Test whether C++17 __has_include is available.
check_cxx_source_compiles("
#if defined(__has_include) &&  __has_include(<string>)
#include <string>
#endif
int main() { std::string str; return 0; }
" HAVE_CXX17_HAS_INCLUDE)

set(LEVELDB_PUBLIC_INCLUDE_DIR "${LEVELDB_ROOT}include/leveldb")
set(LEVELDB_PORT_CONFIG_DIR "${LEVELDB_ROOT}include/port")

configure_file(
  "${LEVELDB_ROOT}port/port_config.h.in"
  "${PROJECT_BINARY_DIR}/${LEVELDB_ROOT}${LEVELDB_PORT_CONFIG_DIR}/port_config.h"
)

include_directories(
  "${PROJECT_BINARY_DIR}/${LEVELDB_ROOT}include"
  "."
)

# Only export LEVELDB_EXPORT symbols from the shared library.
add_compile_options(-fvisibility=hidden)

# Must be included before CMAKE_INSTALL_INCLUDEDIR is used.
include(GNUInstallDirs)

add_library(leveldb "")
target_sources(leveldb
  PRIVATE
    "${PROJECT_BINARY_DIR}/${LEVELDB_ROOT}${LEVELDB_PORT_CONFIG_DIR}/port_config.h"
    "${LEVELDB_ROOT}db/builder.cc"
    "${LEVELDB_ROOT}db/builder.h"
    "${LEVELDB_ROOT}db/c.cc"
    "${LEVELDB_ROOT}db/db_impl.cc"
    "${LEVELDB_ROOT}db/db_impl.h"
    "${LEVELDB_ROOT}db/db_iter.cc"
    "${LEVELDB_ROOT}db/db_iter.h"
    "${LEVELDB_ROOT}db/dbformat.cc"
    "${LEVELDB_ROOT}db/dbformat.h"
    "${LEVELDB_ROOT}db/dumpfile.cc"
    "${LEVELDB_ROOT}db/filename.cc"
    "${LEVELDB_ROOT}db/filename.h"
    "${LEVELDB_ROOT}db/log_format.h"
    "${LEVELDB_ROOT}db/log_reader.cc"
    "${LEVELDB_ROOT}db/log_reader.h"
    "${LEVELDB_ROOT}db/log_writer.cc"
    "${LEVELDB_ROOT}db/log_writer.h"
    "${LEVELDB_ROOT}db/memtable.cc"
    "${LEVELDB_ROOT}db/memtable.h"
    "${LEVELDB_ROOT}db/repair.cc"
    "${LEVELDB_ROOT}db/skiplist.h"
    "${LEVELDB_ROOT}db/snapshot.h"
    "${LEVELDB_ROOT}db/table_cache.cc"
    "${LEVELDB_ROOT}db/table_cache.h"
    "${LEVELDB_ROOT}db/version_edit.cc"
    "${LEVELDB_ROOT}db/version_edit.h"
    "${LEVELDB_ROOT}db/version_set.cc"
    "${LEVELDB_ROOT}db/version_set.h"
    "${LEVELDB_ROOT}db/write_batch_internal.h"
    "${LEVELDB_ROOT}db/write_batch.cc"
    "${LEVELDB_ROOT}port/port_stdcxx.h"
    "${LEVELDB_ROOT}port/port.h"
    "${LEVELDB_ROOT}port/thread_annotations.h"
    "${LEVELDB_ROOT}table/block_builder.cc"
    "${LEVELDB_ROOT}table/block_builder.h"
    "${LEVELDB_ROOT}table/block.cc"
    "${LEVELDB_ROOT}table/block.h"
    "${LEVELDB_ROOT}table/filter_block.cc"
    "${LEVELDB_ROOT}table/filter_block.h"
    "${LEVELDB_ROOT}table/format.cc"
    "${LEVELDB_ROOT}table/format.h"
    "${LEVELDB_ROOT}table/iterator_wrapper.h"
    "${LEVELDB_ROOT}table/iterator.cc"
    "${LEVELDB_ROOT}table/merger.cc"
    "${LEVELDB_ROOT}table/merger.h"
    "${LEVELDB_ROOT}table/table_builder.cc"
    "${LEVELDB_ROOT}table/table.cc"
    "${LEVELDB_ROOT}table/two_level_iterator.cc"
    "${LEVELDB_ROOT}table/two_level_iterator.h"
    "${LEVELDB_ROOT}util/arena.cc"
    "${LEVELDB_ROOT}util/arena.h"
    "${LEVELDB_ROOT}util/bloom.cc"
    "${LEVELDB_ROOT}util/cache.cc"
    "${LEVELDB_ROOT}util/coding.cc"
    "${LEVELDB_ROOT}util/coding.h"
    "${LEVELDB_ROOT}util/comparator.cc"
    "${LEVELDB_ROOT}util/crc32c.cc"
    "${LEVELDB_ROOT}util/crc32c.h"
    "${LEVELDB_ROOT}util/env.cc"
    "${LEVELDB_ROOT}util/filter_policy.cc"
    "${LEVELDB_ROOT}util/hash.cc"
    "${LEVELDB_ROOT}util/hash.h"
    "${LEVELDB_ROOT}util/logging.cc"
    "${LEVELDB_ROOT}util/logging.h"
    "${LEVELDB_ROOT}util/mutexlock.h"
    "${LEVELDB_ROOT}util/no_destructor.h"
    "${LEVELDB_ROOT}util/options.cc"
    "${LEVELDB_ROOT}util/random.h"
    "${LEVELDB_ROOT}util/status.cc"

  # Only CMake 3.3+ supports PUBLIC sources in targets exported by "install".
  $<$<VERSION_GREATER:CMAKE_VERSION,3.2>:PUBLIC>
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/c.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/cache.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/comparator.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/db.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/dumpfile.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/env.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/export.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/filter_policy.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/iterator.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/options.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/slice.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/status.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/table_builder.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/table.h"
    "${LEVELDB_PUBLIC_INCLUDE_DIR}/write_batch.h"
)

set_property(TARGET leveldb PROPERTY
  MSVC_RUNTIME_LIBRARY "MultiThreaded")

if (LEVELDB32)
  target_compile_options(leveldb PRIVATE -m32)
  target_link_options(leveldb PRIVATE -m32)
endif (LEVELDB32)

if (WIN32)
  target_sources(leveldb
    PRIVATE
      "${LEVELDB_ROOT}util/env_windows.cc"
      "${LEVELDB_ROOT}util/windows_logger.h"
  )
else (WIN32)
  target_sources(leveldb
    PRIVATE
      "${LEVELDB_ROOT}util/env_posix.cc"
      "${LEVELDB_ROOT}util/posix_logger.h"
  )
endif (WIN32)

# MemEnv is not part of the interface and could be pulled to a separate library.
target_sources(leveldb
  PRIVATE
    "${LEVELDB_ROOT}helpers/memenv/memenv.cc"
    "${LEVELDB_ROOT}helpers/memenv/memenv.h"
)

target_include_directories(leveldb
  PUBLIC
    $<BUILD_INTERFACE:${PROJECT_SOURCE_DIR}/${LEVELDB_ROOT}/include>
    $<INSTALL_INTERFACE:${CMAKE_INSTALL_INCLUDEDIR}>
)

set_target_properties(leveldb
  PROPERTIES VERSION ${PROJECT_VERSION} SOVERSION ${PROJECT_VERSION_MAJOR})

target_compile_definitions(leveldb
  PRIVATE
    # Used by include/export.h when building shared libraries.
    LEVELDB_COMPILE_LIBRARY
    # Used by port/port.h.
    ${LEVELDB_PLATFORM_NAME}=1
)
if (NOT HAVE_CXX17_HAS_INCLUDE)
  target_compile_definitions(leveldb
    PRIVATE
      LEVELDB_HAS_PORT_CONFIG_H=1
  )
endif(NOT HAVE_CXX17_HAS_INCLUDE)

target_compile_definitions(leveldb
PUBLIC
  # Used by include/export.h.
  LEVELDB_SHARED_LIBRARY
)

if(HAVE_CLANG_THREAD_SAFETY)
  target_compile_options(leveldb
    PUBLIC
      -Werror -Wthread-safety)
endif(HAVE_CLANG_THREAD_SAFETY)

if(HAVE_CRC32C)
  target_link_libraries(leveldb crc32c -Bstatic)
endif(HAVE_CRC32C)
if(HAVE_SNAPPY)
  target_link_libraries(leveldb snappy -Bstatic)
endif(HAVE_SNAPPY)
if(HAVE_TCMALLOC)
  target_link_libraries(leveldb tcmalloc -Bstatic)
endif(HAVE_TCMALLOC)

# Needed by port_stdcxx.h
find_package(Threads REQUIRED)
target_link_libraries(leveldb Threads::Threads -Bstatic)
