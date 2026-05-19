-- One-time migration from the old MachineLayoutDesignerSequence / MachineLayoutDesignerPose names.
-- New databases should use CreateMachineLayoutDesignerTables.sql directly.

ALTER TABLE MachineLayoutDesignerSequence RENAME TO MachineLayoutDesigner;

ALTER TABLE MachineLayoutDesigner RENAME COLUMN MachineLayoutDesignerSequenceId TO MachineLayoutDesignerId;
ALTER TABLE MachineLayoutDesigner ADD COLUMN LocationId INTEGER NOT NULL DEFAULT 0;

ALTER TABLE MachineLayoutDesignerPose RENAME TO RobotPose;

ALTER TABLE RobotPose RENAME COLUMN MachineLayoutDesignerPoseId TO RobotPoseId;
ALTER TABLE RobotPose RENAME COLUMN MachineLayoutDesignerSequenceId TO LocationId;

CREATE UNIQUE INDEX IF NOT EXISTS idx_machine_layout_designer_sequence_name
ON MachineLayoutDesigner(SequenceName);

CREATE INDEX IF NOT EXISTS idx_machine_layout_designer_location_id
ON MachineLayoutDesigner(LocationId);

CREATE INDEX IF NOT EXISTS idx_robot_pose_location_sequence
ON RobotPose(LocationId, PoseIndex);
