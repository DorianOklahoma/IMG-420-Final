#include "procedural_generator_2d.h"

using namespace godot;

void ProceduralGenerator2D::_bind_methods() {
    ClassDB::bind_method(D_METHOD("generate"), &ProceduralGenerator2D::generate);
    ClassDB::bind_method(D_METHOD("set_tilemap_node", "tilemap_node"), &ProceduralGenerator2D::set_tilemap_node);
    ClassDB::bind_method(D_METHOD("get_tilemap_node"), &ProceduralGenerator2D::get_tilemap_node);

    ADD_PROPERTY(PropertyInfo(Variant::OBJECT, "tilemap_node", PROPERTY_HINT_NODE_TYPE, "TileMap"), "set_tilemap_node", "get_tilemap_node");
}

ProceduralGenerator2D::ProceduralGenerator2D() {
    _tilemap_node = nullptr;
}

ProceduralGenerator2D::~ProceduralGenerator2D() {
}

void ProceduralGenerator2D::set_tilemap_node(Node *tilemap_node) {
    _tilemap_node = tilemap_node;
}

Node* ProceduralGenerator2D::get_tilemap_node() const {
    return _tilemap_node;
}

void ProceduralGenerator2D::generate() {
    if (!_tilemap_node) {
        UtilityFunctions::print("ProceduralGenerator2D: no TileMap assigned");
        return;
    }

    TileMap *tm = Object::cast_to<TileMap>(_tilemap_node);
    if (!tm) {
        UtilityFunctions::print("ProceduralGenerator2D: assigned node is not a TileMap");
        return;
    }

    // Clear existing cells in used rect
    Rect2i used = tm->get_used_rect();
    for (int x = used.position.x; x < used.position.x + used.size.x; x++) {
        for (int y = used.position.y; y < used.position.y + used.size.y; y++) {
            tm->set_cell(0, Vector2i(x, y), -1); // layer 0, clear tile
        }
    }

    // Simple procedural checkerboard pattern
    for (int x = -10; x <= 10; x++) {
        for (int y = -10; y <= 10; y++) {
            int tile = ((x + y) % 2 == 0) ? 0 : 1;
            tm->set_cell(0, Vector2i(x, y), tile); // layer 0
        }
    }

    UtilityFunctions::print("ProceduralGenerator2D: generation complete");
}
