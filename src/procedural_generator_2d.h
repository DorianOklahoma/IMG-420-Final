#ifndef PROCEDURAL_GENERATOR_2D_H
#define PROCEDURAL_GENERATOR_2D_H

#include <godot_cpp/classes/node2d.hpp>
#include <godot_cpp/classes/tile_map_layer.hpp>
#include <godot_cpp/classes/node.hpp>
#include <godot_cpp/classes/global_constants.hpp>
#include <godot_cpp/classes/engine.hpp>
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

    void generate();

    // Properties
    void set_tilemap_node(Node *tilemap_layer_node);
    Node *get_tilemap_node() const;

    void set_center_node(Node *node);
    Node *get_center_node() const;

    void set_generation_radius(int radius);
    int get_generation_radius() const;

    void set_seed(int64_t seed);
    int64_t get_seed() const;

private:
    Node *_tilemap_layer_node;
    Node *_center_node;

    int _generation_radius = 20;
    int64_t _seed = 123456789; // default seed

    // PRNG helper that is deterministic per position
    uint32_t position_rand(Vector2i pos, int index = 0) const;

    void generate_paths(TileMapLayer *layer, const Vector2i &center_cell, int depth);
    void generate_rooms(TileMapLayer *layer, const Vector2i &center_cell, int depth);
};

#endif // PROCEDURAL_GENERATOR_2D_H
