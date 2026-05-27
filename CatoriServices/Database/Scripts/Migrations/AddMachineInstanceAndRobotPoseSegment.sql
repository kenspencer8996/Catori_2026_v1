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

-- Run this ALTER only if RobotSequence.MachineInstanceId does not already exist.
ALTER TABLE RobotSequence ADD COLUMN MachineInstanceId INTEGER NOT NULL DEFAULT 0;

-- Run this ALTER only if RobotPose.JointEnd does not already exist.
ALTER TABLE RobotPose ADD COLUMN JointEnd REAL NOT NULL DEFAULT 0;

-- Keep older databases that still have JointHand usable during the transition.
UPDATE RobotPose
SET JointEnd = JointHand
WHERE JointEnd = 0 AND JointHand <> 0;

CREATE INDEX IF NOT EXISTS idx_robot_sequence_machine_instance
ON RobotSequence(MachineInstanceId);

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

INSERT OR IGNORE INTO RobotPoseSegment (RobotPoseId, SegmentIndex, Angle)
SELECT RobotPoseId, 0, Joint1 FROM RobotPose;

INSERT OR IGNORE INTO RobotPoseSegment (RobotPoseId, SegmentIndex, Angle)
SELECT RobotPoseId, 1, Joint2 FROM RobotPose;

INSERT OR IGNORE INTO RobotPoseSegment (RobotPoseId, SegmentIndex, Angle)
SELECT RobotPoseId, 2, Joint3 FROM RobotPose;

INSERT OR IGNORE INTO RobotPoseSegment (RobotPoseId, SegmentIndex, Angle)
SELECT RobotPoseId, 3, JointEnd FROM RobotPose;
