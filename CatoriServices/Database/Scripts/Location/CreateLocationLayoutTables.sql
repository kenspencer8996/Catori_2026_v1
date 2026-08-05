-- SQLite Location Layout Tables Creation Script
-- Aligned to the 6/3/2026 Location, LocationLayoutItem, and LocationLayoutPoint schema.

CREATE TABLE IF NOT EXISTS Location (
    LocationId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    LocationName TEXT NOT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    BusinessId INTEGER NULL,
    Description TEXT NULL,
    BackgroundImagePath TEXT NOT NULL DEFAULT ''
);

CREATE TABLE IF NOT EXISTS LocationLayoutItem (
    LocationLayoutItemId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    LocationId INTEGER NOT NULL,
    ItemName TEXT NOT NULL,
    ItemType TEXT NOT NULL,
    MajorItemType TEXT NULL,
    X REAL NOT NULL DEFAULT 0,
    Y REAL NOT NULL DEFAULT 0,
    Z REAL NOT NULL DEFAULT 0,
    Width REAL NOT NULL DEFAULT 0,
    Height REAL NOT NULL DEFAULT 0,
    RotationDegrees REAL NOT NULL DEFAULT 0,
    ZIndex INTEGER NOT NULL DEFAULT 0,
    IsLocked INTEGER NOT NULL DEFAULT 0,
    ItemDataJson TEXT NULL,
    MetadataJson TEXT NULL,
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_location_layout_item_location_id
ON LocationLayoutItem(LocationId);

CREATE TABLE IF NOT EXISTS LocationLayoutPoint (
    LocationLayoutPointId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    LocationLayoutItemId INTEGER NOT NULL,
    LocationId INTEGER NOT NULL,
    PointIndex INTEGER NOT NULL,
    PointRole TEXT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    RotationDegrees REAL NULL,
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE,
    FOREIGN KEY (LocationLayoutItemId) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_location_layout_point_item_id
ON LocationLayoutPoint(LocationLayoutItemId);
