-- SQLite Factory Layout Tables Creation Script
-- Matches the live CatoriDatabase2026 schema.

CREATE TABLE IF NOT EXISTS Factory (
    FactoryId INTEGER PRIMARY KEY AUTOINCREMENT,
    FactoryName TEXT NOT NULL,
    BackgroundImagePath TEXT NULL,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS FactoryLayoutObject (
    LayoutObjectId INTEGER PRIMARY KEY AUTOINCREMENT,
    FactoryId INTEGER NOT NULL,
    ObjectName TEXT NOT NULL,
    ObjectType TEXT NOT NULL,
    ImagePath TEXT NULL,
    ZIndex INTEGER NOT NULL DEFAULT 0,
    IsInteractive INTEGER NOT NULL DEFAULT 1,
    IsVisible INTEGER NOT NULL DEFAULT 1,
    Notes TEXT NULL,
    FOREIGN KEY (FactoryId) REFERENCES Factory(FactoryId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_factory_layout_object_factory_id
ON FactoryLayoutObject(FactoryId);

CREATE TABLE IF NOT EXISTS FactoryLayoutObjectPoint (
    LayoutObjectPointId INTEGER PRIMARY KEY AUTOINCREMENT,
    LayoutObjectId INTEGER NOT NULL,
    PointIndex INTEGER NOT NULL,
    X REAL NOT NULL,
    Y REAL NOT NULL,
    PointRole TEXT NULL,
    FOREIGN KEY (LayoutObjectId) REFERENCES FactoryLayoutObject(LayoutObjectId) ON DELETE CASCADE
);
