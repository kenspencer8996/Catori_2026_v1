CREATE TABLE IF NOT EXISTS MachineLayoutDesigner (
    MachineLayoutDesignerId INTEGER PRIMARY KEY AUTOINCREMENT,
    LocationId INTEGER NOT NULL,
    SequenceName TEXT NOT NULL,
    SelectionX REAL NOT NULL DEFAULT 0,
    SelectionY REAL NOT NULL DEFAULT 0,
    SelectionWidth REAL NOT NULL DEFAULT 0,
    SelectionHeight REAL NOT NULL DEFAULT 0,
    RobotX REAL NOT NULL DEFAULT 300,
    RobotY REAL NOT NULL DEFAULT 200,
    RobotWidth REAL NOT NULL DEFAULT 100,
    RobotHeight REAL NOT NULL DEFAULT 100,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_machine_layout_designer_sequence_name
ON MachineLayoutDesigner(SequenceName);

CREATE INDEX IF NOT EXISTS idx_machine_layout_designer_location_id
ON MachineLayoutDesigner(LocationId);

CREATE TABLE IF NOT EXISTS RobotPose (
    RobotPoseId INTEGER PRIMARY KEY AUTOINCREMENT,
    LocationId INTEGER NOT NULL,
    PoseIndex INTEGER NOT NULL,
    PoseName TEXT NOT NULL,
    Joint1 REAL NOT NULL DEFAULT 0,
    Joint2 REAL NOT NULL DEFAULT 0,
    Joint3 REAL NOT NULL DEFAULT 0,
    JointHand REAL NOT NULL DEFAULT 0,
    DurationMilliseconds INTEGER NOT NULL DEFAULT 600,
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_robot_pose_location_sequence
ON RobotPose(LocationId, PoseIndex);
