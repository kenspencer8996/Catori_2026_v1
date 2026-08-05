CREATE TABLE IF NOT EXISTS MachineLayoutDesigner (
    MachineLayoutDesignerId INTEGER PRIMARY KEY AUTOINCREMENT,
    LocationId INTEGER NOT NULL,
    SelectionX REAL NOT NULL DEFAULT 0,
    SelectionY REAL NOT NULL DEFAULT 0,
    SelectionWidth REAL NOT NULL DEFAULT 0,
    SelectionHeight REAL NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_machine_layout_designer_location_id
ON MachineLayoutDesigner(LocationId);

CREATE TABLE IF NOT EXISTS MachineDefinition (
    MachineDefinitionId INTEGER PRIMARY KEY AUTOINCREMENT,
    MachineType TEXT NOT NULL,
    MachineName TEXT NOT NULL,
    Description TEXT NOT NULL DEFAULT '',
    DefaultWidth REAL NOT NULL DEFAULT 100,
    DefaultHeight REAL NOT NULL DEFAULT 100,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_machine_definition_type_name
ON MachineDefinition(MachineType, MachineName);

CREATE TABLE IF NOT EXISTS MachineInstance (
    MachineInstanceId INTEGER PRIMARY KEY AUTOINCREMENT,
    MachineDefinitionId INTEGER NOT NULL,
    InstanceName TEXT NOT NULL,
    DisplayName TEXT NOT NULL DEFAULT '',
    DefaultScale REAL NOT NULL DEFAULT 1,
    DefaultWidth REAL NOT NULL DEFAULT 100,
    DefaultHeight REAL NOT NULL DEFAULT 100,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (MachineDefinitionId) REFERENCES MachineDefinition(MachineDefinitionId)
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_machine_instance_definition_name
ON MachineInstance(MachineDefinitionId, InstanceName);

CREATE TABLE IF NOT EXISTS MachineInstanceSegment (
    MachineInstanceSegmentId INTEGER PRIMARY KEY AUTOINCREMENT,
    MachineInstanceId INTEGER NOT NULL,
    SegmentIndex INTEGER NOT NULL,
    SegmentName TEXT NOT NULL DEFAULT '',
    Length REAL NOT NULL DEFAULT 124,
    Width REAL NOT NULL DEFAULT 40,
    InitialAngle REAL NOT NULL DEFAULT 0,
    MinAngle REAL NOT NULL DEFAULT -180,
    MaxAngle REAL NOT NULL DEFAULT 180,
    Overlap REAL NOT NULL DEFAULT 16,
    Color TEXT NOT NULL DEFAULT '',
    ImageName TEXT NOT NULL DEFAULT '',
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (MachineInstanceId) REFERENCES MachineInstance(MachineInstanceId) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_machine_instance_segment_index
ON MachineInstanceSegment(MachineInstanceId, SegmentIndex);
