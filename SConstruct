#!/usr/bin/env python
import os
from SCons.Script import *

GODOT_CPP_PATH = "godot-cpp"

env = SConscript(os.path.join(GODOT_CPP_PATH, "SConstruct"))

SRC_DIR = "src"
sources = Glob(os.path.join(SRC_DIR, "*.cpp"))

BUILD_DIR = "final/bin"
if not os.path.exists(BUILD_DIR):
    os.makedirs(BUILD_DIR)

lib_name = "procedural_generator"
if env["platform"] == "macos":
    output_path = os.path.join(BUILD_DIR, f"{lib_name}.dylib")
elif env["platform"] == "windows":
    output_path = os.path.join(BUILD_DIR, f"{lib_name}.dll")
else:  # Linux / others
    output_path = os.path.join(BUILD_DIR, f"{lib_name}.so")

library = env.SharedLibrary(target=output_path, source=sources)

Default(library)
