
Install cygwin64 2.4.1
----------------------
```
 Devel/git                2.7.0-1
      /cmake              3.3.2-1
      /clang              3.7.1-1 
      /gcc-core           5.3.0-3
      /gcc-g++            5.3.0-3
      /swig               3.0.7-1
      /pkg-config         0.29-1
  Libs/libuuid-devel      2.25.2-2
      /libicu-devel       56.1-1
      /libedit-devel      20130712-1
      /libxml2-devel      2.9.3-1
      /libsqlite3_0       3.11.0-2
      /libstdc++6         5.3.0-3
      /libncurses-devel   6.0-4.20160305
      /libiconv-devel     1.14-3
```

Patch gcc header
----------------
  
  The header file 'c++config.h' must be modified.
```
  Edit /usr/lib/gcc/x86_64-pc-cygwin/5.3.0/include/c++/x86_64-pc-cygwin/bits/c++config.h Line 980
    Insert three lines which undefine _GLIBCXX_HAVE_TLS as follows
      #define _GLIBCXX_HAVE_TLS 1
->    #if defined (__clang__)
->    #undef _GLIBCXX_HAVE_TLS
->    #endif    
``` 

Patch clang header
------------------

  The header file 'limits.h' must be modified.
```
  Edit /usr/lib/clang/3.7.1/include/limits.h Line 28
    Insert a line which defines _GCC_NEXT_LIMITS_H as follows
      #ifndef __CLANG_LIMITS_H
      #define __CLANG_LIMITS_H

->    #define _GCC_NEXT_LIMITS_H
      /* The system's limits.h may, in turn, try to #include_next GCC's limits.h.
         Avert this #include_next madness. */
```

Patch cmake
-----------

  Without this patch, the import libraries will not be generated.
  (This patch is already applied to the CMake nightly build, but not yet to this 3.3.2.)
```
   Create two files as follows (each has one line)
 
     /usr/share/cmake-3.3.2/Modules/Platform/CYGWIN-Clang-C.cmake
       include(Platform/CYGWIN-GNU-C)
 
     /usr/share/cmake-3.3.2/Modules/Platform/CYGWIN-Clang-CXX.cmake
       include(Platform/CYGWIN-GNU-CXX)
```

Download sources
----------------
```
  export WORK_DIR=<working directory>
  cd $WORK_DIR
  
  git clone https://github.com/tinysun212/swift-cygwin.git swift
  git clone https://github.com/tinysun212/swift-llvm-cygwin.git llvm
  git clone https://github.com/tinysun212/swift-clang-cygwin.git clang
  git clone https://github.com/apple/swift-cmark.git cmark
  git clone https://github.com/ninja-build/ninja.git

  cd swift; git checkout swift-cygwin-20160418 ; cd ..
  cd llvm; git checkout swift-cygwin-20160418 ; cd ..
  cd clang; git checkout swift-cygwin-20160418 ; cd ..
  cd cmark; git checkout 6873b; cd ..
  cd ninja; git checkout 2eb1cc9; cd ..
```

Build
-----
```
  mkdir $WORK_DIR/build/Ninja-ReleaseAssert/swift-cygwin-x86_64/lib/swift/cygwin
  cd $WORK_DIR/build/Ninja-ReleaseAssert/swift-cygwin-x86_64/lib/swift
  ln -s cygwin windows
  
  cd $WORK_DIR/swift
  utils/build-script -R
```
  
Build Troubleshoot
------------------
  The build processing should be completed without manual work.
