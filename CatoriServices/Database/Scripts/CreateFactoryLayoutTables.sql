-- SQLite Factory Layout Tables Creation Script
-- Factories, layouts, placeable items, geometry points, and saved part routes.

CREATE TABLE IF NOT EXISTS Factories (
    factory_id INTEGER PRIMARY KEY AUTOINCREMENT,
    business_id INTEGER NULL,
    factory_name TEXT NOT NULL,
    background_image_path TEXT NULL,
    created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE INDEX IF NOT EXISTS idx_factories_business_id ON Factories(business_id);

CREATE TABLE IF NOT EXISTS FactoryLayouts (
    factory_layout_id INTEGER PRIMARY KEY AUTOINCREMENT,
    factory_id INTEGER NOT NULL,
    layout_name TEXT NOT NULL,
    canvas_width REAL NOT NULL DEFAULT 1920,
    canvas_height REAL NOT NULL DEFAULT 1080,
    is_active INTEGER NOT NULL DEFAULT 1,
    notes TEXT NULL,
    created_at TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (factory_id) REFERENCES Factories(factory_id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_factory_layouts_factory_id ON FactoryLayouts(factory_id);
CREATE INDEX IF NOT EXISTS idx_factory_layouts_active ON FactoryLayouts(factory_id, is_active);

CREATE TABLE IF NOT EXISTS FactoryLayoutItems (
    factory_layout_item_id INTEGER PRIMARY KEY AUTOINCREMENT,
    factory_layout_id INTEGER NOT NULL,
    item_name TEXT NOT NULL,
    item_type TEXT NOT NULL CHECK(item_type IN ('Conveyor', 'Table', 'Robot', 'Workstation', 'Storage', 'Decoration')),
    x REAL NOT NULL DEFAULT 0,
    y REAL NOT NULL DEFAULT 0,
    z REAL NOT NULL DEFAULT 0,
    width REAL NOT NULL DEFAULT 0,
    height REAL NOT NULL DEFAULT 0,
    rotation_degrees REAL NOT NULL DEFAULT 0,
    z_index INTEGER NOT NULL DEFAULT 0,
    is_locked INTEGER NOT NULL DEFAULT 0,
    image_path TEXT NULL,
    metadata_json TEXT NULL,
    FOREIGN KEY (factory_layout_id) REFERENCES FactoryLayouts(factory_layout_id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_factory_layout_items_layout_id ON FactoryLayoutItems(factory_layout_id);
CREATE INDEX IF NOT EXISTS idx_factory_layout_items_type ON FactoryLayoutItems(factory_layout_id, item_type);

CREATE TABLE IF NOT EXISTS FactoryLayoutPoints (
    factory_layout_point_id INTEGER PRIMARY KEY AUTOINCREMENT,
    factory_layout_item_id INTEGER NOT NULL,
    point_index INTEGER NOT NULL,
    point_role TEXT NULL,
    x REAL NOT NULL,
    y REAL NOT NULL,
    z REAL NOT NULL DEFAULT 0,
    segment_kind TEXT NOT NULL DEFAULT 'Line' CHECK(segment_kind IN ('Line', 'QuadraticBezier', 'CubicBezier')),
    control1_x REAL NULL,
    control1_y REAL NULL,
    control2_x REAL NULL,
    control2_y REAL NULL,
    rotation_degrees REAL NULL,
    FOREIGN KEY (factory_layout_item_id) REFERENCES FactoryLayoutItems(factory_layout_item_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_factory_layout_points_item_index ON FactoryLayoutPoints(factory_layout_item_id, point_index);

CREATE TABLE IF NOT EXISTS FactoryPartRoutes (
    factory_part_route_id INTEGER PRIMARY KEY AUTOINCREMENT,
    factory_layout_id INTEGER NOT NULL,
    route_name TEXT NOT NULL,
    product_id INTEGER NULL,
    from_item_id INTEGER NULL,
    to_item_id INTEGER NULL,
    is_active INTEGER NOT NULL DEFAULT 1,
    created_at TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (factory_layout_id) REFERENCES FactoryLayouts(factory_layout_id) ON DELETE CASCADE,
    FOREIGN KEY (from_item_id) REFERENCES FactoryLayoutItems(factory_layout_item_id) ON DELETE SET NULL,
    FOREIGN KEY (to_item_id) REFERENCES FactoryLayoutItems(factory_layout_item_id) ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS idx_factory_part_routes_layout_id ON FactoryPartRoutes(factory_layout_id);

CREATE TABLE IF NOT EXISTS FactoryPartRoutePoints (
    factory_part_route_point_id INTEGER PRIMARY KEY AUTOINCREMENT,
    factory_part_route_id INTEGER NOT NULL,
    point_index INTEGER NOT NULL,
    x REAL NOT NULL,
    y REAL NOT NULL,
    z REAL NOT NULL DEFAULT 0,
    seconds_from_start REAL NOT NULL DEFAULT 0,
    FOREIGN KEY (factory_part_route_id) REFERENCES FactoryPartRoutes(factory_part_route_id) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_factory_part_route_points_route_index ON FactoryPartRoutePoints(factory_part_route_id, point_index);
