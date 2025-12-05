# 2D Tiled Procedural Generator

**Team:**
Dorian Sanchez, Gareth Carew, Kristopher Thomas

---

## *Description*
This Godot 4.4 extension is created for procedural generation. It generates an **infinitely expanding** scene around the player as they move.


The extension adds a new node accessible in the Godot editor. The node exposes several configurable parameters:

- A TileMapLayer node to generate on
- A Node used as the player reference
- An integer for the generation radius
- An integer for the generation seed

When run, the generator produces a **deterministic map** within a chunk-based radius around the player.

---

## *Features*

- Deterministic generation
- Infinite expansion
- Load radius
- Load center tracker
- Changeable seeds

---

## *Installation and Build Instructions*

### Cloning the Repository

The repository includes a submodule (`godot-cpp`). To clone it correctly, use:

```
git clone --recurse-submodules https://github.com/DorianOklahoma/IMG-420-Final
```

If you have already cloned the repository **without** submodules, initialize them with:

```
git submodule update --init --recursive
```

---

### Building the Project

After cloning, build the GDExtension by running SCons from the root of the repository:

#### **macOS**
```
scons platform=macos
```

#### **Windows**
```
scons platform=windows
```

#### **Linux / Other**
```
scons platform=linux
```

 The build output will be placed in `final/bin/`.

---

## *Running the Game*

To play the game:

1. In Godot, build the project using the **hammer** button.
2. When the build completes, press the **play** button.
3. Press the **Start** button to begin.

---

## *Controls and Gameplay*

- **Arrow keys** — Move the player
- **Space (no lightning bolt)** — Mine through walls
- **Space (with lightning bolt)** — Kill enemies

Your goal is to survive as long as possible while avoiding enemies.

---

## *Known Issues*

- Enemies sometimes spawn inside walls
- The player can mine infinitely
- Items sometimes despawn randomly

---

## *Future Improvements*

- Ensure enemies spawn within valid map bounds
- Add mining limiter
- Add room-size parameter to the extension

---

## *Credits*

- Godot documentation
- https://benhoyt.com/writings/hash-table-in-c/

- https://docs.godotengine.org/en/stable/classes/class_hashingcontext.htmlV

---

## *Demo Video Link*

- youtube.com