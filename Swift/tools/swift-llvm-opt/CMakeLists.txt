add_swift_executable(swift-llvm-opt
  LLVMOpt.cpp
  LINK_LIBRARIES
  swiftIRGen

  # Swift libraries included to appease the linker on linux.
  swiftSema
  swiftAST

  # Clang libraries included to appease the linker on linux.
  clangBasic
  clangCodeGen
)

swift_install_in_component(tools
    TARGETS swift-llvm-opt
    RUNTIME DESTINATION bin)

set_property(TARGET swift-llvm-opt APPEND_STRING PROPERTY
    LINK_FLAGS " -Wl,--allow-multiple-definition")
