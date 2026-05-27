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

CREATE TABLE IF NOT EXISTS RobotSequence (
    RobotSequenceId INTEGER PRIMARY KEY AUTOINCREMENT,
    LocationId INTEGER NOT NULL,
    MachineInstanceId INTEGER NOT NULL DEFAULT 0,
    SequenceName TEXT NOT NULL,
    RobotX REAL NOT NULL DEFAULT 300,
    RobotY REAL NOT NULL DEFAULT 200,
    RobotWidth REAL NOT NULL DEFAULT 100,
    RobotHeight REAL NOT NULL DEFAULT 100,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (LocationId) REFERENCES Location(LocationId) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_robot_sequence_location_name
ON RobotSequence(LocationId, SequenceName);

CREATE INDEX IF NOT EXISTS idx_robot_sequence_location_id
ON RobotSequence(LocationId);

CREATE INDEX IF NOT EXISTS idx_robot_sequence_machine_instance
ON RobotSequence(MachineInstanceId);

CREATE TABLE IF NOT EXISTS RobotPose (
    RobotPoseId INTEGER PRIMARY KEY AUTOINCREMENT,
    RobotSequenceId INTEGER NOT NULL,
    LocationId INTEGER NOT NULL DEFAULT 0,
    PoseIndex INTEGER NOT NULL,
    PoseName TEXT NOT NULL,
    Joint1 REAL NOT NULL DEFAULT 0,
    Joint2 REAL NOT NULL DEFAULT 0,
    Joint3 REAL NOT NULL DEFAULT 0,
    JointEnd REAL NOT NULL DEFAULT 0,
    DurationMilliseconds INTEGER NOT NULL DEFAULT 600,
    FOREIGN KEY (RobotSequenceId) REFERENCES RobotSequence(RobotSequenceId) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_robot_pose_sequence
ON RobotPose(RobotSequenceId, PoseIndex);

CREATE TABLE IF NOT EXISTS RobotPoseSegment (
    RobotPoseSegmentId INTEGER PRIMARY KEY AUTOINCREMENT,
    RobotPoseId INTEGER NOT NULL,
    SegmentIndex INTEGER NOT NULL,
    Angle REAL NOT NULL DEFAULT 0,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    FOREIGN KEY (RobotPoseId) REFERENCES RobotPose(RobotPoseId) ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS idx_robot_pose_segment_pose_index
ON RobotPoseSegment(RobotPoseId, SegmentIndex);
