-- One-time SQLite migration from Factory* schema names to Location* schema names.
-- Run this ONLY against an existing database whose tables and columns are both
-- still Factory-based:
--   Factory.FactoryId, Factory.FactoryName
--   FactoryLayoutItem.FactoryLayoutItemId, FactoryLayoutItem.FactoryId
--   FactoryLayoutPoint.FactoryLayoutPointId, FactoryLayoutPoint.FactoryLayoutItemId, FactoryLayoutPoint.FactoryId
--
-- If the database already has LocationId / LocationName columns, do not run this
-- file. Use RenameFactoryTablesToLocation_ColumnsAlreadyRenamed.sql instead.
--
-- Important:
-- SQLite ALTER TABLE statements are not conditional. If your database is already
-- migrated, partially migrated, or if an optional legacy table does not exist,
-- the matching ALTER TABLE statement will fail.
-- Make a backup of the .db file before running this script.

PRAGMA foreign_keys = OFF;
BEGIN TRANSACTION;

-- Core live layout tables.
ALTER TABLE Factory RENAME TO Location;
ALTER TABLE Location RENAME COLUMN FactoryId TO LocationId;
ALTER TABLE Location RENAME COLUMN FactoryName TO LocationName;

ALTER TABLE FactoryLayoutItem RENAME TO LocationLayoutItem;
ALTER TABLE LocationLayoutItem RENAME COLUMN FactoryLayoutItemId TO LocationLayoutItemId;
ALTER TABLE LocationLayoutItem RENAME COLUMN FactoryId TO LocationId;

ALTER TABLE FactoryLayoutPoint RENAME TO LocationLayoutPoint;
ALTER TABLE LocationLayoutPoint RENAME COLUMN FactoryLayoutPointId TO LocationLayoutPointId;
ALTER TABLE LocationLayoutPoint RENAME COLUMN FactoryLayoutItemId TO LocationLayoutItemId;
ALTER TABLE LocationLayoutPoint RENAME COLUMN FactoryId TO LocationId;

-- Robot designer tables now point at Location instead of Factory.
ALTER TABLE MachineLayoutDesigner RENAME COLUMN FactoryId TO LocationId;
ALTER TABLE RobotPose RENAME COLUMN FactoryId TO LocationId;

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

-- Optional legacy sections:
-- Run these only if the corresponding old tables exist in your database.
--
-- ALTER TABLE FactoryLayoutMachine RENAME TO LocationLayoutMachine;
-- ALTER TABLE LocationLayoutMachine RENAME COLUMN FactoryLayoutMachineId TO LocationLayoutMachineId;
-- ALTER TABLE LocationLayoutMachine RENAME COLUMN FactoryId TO LocationId;
--
-- ALTER TABLE FactoryInterior RENAME TO LocationInterior;
-- ALTER TABLE LocationInterior RENAME COLUMN FactoryId TO LocationId;
--
-- ALTER TABLE FactoryLayoutObject RENAME TO LocationLayoutObject;
-- ALTER TABLE LocationLayoutObject RENAME COLUMN FactoryId TO LocationId;
--
-- ALTER TABLE FactoryLayoutObjectPoint RENAME TO LocationLayoutObjectPoint;
