PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

CREATE TEMP TABLE RobotLocationLayoutItemMap (
    RobotId INTEGER PRIMARY KEY,
    LocationLayoutItemId INTEGER NOT NULL
);

INSERT INTO LocationLayoutItem
    (LocationId, ItemName, ItemType, MajorItemType, X, Y, Width, Height, MetadataJson)
SELECT r.LocationId,
       'Robot ' || r.RobotId,
       'Robot',
       'Robot',
       r.RobotX,
       r.RobotY,
       r.RobotWidth,
       r.RobotHeight,
       json_object('legacyRobotId', r.RobotId)
FROM Robot r
WHERE NOT EXISTS (
    SELECT 1
    FROM LocationLayoutItem lli
    WHERE lli.LocationId = r.LocationId
      AND lower(lli.ItemType) = 'robot'
      AND lli.X = r.RobotX
      AND lli.Y = r.RobotY
);

INSERT INTO RobotLocationLayoutItemMap (RobotId, LocationLayoutItemId)
SELECT r.RobotId,
       COALESCE(
         (
           SELECT lli.LocationLayoutItemId
           FROM LocationLayoutItem lli
           WHERE lli.LocationId = r.LocationId
             AND lower(lli.ItemType) = 'robot'
             AND lli.X = r.RobotX
             AND lli.Y = r.RobotY
           ORDER BY lli.LocationLayoutItemId
           LIMIT 1
         ),
         (
           SELECT lli.LocationLayoutItemId
           FROM LocationLayoutItem lli
           WHERE lli.LocationId = r.LocationId
             AND lower(lli.ItemType) = 'robot'
           ORDER BY lli.LocationLayoutItemId
           LIMIT 1
         )
       )
FROM Robot r;

CREATE TABLE RobotPose_New (
    RobotPoseId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    LocationLayoutItemId INTEGER NOT NULL,
    PoseName TEXT NOT NULL DEFAULT '',
    Pose TEXT NOT NULL DEFAULT '[]',
    FOREIGN KEY (LocationLayoutItemId)
        REFERENCES LocationLayoutItem(LocationLayoutItemId) ON DELETE CASCADE
);

INSERT INTO RobotPose_New (RobotPoseId, LocationLayoutItemId, PoseName, Pose)
SELECT rp.RobotPoseId,
       map.LocationLayoutItemId,
       COALESCE(rp.PoseName, ''),
       COALESCE(rp.Pose, '[]')
FROM RobotPose rp
JOIN RobotLocationLayoutItemMap map ON map.RobotId = rp.RobotId;

DROP TABLE RobotPose;
ALTER TABLE RobotPose_New RENAME TO RobotPose;
CREATE INDEX idx_robot_pose_location_layout_item_id
ON RobotPose(LocationLayoutItemId);
DROP TABLE Robot;
DROP TABLE RobotLocationLayoutItemMap;

COMMIT;
PRAGMA foreign_keys = ON;
