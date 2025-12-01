#include "procedural_generator_2d.h"

using namespace godot;

void ProceduralGenerator2D::_bind_methods() {
    ClassDB::bind_method(D_METHOD("generate"), &ProceduralGenerator2D::generate);

    ClassDB::bind_method(D_METHOD("set_tilemap_node", "tilemap_layer_node"), &ProceduralGenerator2D::set_tilemap_node);
    ClassDB::bind_method(D_METHOD("get_tilemap_node"), &ProceduralGenerator2D::get_tilemap_node);

    ClassDB::bind_method(D_METHOD("set_center_node", "node"), &ProceduralGenerator2D::set_center_node);
    ClassDB::bind_method(D_METHOD("get_center_node"), &ProceduralGenerator2D::get_center_node);

    ClassDB::bind_method(D_METHOD("set_generation_radius", "radius"), &ProceduralGenerator2D::set_generation_radius);
    ClassDB::bind_method(D_METHOD("get_generation_radius"), &ProceduralGenerator2D::get_generation_radius);

    ClassDB::bind_method(D_METHOD("set_seed", "seed"), &ProceduralGenerator2D::set_seed);
    ClassDB::bind_method(D_METHOD("get_seed"), &ProceduralGenerator2D::get_seed);

    ADD_PROPERTY(PropertyInfo(Variant::OBJECT, "tilemap_layer_node", PROPERTY_HINT_NODE_TYPE, "TileMapLayer"), "set_tilemap_node", "get_tilemap_node");

    ADD_PROPERTY(PropertyInfo(Variant::OBJECT, "center_node", PROPERTY_HINT_NODE_TYPE, "Node2D"), "set_center_node", "get_center_node");

    ADD_PROPERTY(PropertyInfo(Variant::INT, "generation_radius", PROPERTY_HINT_RANGE, "5,200"), "set_generation_radius", "get_generation_radius");

    ADD_PROPERTY(PropertyInfo(Variant::INT, "seed", PROPERTY_HINT_NONE), "set_seed", "get_seed");
}

ProceduralGenerator2D::ProceduralGenerator2D() {
    _tilemap_layer_node = nullptr;
    _center_node = nullptr;
}

ProceduralGenerator2D::~ProceduralGenerator2D() {}

void ProceduralGenerator2D::set_tilemap_node(Node *tilemap_layer_node) {
    _tilemap_layer_node = tilemap_layer_node;
}
Node *ProceduralGenerator2D::get_tilemap_node() const {
    return _tilemap_layer_node;
}
void ProceduralGenerator2D::set_center_node(Node *node) {
    _center_node = node;
}
Node *ProceduralGenerator2D::get_center_node() const {
    return _center_node;
}
void ProceduralGenerator2D::set_generation_radius(int radius) {
    _generation_radius = radius;
}
int ProceduralGenerator2D::get_generation_radius() const {
    return _generation_radius;
}

void ProceduralGenerator2D::set_seed(int64_t seed){
    _seed = seed;
}
int64_t ProceduralGenerator2D::get_seed() const {
    return _seed;
}


void ProceduralGenerator2D::generate()
{
    if (!_tilemap_layer_node) {
        UtilityFunctions::print("ProceduralGenerator2D: No TileMapLayer assigned");
        return;
    }
    if (!_center_node) {
        UtilityFunctions::print("ProceduralGenerator2D: No center node assigned");
        return;
    }

    TileMapLayer *layer = Object::cast_to<TileMapLayer>(_tilemap_layer_node);
    if (!layer)
    {
        UtilityFunctions::print("ProceduralGenerator2D: Assigned node is not a TileMapLayer");
        return;
    }

    // Convert world to map cell
    Vector2 world_pos = Object::cast_to<Node2D>(_center_node)->get_global_position();
    Vector2i center_cell = layer->local_to_map(world_pos);

    // Clear existing used area
    Rect2i used = layer->get_used_rect();
    for (int x = used.position.x; x < used.position.x + used.size.x; x++)
    {
        for (int y = used.position.y; y < used.position.y + used.size.y; y++)
        {
            layer->set_cell(Vector2i(x, y), -1);
        }
    }

    generate_rooms(layer, center_cell, 10);

    UtilityFunctions::print("ProceduralGenerator2D: Generated with seed ", _seed);
}


uint32_t ProceduralGenerator2D::position_rand(Vector2i pos, int index) const
{
    uint64_t v = (uint64_t(pos.x) * 73856093ULL)
               ^ (uint64_t(pos.y) * 19349663ULL)
               ^ (uint64_t(index) * 83492791ULL)
               ^ uint64_t(_seed);

    v ^= v >> 33;
    v *= 0xff51afd7ed558ccdULL;
    v ^= v >> 33;
    v *= 0xc4ceb9fe1a85ec53ULL;
    v ^= v >> 33;

    return uint32_t(v);
}


void ProceduralGenerator2D::generate_rooms(TileMapLayer *layer, const Vector2i &center_cell, int depth)
{
    if (depth <= 0)
    {
        return;
    }

    int terrain_set = 0;
    int terrain_floor = 0;

    TypedArray<Vector2i> room_cells;

    int size = 3;
    Vector2i start = center_cell - Vector2i(size / 2, size / 2);

    for (int x = 0; x < size; x++)
    {
        for (int y = 0; y < size; y++)
        {
            room_cells.push_back(start + Vector2i(x, y));
        }
    }

    layer->set_cells_terrain_connect(room_cells, terrain_set, terrain_floor);

    generate_paths(layer, center_cell, depth - 1);
}


void ProceduralGenerator2D::generate_paths(TileMapLayer *layer, const Vector2i &center_cell, int depth)
{
    if (depth <= 0)
        return;

    int path_len = 20;

    Vector2i pos = center_cell;
    TypedArray<Vector2i> path_cells;

    for (int i = 0; i < path_len; i++)
    {

        path_cells.push_back(pos);

        uint32_t r = position_rand(pos, i) % 4;

        switch (r)
        {
            case 0: pos.x += 1; break;
            case 1: pos.x -= 1; break;
            case 2: pos.y += 1; break;
            case 3: pos.y -= 1; break;
        }

        if ((pos - center_cell).length() > _generation_radius) {
            pos = center_cell;
        }
    }

    layer->set_cells_terrain_path(path_cells, 0, 0);

    generate_rooms(layer, path_cells[path_len - 1], depth - 1);
}
