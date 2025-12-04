# IMG-420-Final

A Godot 4.4 project with a custom GDExtension for procedural tilemap generation.

---

## Cloning the Repository

The repository includes a submodule (`godot-cpp`). To clone it correctly, use:

```
git clone --recurse-submodules https://github.com/DorianOklahoma/IMG-420-Final
```

If you have already cloned the repository **without** submodules, initialize them with:

```
git submodule update --init --recursive
```

---

## Building the Project

After cloning, build the GDExtension by running SCons from the root of the repository:

### macOS
```
scons platform=macos
```

### Windows
```
scons platform=windows
```

### Linux / Other
```
scons platform=linux
```

The build output will be placed in `final/bin/`.

---

## Importing into Godot

Once the build is complete:

1. Open Godot.
2. Import the project by opening the `final/project.godot` file.
3. The GDExtension (`procedural_generator`) will be ready to use in the project.

---

## Modifying the Extension

The extension source code is located in the `src/` folder.  
You can:

- Modify existing functionality.
- Add new nodes or features.
- Rebuild using SCons after making changes.
