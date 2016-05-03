add_swift_executable(sil-extract
  SILExtract.cpp
  LINK_LIBRARIES
    swiftFrontend
    swiftSILGen
    swiftSILOptimizer
    swiftSerialization
    swiftClangImporter
)

swift_install_in_component(tools
    TARGETS sil-extract
    RUNTIME DESTINATION "bin")

set_property(TARGET sil-extract APPEND_STRING PROPERTY
    LINK_FLAGS " -Wl,--allow-multiple-definition")
