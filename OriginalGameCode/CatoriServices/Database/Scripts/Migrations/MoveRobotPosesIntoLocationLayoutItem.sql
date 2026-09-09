-- Run after RenameLayoutTypesAndLinkRobotPoseToLayoutItem.sql.
-- Preserves all named robot poses as JSON on their LocationLayoutItem.
PRAGMA foreign_keys = OFF;
BEGIN IMMEDIATE TRANSACTION;

ALTER TABLE LocationLayoutItem RENAME COLUMN WpfPath TO ItemDataJson;

UPDATE LocationLayoutItem
SET ItemDataJson = (
    SELECT json_group_array(
        json_object(
            'PoseName', COALESCE(rp.PoseName, ''),
            'Angles', json(COALESCE(rp.Pose, '[]'))
        )
    )
    FROM RobotPose rp
    WHERE rp.LocationLayoutItemId = LocationLayoutItem.LocationLayoutItemId
)
WHERE lower(ItemType) = 'robot'
  AND EXISTS (
      SELECT 1 FROM RobotPose rp
      WHERE rp.LocationLayoutItemId = LocationLayoutItem.LocationLayoutItemId
  );

DROP TABLE RobotPose;

COMMIT;
PRAGMA foreign_keys = ON;

SELECT LocationLayoutItemId, ItemName, ItemDataJson
FROM LocationLayoutItem
WHERE lower(ItemType) = 'robot';
