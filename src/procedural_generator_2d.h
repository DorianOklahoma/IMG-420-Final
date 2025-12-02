#ifndef PROCEDURAL_GENERATOR_2D_H
#define PROCEDURAL_GENERATOR_2D_H

#include <godot_cpp/classes/node2d.hpp>
#include <godot_cpp/classes/tile_map_layer.hpp>
#include <godot_cpp/classes/node.hpp>
#include <godot_cpp/variant/utility_functions.hpp>
#include <godot_cpp/core/class_db.hpp>

#include <queue>
#include <set>
#include <vector>

using namespace godot;

struct ChunkCoord
{
    int x;
    int y;
    bool operator<(const ChunkCoord &o) const
    {
        return (x == o.x) ? (y < o.y) : (x < o.x);
    }
};

class ProceduralGenerator2D : public Node2D
{
    GDCLASS(ProceduralGenerator2D, Node2D);

protected:
    static void _bind_methods();

public:
    ProceduralGenerator2D();
    ~ProceduralGenerator2D();

    void generate();
    void generate_chunk(const ChunkCoord &chunk);

    void set_tilemap_node(Node *tilemap_layer_node);
    Node *get_tilemap_node() const;

    void set_center_node(Node *node);
    Node *get_center_node() const;

    void set_generation_radius(int radius);
    int get_generation_radius() const;

    void set_seed(int64_t seed);
    int64_t get_seed() const;

private:
    Node *_tilemap_layer_node = nullptr;
    Node *_center_node = nullptr;

    int _generation_radius = 5;
    int64_t _seed = 123456789;

    const int chunk_size = 16;

    std::queue<ChunkCoord> _chunk_queue;
    std::set<ChunkCoord> _generated_chunks;

    uint32_t position_rand(Vector2i pos, int index = 0) const;

    void schedule_chunks(TileMapLayer *layer, Vector2i center_cell);
    
    std::vector<Vector2i> generate_room_shape(const Vector2i &center_cell);
    void stamp_cells(TileMapLayer *layer, const std::vector<Vector2i> &cells);
    void carve_corridor(TileMapLayer *layer, Vector2i from, Vector2i to);
};

#endif
