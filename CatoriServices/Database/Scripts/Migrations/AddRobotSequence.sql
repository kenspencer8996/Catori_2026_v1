CREATE TABLE IF NOT EXISTS RobotSequence (
    RobotSequenceId INTEGER PRIMARY KEY AUTOINCREMENT,
    LocationId INTEGER NOT NULL,
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

-- SQLite cannot add a foreign key constraint to an existing table without rebuilding it.
-- The app repository adds RobotPose.RobotSequenceId when needed and backfills the value.
ALTER TABLE RobotPose ADD COLUMN RobotSequenceId INTEGER NOT NULL DEFAULT 0;

INSERT OR IGNORE INTO RobotSequence
    (LocationId, SequenceName, RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt)
SELECT LocationId, SequenceName, RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt
FROM MachineLayoutDesigner
WHERE SequenceName IS NOT NULL AND TRIM(SequenceName) <> '';

UPDATE RobotPose
SET RobotSequenceId = (
    SELECT rs.RobotSequenceId
    FROM RobotSequence rs
    WHERE rs.LocationId = RobotPose.LocationId
    ORDER BY rs.UpdatedAt DESC
    LIMIT 1
)
WHERE RobotSequenceId = 0;

CREATE INDEX IF NOT EXISTS idx_robot_pose_sequence
ON RobotPose(RobotSequenceId, PoseIndex);
