add_swift_executable(swift-demangle
  swift-demangle.cpp
  LINK_LIBRARIES swiftBasic
  COMPONENT_DEPENDS support)

swift_install_in_component(compiler
    TARGETS swift-demangle
    RUNTIME DESTINATION "bin")

set_property(TARGET swift-demangle APPEND_STRING PROPERTY
    LINK_FLAGS " -Wl,--allow-multiple-definition")
