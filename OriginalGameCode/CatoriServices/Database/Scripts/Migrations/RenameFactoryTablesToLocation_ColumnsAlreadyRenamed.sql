-- One-time SQLite migration for databases where the Factory table names remain,
-- but the key columns were already renamed to LocationId / LocationName.
--
-- Use this if CheckFactoryLocationSchema.sql shows:
--   Factory has LocationId / LocationName, not FactoryId / FactoryName
--   FactoryLayoutItem has LocationLayoutItemId / LocationId
--   FactoryLayoutPoint has LocationLayoutPointId / LocationLayoutItemId / LocationId
--
-- Make a backup of the .db file before running this script.

PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

ALTER TABLE Factory RENAME TO Location;
ALTER TABLE FactoryLayoutItem RENAME TO LocationLayoutItem;
ALTER TABLE FactoryLayoutPoint RENAME TO LocationLayoutPoint;

DROP INDEX IF EXISTS idx_factory_layout_item_layout_id;
DROP INDEX IF EXISTS idx_factory_layout_point_item_id;
DROP INDEX IF EXISTS idx_machine_layout_designer_factory_id;
DROP INDEX IF EXISTS idx_robot_pose_factory_sequence;

CREATE INDEX IF NOT EXISTS idx_location_layout_item_layout_id
ON LocationLayoutItem(LocationId);

CREATE INDEX IF NOT EXISTS idx_location_layout_point_item_id
ON LocationLayoutPoint(LocationLayoutItemId);

CREATE INDEX IF NOT EXISTS idx_machine_layout_designer_location_id
ON MachineLayoutDesigner(LocationId);

CREATE INDEX IF NOT EXISTS idx_robot_pose_location_sequence
ON RobotPose(LocationId, PoseIndex);

COMMIT;
PRAGMA foreign_keys = ON;
