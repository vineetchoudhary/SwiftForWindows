add_swift_executable(sil-opt
  SILOpt.cpp
  LINK_LIBRARIES
    swiftFrontend
    swiftIRGen
    swiftSILGen
    swiftSILOptimizer
)

swift_install_in_component(tools
    TARGETS sil-opt
    RUNTIME DESTINATION bin)

set_property(TARGET sil-opt APPEND_STRING PROPERTY
    LINK_FLAGS " -Wl,--allow-multiple-definition")
