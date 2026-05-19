-- SQLite Location Layout Tables Creation Script
-- Location now owns the former LocationInterior metadata.

CREATE TABLE IF NOT EXISTS Location (
    LocationId INTEGER PRIMARY KEY AUTOINCREMENT,
    BusinessId INTEGER NULL,
    LocationName TEXT NOT NULL,
    Description TEXT,
    BackgroundImagePath TEXT NOT NULL DEFAULT '',
    InteriorType TEXT NOT NULL DEFAULT 'LocationEntity',
    WorldMapImagePath TEXT,
    HotspotLeft REAL NOT NULL DEFAULT 0,
    HotspotTop REAL NOT NULL DEFAULT 0,
    HotspotWidth REAL NOT NULL DEFAULT 100,
    HotspotHeight REAL NOT NULL DEFAULT 100,
    DesignWidth REAL NOT NULL DEFAULT 1920,
    DesignHeight REAL NOT NULL DEFAULT 1080,
    DefaultRobotX REAL,
    DefaultRobotY REAL,
    IsActive INTEGER NOT NULL DEFAULT 1,
    SortOrder INTEGER NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedDate TEXT
);

CREATE TABLE IF NOT EXISTS LocationLayoutItem (
    LocationLayoutItemId INTEGER PRIMARY KEY AUTOINCREMENT,
    LocationId INTEGER NOT NULL,
    ItemName TEXT NOT NULL,
    ItemType TEXT NOT NULL,
    X REAL NOT NULL DEFAULT 0,
    Y REAL NOT NULL DEFAULT 0,
    Z REAL NOT NULL DEFAULT 0,
    Width REAL NOT NULL DEFAULT 0,
    Height REAL NOT NULL DEFAULT 0,
    RotationDegrees REAL NOT NULL DEFAULT 0,
    ZIndex INTEGER NOT NULL DEFAULT 0,
    IsLocked INTEGER NOT NULL DEFAULT 0,
    ImagePath TEXT NULL,
    MetadataJson TEXT NULL,
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_location_layout_item_layout_id
ON LocationLayoutItem(LocationId);

CREATE TABLE IF NOT EXISTS LocationLayoutPoint (
    LocationLayoutPointId INTEGER PRIMARY KEY AUTOINCREMENT,
    LocationLayoutItemId INTEGER NOT NULL,
    LocationId INTEGER NOT NULL,
    PointIndex INTEGER NOT NULL,
    PointRole TEXT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    Z REAL NOT NULL DEFAULT 0,
    SegmentKind TEXT NOT NULL DEFAULT 'Line',
    Control1X REAL,
    Control1Y REAL,
    Control2X REAL,
    Control2Y REAL,
    RotationDegrees REAL,
    FOREIGN KEY (LocationLayoutItemId) REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE CASCADE,
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_location_layout_point_item_id
ON LocationLayoutPoint(LocationLayoutItemId);
