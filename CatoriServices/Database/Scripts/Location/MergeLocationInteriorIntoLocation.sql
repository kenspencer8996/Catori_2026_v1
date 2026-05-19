-- One-time SQLite migration.
-- Merges LocationInterior columns into Location and replaces LocationLayoutObject tables
-- with LocationLayoutItem / LocationLayoutPoint.

PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

ALTER TABLE Location ADD COLUMN BusinessId INTEGER NULL;
ALTER TABLE Location ADD COLUMN Description TEXT;
ALTER TABLE Location ADD COLUMN BackgroundImagePath TEXT NOT NULL DEFAULT '';
ALTER TABLE Location ADD COLUMN InteriorType TEXT NOT NULL DEFAULT 'LocationEntity';
ALTER TABLE Location ADD COLUMN WorldMapImagePath TEXT;
ALTER TABLE Location ADD COLUMN HotspotLeft REAL NOT NULL DEFAULT 0;
ALTER TABLE Location ADD COLUMN HotspotTop REAL NOT NULL DEFAULT 0;
ALTER TABLE Location ADD COLUMN HotspotWidth REAL NOT NULL DEFAULT 100;
ALTER TABLE Location ADD COLUMN HotspotHeight REAL NOT NULL DEFAULT 100;
ALTER TABLE Location ADD COLUMN DesignWidth REAL NOT NULL DEFAULT 1920;
ALTER TABLE Location ADD COLUMN DesignHeight REAL NOT NULL DEFAULT 1080;
ALTER TABLE Location ADD COLUMN DefaultRobotX REAL;
ALTER TABLE Location ADD COLUMN DefaultRobotY REAL;
ALTER TABLE Location ADD COLUMN IsActive INTEGER NOT NULL DEFAULT 1;
ALTER TABLE Location ADD COLUMN SortOrder INTEGER NOT NULL DEFAULT 0;
ALTER TABLE Location ADD COLUMN UpdatedDate TEXT;

UPDATE Location
SET
    Description = (
        SELECT Description FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ),
    BackgroundImagePath = COALESCE((
        SELECT BackgroundImagePath FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), BackgroundImagePath),
    InteriorType = COALESCE((
        SELECT InteriorType FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), InteriorType),
    WorldMapImagePath = (
        SELECT WorldMapImagePath FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ),
    HotspotLeft = COALESCE((
        SELECT HotspotLeft FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), HotspotLeft),
    HotspotTop = COALESCE((
        SELECT HotspotTop FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), HotspotTop),
    HotspotWidth = COALESCE((
        SELECT HotspotWidth FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), HotspotWidth),
    HotspotHeight = COALESCE((
        SELECT HotspotHeight FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), HotspotHeight),
    DesignWidth = COALESCE((
        SELECT DesignWidth FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), DesignWidth),
    DesignHeight = COALESCE((
        SELECT DesignHeight FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), DesignHeight),
    DefaultRobotX = (
        SELECT DefaultRobotX FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ),
    DefaultRobotY = (
        SELECT DefaultRobotY FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ),
    IsActive = COALESCE((
        SELECT IsActive FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), IsActive),
    SortOrder = COALESCE((
        SELECT SortOrder FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    ), SortOrder),
    UpdatedDate = (
        SELECT UpdatedDate FROM LocationInterior
        WHERE LocationInterior.LocationId = Location.LocationId
        ORDER BY SortOrder, Name LIMIT 1
    )
WHERE EXISTS (
    SELECT 1 FROM LocationInterior
    WHERE LocationInterior.LocationId = Location.LocationId
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

INSERT OR IGNORE INTO LocationLayoutItem
    (LocationLayoutItemId, LocationId, ItemName, ItemType, X, Y, Z, Width, Height,
     RotationDegrees, ZIndex, IsLocked, ImagePath, MetadataJson)
SELECT
    LayoutObjectId,
    LocationId,
    ObjectName,
    ObjectType,
    COALESCE((SELECT X FROM LocationLayoutObjectPoint p WHERE p.LayoutObjectId = o.LayoutObjectId ORDER BY CASE WHEN PointRole = 'Anchor' THEN 0 ELSE 1 END, PointIndex LIMIT 1), 0),
    COALESCE((SELECT Y FROM LocationLayoutObjectPoint p WHERE p.LayoutObjectId = o.LayoutObjectId ORDER BY CASE WHEN PointRole = 'Anchor' THEN 0 ELSE 1 END, PointIndex LIMIT 1), 0),
    0,
    0,
    0,
    0,
    ZIndex,
    CASE WHEN IsInteractive = 1 THEN 0 ELSE 1 END,
    ImagePath,
    Notes
FROM LocationLayoutObject o;

INSERT OR IGNORE INTO LocationLayoutPoint
    (LocationLayoutPointId, LocationLayoutItemId, LocationId, PointIndex, PointRole,
     X, Y, Z, SegmentKind)
SELECT
    p.LayoutObjectPointId,
    p.LayoutObjectId,
    o.LocationId,
    p.PointIndex,
    p.PointRole,
    p.X,
    p.Y,
    0,
    'Line'
FROM LocationLayoutObjectPoint p
INNER JOIN LocationLayoutObject o ON o.LayoutObjectId = p.LayoutObjectId;

DROP INDEX IF EXISTS idx_location_interior_location_id;
DROP INDEX IF EXISTS idx_location_layout_object_location_id;
DROP TABLE IF EXISTS LocationInterior;
DROP TABLE IF EXISTS LocationLayoutObjectPoint;
DROP TABLE IF EXISTS LocationLayoutObject;

CREATE INDEX IF NOT EXISTS idx_location_layout_item_layout_id
ON LocationLayoutItem(LocationId);

CREATE INDEX IF NOT EXISTS idx_location_layout_point_item_id
ON LocationLayoutPoint(LocationLayoutItemId);

COMMIT;
PRAGMA foreign_keys = ON;
