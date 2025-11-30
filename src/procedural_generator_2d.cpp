#include "procedural_generator_2d.h"

using namespace godot;

void ProceduralGenerator2D::_bind_methods() {
    ClassDB::bind_method(D_METHOD("generate"), &ProceduralGenerator2D::generate);
    ClassDB::bind_method(D_METHOD("set_tilemap_node", "tilemap_layer_node"), &ProceduralGenerator2D::set_tilemap_node);
    ClassDB::bind_method(D_METHOD("get_tilemap_node"), &ProceduralGenerator2D::get_tilemap_node);

    ADD_PROPERTY(
        PropertyInfo(Variant::OBJECT, "tilemap_layer_node", PROPERTY_HINT_NODE_TYPE, "TileMapLayer"),
        "set_tilemap_node",
        "get_tilemap_node"
    );
}

ProceduralGenerator2D::ProceduralGenerator2D() {
    _tilemap_layer_node = nullptr;
}

ProceduralGenerator2D::~ProceduralGenerator2D() {
}

void ProceduralGenerator2D::set_tilemap_node(Node *tilemap_layer_node) {
    _tilemap_layer_node = tilemap_layer_node;
}

Node *ProceduralGenerator2D::get_tilemap_node() const {
    return _tilemap_layer_node;
}

void ProceduralGenerator2D::generate() {
    if (!_tilemap_layer_node) {
        UtilityFunctions::print("ProceduralGenerator2D: no TileMapLayer assigned");
        return;
    }

    TileMapLayer *layer = Object::cast_to<TileMapLayer>(_tilemap_layer_node);
    if (!layer) {
        UtilityFunctions::print("ProceduralGenerator2D: assigned node is not a TileMapLayer");
        return;
    }

    // Clear existing used area
    Rect2i used = layer->get_used_rect();
    for (int x = used.position.x; x < used.position.x + used.size.x; x++) {
        for (int y = used.position.y; y < used.position.y + used.size.y; y++) {
            layer->set_cell(Vector2i(x, y), -1);
        }
    }

    // Place ONE tile
    int tile_id = 0;
    layer->set_cell(Vector2i(0, 0), tile_id);

    UtilityFunctions::print("ProceduralGenerator2D: placed single tile");
}
