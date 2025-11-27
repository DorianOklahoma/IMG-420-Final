#ifndef PROCEDURAL_GENERATOR_2D_H
#define PROCEDURAL_GENERATOR_2D_H

#include <godot_cpp/classes/node2d.hpp>
#include <godot_cpp/classes/tile_map.hpp>
#include <godot_cpp/variant/utility_functions.hpp>
#include <godot_cpp/core/class_db.hpp>

using namespace godot;

class ProceduralGenerator2D : public Node2D {
    GDCLASS(ProceduralGenerator2D, Node2D);

protected:
    static void _bind_methods();

public:
    ProceduralGenerator2D();
    ~ProceduralGenerator2D();

    // Method to trigger procedural generation
    void generate();

    // Assign TileMap node
    void set_tilemap_node(Node *tilemap_node);
    Node* get_tilemap_node() const;

private:
    Node* _tilemap_node;
};

#endif // PROCEDURAL_GENERATOR_2D_H
