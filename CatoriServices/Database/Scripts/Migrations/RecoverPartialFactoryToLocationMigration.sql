-- Recovery helper if RenameFactoryTablesToLocation.sql already renamed Factory
-- to Location, then failed at:
--   no such column: "FactoryId"
--
-- Use this if CheckFactoryLocationSchema.sql shows:
--   Location exists and already has LocationId / LocationName
--   Factory no longer exists
--   FactoryLayoutItem and FactoryLayoutPoint still exist
--
-- Make a backup of the .db file before running this script.

PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

ALTER TABLE FactoryLayoutItem RENAME TO LocationLayoutItem;
ALTER TABLE FactoryLayoutPoint RENAME TO LocationLayoutPoint;

-- Uncomment these only if the columns still have Factory* names.
-- If they are already Location* names, leave them commented.
--
-- ALTER TABLE LocationLayoutItem RENAME COLUMN FactoryLayoutItemId TO LocationLayoutItemId;
-- ALTER TABLE LocationLayoutItem RENAME COLUMN FactoryId TO LocationId;
-- ALTER TABLE LocationLayoutPoint RENAME COLUMN FactoryLayoutPointId TO LocationLayoutPointId;
-- ALTER TABLE LocationLayoutPoint RENAME COLUMN FactoryLayoutItemId TO LocationLayoutItemId;
-- ALTER TABLE LocationLayoutPoint RENAME COLUMN FactoryId TO LocationId;
-- ALTER TABLE MachineLayoutDesigner RENAME COLUMN FactoryId TO LocationId;
-- ALTER TABLE RobotPose RENAME COLUMN FactoryId TO LocationId;

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
