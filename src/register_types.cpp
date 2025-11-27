#include "register_types.h"
#include "procedural_generator_2d.h"

#include <gdextension_interface.h>
#include <godot_cpp/godot.hpp>

using namespace godot;

void initialize_procedural_generator_module(ModuleInitializationLevel p_level) {
    if (p_level != MODULE_INITIALIZATION_LEVEL_SCENE) return;

    GDREGISTER_RUNTIME_CLASS(ProceduralGenerator2D);
}

void uninitialize_procedural_generator_module(ModuleInitializationLevel p_level) {
    if (p_level != MODULE_INITIALIZATION_LEVEL_SCENE) return;
}

extern "C" {

GDExtensionBool GDE_EXPORT procedural_generator_library_init(
    GDExtensionInterfaceGetProcAddress p_get_proc_address,
    const GDExtensionClassLibraryPtr p_library,
    GDExtensionInitialization *r_initialization
) {
    godot::GDExtensionBinding::InitObject init_obj(
        p_get_proc_address, 
        p_library, 
        r_initialization
    );

    init_obj.register_initializer(initialize_procedural_generator_module);
    init_obj.register_terminator(uninitialize_procedural_generator_module);
    init_obj.set_minimum_library_initialization_level(MODULE_INITIALIZATION_LEVEL_SCENE);

    return init_obj.init();
}

} // extern "C"
