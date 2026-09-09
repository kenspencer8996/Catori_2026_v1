PRAGMA foreign_keys = OFF;

CREATE TABLE Robot (
    RobotId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    RobotX REAL NOT NULL DEFAULT 300,
    RobotY REAL NOT NULL DEFAULT 200,
    RobotWidth REAL NOT NULL DEFAULT 100,
    RobotHeight REAL NOT NULL DEFAULT 100,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    UpdatedAt TEXT NOT NULL DEFAULT (datetime('now')),
    LocationId INTEGER NOT NULL DEFAULT 0
);

INSERT INTO Robot (RobotId, RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt, LocationId)
SELECT RobotDesignerId, RobotX, RobotY, RobotWidth, RobotHeight, CreatedAt, UpdatedAt, 0
FROM RobotDesigner;

-- Preserve poses whose legacy RobotDesigner parent is missing.
INSERT OR IGNORE INTO Robot (RobotId, LocationId)
SELECT DISTINCT RobotDesignerId, 0
FROM RobotPose;

CREATE TABLE RobotPose_New (
    RobotPoseId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    RobotId INTEGER,
    PoseName TEXT,
    Pose TEXT,
    FOREIGN KEY (RobotId) REFERENCES Robot(RobotId) ON DELETE CASCADE
);

INSERT INTO RobotPose_New (RobotPoseId, RobotId, PoseName, Pose)
SELECT rp.RobotPoseId,
       rp.RobotDesignerId,
       rp.PoseName,
       COALESCE((
           SELECT json_group_array(ordered.Angle)
           FROM (
               SELECT rps.Angle
               FROM RobotPoseSegment rps
               WHERE rps.RobotPoseId = rp.RobotPoseId
               ORDER BY rps.SegmentIndex
           ) ordered
       ), '[]')
FROM RobotPose rp;

DROP TABLE RobotPose;
ALTER TABLE RobotPose_New RENAME TO RobotPose;
DROP TABLE RobotPoseSegment;
DROP TABLE RobotSequence;
DROP TABLE RobotDesigner;

CREATE INDEX idx_robot_location_id ON Robot(LocationId);
CREATE INDEX idx_robot_pose_robot_id ON RobotPose(RobotId);

PRAGMA foreign_keys = ON;
