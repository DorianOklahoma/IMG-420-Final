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
    ADD_PROPERTY(PropertyInfo(Variant::INT, "generation_radius", PROPERTY_HINT_RANGE, "1,200"), "set_generation_radius", "get_generation_radius");
    ADD_PROPERTY(PropertyInfo(Variant::INT, "seed"), "set_seed", "get_seed");
}

ProceduralGenerator2D::ProceduralGenerator2D() {}
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

void ProceduralGenerator2D::set_seed(int64_t seed) {
    _seed = seed;
}
int64_t ProceduralGenerator2D::get_seed() const {
    return _seed;
}

uint32_t ProceduralGenerator2D::position_rand(Vector2i pos, int index) const
{
    uint64_t v = uint64_t(pos.x) * 73856093ULL ^ uint64_t(pos.y) * 19349663ULL ^ uint64_t(index) * 83492791ULL ^ uint64_t(_seed);

    v ^= v >> 33;
    v *= 0xff51afd7ed558ccdULL;
    v ^= v >> 33;
    v *= 0xc4ceb9fe1a85ec53ULL;
    v ^= v >> 33;

    return uint32_t(v);
}

void ProceduralGenerator2D::generate()
{
    if (!_tilemap_layer_node || !_center_node)
    {
        UtilityFunctions::print("Missing tilemap or center node");
        return;
    }

    TileMapLayer *layer = Object::cast_to<TileMapLayer>(_tilemap_layer_node);
    Node2D *center = Object::cast_to<Node2D>(_center_node);
    Vector2 world = center->get_global_position();
    Vector2 local = layer->to_local(world);
    Vector2i cell = layer->local_to_map(local);

    schedule_chunks(layer, cell);

    if (!_chunk_queue.empty())
    {
        ChunkCoord c = _chunk_queue.front();
        _chunk_queue.pop();
        generate_chunk(c);
    }
}

void ProceduralGenerator2D::schedule_chunks(TileMapLayer *layer, Vector2i center_cell)
{
    auto floor_div = [](int a, int b)
    {
        return (a >= 0) ? a / b : (a - b + 1) / b;
    };

    int cx = floor_div(center_cell.x, chunk_size);
    int cy = floor_div(center_cell.y, chunk_size);

    for (int x = cx - _generation_radius; x <= cx + _generation_radius; x++)
    {
        for (int y = cy - _generation_radius; y <= cy + _generation_radius; y++)
        {
            ChunkCoord c{x, y};
            if (_generated_chunks.find(c) == _generated_chunks.end())
            {
                _generated_chunks.insert(c);
                _chunk_queue.push(c);
            }
        }
    }
}

void ProceduralGenerator2D::generate_chunk(const ChunkCoord &coord)
{
    TileMapLayer *layer = Object::cast_to<TileMapLayer>(_tilemap_layer_node);

    Vector2i origin(coord.x * chunk_size, coord.y * chunk_size);

    int room_count = (position_rand(origin, 0) % 3) + 2;

    std::vector<Vector2i> room_centers;
    room_centers.reserve(room_count);

    for (int i = 0; i < room_count; i++)
    {
        Vector2i rp(origin.x + (position_rand(origin, i + 1) % chunk_size), origin.y + (position_rand(origin, i + 2) % chunk_size));
        room_centers.push_back(rp);
        auto shape = generate_room_shape(rp);
        stamp_cells(layer, shape);
    }

    for (int i = 0; i + 1 < (int)room_centers.size(); i++)
    {
        carve_corridor(layer, room_centers[i], room_centers[i + 1]);
    }
}

std::vector<Vector2i> ProceduralGenerator2D::generate_room_shape(const Vector2i &center)
{
    std::vector<Vector2i> cells;

    // Room size (width x height)
    int w = 4 + (position_rand(center, 0) % 5);
    int h = 4 + (position_rand(center, 1) % 5);

    for (int dx = -w/2; dx <= w/2; dx++)
    {
        for (int dy = -h/2; dy <= h/2; dy++)
        {
            cells.push_back(center + Vector2i(dx, dy));
        }
    }

    return cells;
}

void ProceduralGenerator2D::stamp_cells(TileMapLayer *layer, const std::vector<Vector2i> &cells)
{
    TypedArray<Vector2i> arr;
    arr.resize(cells.size());
    for (int i = 0; i < (int)cells.size(); i++)
    {
        arr[i] = cells[i];
    }
    layer->set_cells_terrain_connect(arr, 0, 0);
}

void ProceduralGenerator2D::carve_corridor(TileMapLayer *layer, Vector2i from, Vector2i to)
{
    TypedArray<Vector2i> path;
    Vector2i c = from;

    while (c != to) {
        path.push_back(c);

        if (abs(to.x - c.x) > abs(to.y - c.y))
        {
            c.x += (to.x > c.x) ? 1 : -1;
        }
        else
        {
            c.y += (to.y > c.y) ? 1 : -1;
        }
    }

    path.push_back(to);
    int corridor_radius = 1;
    TypedArray<Vector2i> thick_path;

    for (int i = 0; i < path.size(); ++i)
    {
        Vector2i p = path[i];
        for (int dx = -corridor_radius; dx <= corridor_radius; ++dx)
        {
            for (int dy = -corridor_radius; dy <= corridor_radius; ++dy)
            {
                thick_path.push_back(Vector2i(p.x + dx, p.y + dy));
            }
        }
    }

    layer->set_cells_terrain_connect(thick_path, 0, 0);
}
