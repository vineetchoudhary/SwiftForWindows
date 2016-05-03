/*

In my opnion, there are three kinds of platform specific implementation factors in performing '_initializeCallbacksToInspectDylib()'.

First, the function for seaching loaded dls/images/DLLs is diffrent for every platform.
The functions are _dyld_register_func_for_add_image(), dl_iterate_phdr(), android_iterate_libs(), _swift_dl_iterate_phdr(). Some of these are supported by system library (Apple , Linux) and others are implemented for porting (Android, Cygwin).
For that reason, there are conditional preprocessing in two '_initializeCallbacksToInspectDylib()'.

Second, the callback function which calling by the searching function is different.
Although the callback function name is same for each platform, the prototype is different. The reason why is because the different searching function specifies different prototype of callback.

Finally, the method for inspecting a section of dl/image/DLL is different.
This method is directly supported by system library getsectiondata() in Apple, or implemented with POSIX dlsym() and linker script in Linux/Android, or implemented with accessing PE/COFF structure in Cygwin. Except Apple, they all use POSIX dlopen()/dlclose() for accessing a dl/image/DLL.

I think it is most important to generalize two '_initializeCallbacksToInspectDylib()' from where the duplication is begining for every platform.

How about introducing general function _swift_inspectDylibs() as follows?

*/

/////////////////////////////////////////////////

// Inspect Dynamic Libraries, and if a library has the section of 'sectionName'
// then call fnAddImageBlock() for the section block.
void _swift_inspectDylibs(void (*fnAddImageBlock)(const uint8_t *, size_t),
                          const char *sectionName);
// MetadataLookup.cpp
static void _initializeCallbacksToInspectDylib() {
  _swift_inspectDylibs(_addImageTypeMetadataRecordsBlock,
                       SWIFT_TYPE_METADATA_SECTION);
}

// ProtocolConformance.cpp
static void _initializeCallbacksToInspectDylib() {
  _swift_inspectDylibs(_addImageProtocolConformancesBlock,
                       SWIFT_PROTOCOL_CONFORMANCES_SECTION);
}

// Common Structure
struct InspectArgs {
  void (*fnAddImageBlock)(const uint8_t *, size_t);
  const char *sectionName;
};

#if defined(__APPLE__) && defined(__MACH__)

thread_local  InspectArgs _inspectArgs;

// Generalized function _addImageProtocolConformances and _addImageMetadataConformances
int _inspectOneDylib(const mach_header *mh, intptr_t vmaddr_slide) {
  // use _inspectArgs.fnAddImageBlock(), _inspectArgs.sectionName
  
  .....
}

void _swift_inspectDylibs(void (*fnAddImageBlock)(const uint8_t *, size_t),
                          const char *sectionName) {
  _inspectArgs = {fnAddImageBlock, sectionName};
  _dyld_register_func_for_add_image(_inspectOneDylib);
}

#elif defined(__ELF__)

// Generalized function _addImageProtocolConformances and _addImageMetadataConformances
int _inspectOneDylib(struct dl_phdr_info *info, size_t size, void *data) {
  InspectArgs *inspectArgs = (InspectArgs *)data;
  // use inspectArgs->fnAddImageBlock(), _inspectArgs->sectionName
  
  .....
}

void _swift_inspectDylibs(void (*fnAddImageBlock)(const uint8_t *, size_t),
                          const char *sectionName) {

  InspectArgs inspectArgs = {fnAddImageBlock, sectionName};
  dl_iterate_phdr(_inspectOneDylib, &inspectArgs);
}
#elif defined(__ANDROID__)

// Generalized function _addImageProtocolConformances and _addImageMetadataConformances
int _inspectOneDylib(const char *name, void *data) {
  InspectArgs *inspectArgs = (InspectArgs *)data;
  // use inspectArgs->fnAddImageBlock(), _inspectArgs->sectionName
  
  .....
}

void _swift_inspectDylibs(void (*fnAddImageBlock)(const uint8_t *, size_t),
                          const char *sectionName) {
  InspectArgs inspectArgs = {fnAddImageBlock, sectionName};
  
  // Some modification is needed for second parameter.
  android_iterate_libs(_inspectOneDylib, &inspectArgs);
}

#elif defined(__CYGWIN__)

// Generalized function _addImageProtocolConformances and _addImageMetadataConformances
int _inspectOneDylib(struct dl_phdr_info *info, void *data) {
  InspectArgs *inspectArgs = (InspectArgs *)data;
  // use inspectArgs->fnAddImageBlock(), _inspectArgs->sectionName

  .....
}

void _swift_inspectDylibs(void (*fnAddImageBlock)(const uint8_t *, size_t),
                          const char *sectionName) {                             
  InspectArgs inspectArgs = {fnAddImageBlock, sectionName};
  _swift_dl_iterate_phdr(_inspectOneDylib, &inspectArgs);
}
#else
# error No known mechanism to inspect dynamic libraries on this platform.
#endif



