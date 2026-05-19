-- Diagnostic helper for the Factory -> Location rename.
-- Run this first and look at the table/column names it returns.

SELECT name AS table_name
FROM sqlite_master
WHERE type = 'table'
  AND name IN (
      'Factory',
      'Location',
      'FactoryLayoutItem',
      'LocationLayoutItem',
      'FactoryLayoutPoint',
      'LocationLayoutPoint',
      'FactoryLayoutMachine',
      'LocationLayoutMachine',
      'MachineLayoutDesigner',
      'RobotPose'
  )
ORDER BY name;

PRAGMA table_info('Factory');
PRAGMA table_info('Location');
PRAGMA table_info('FactoryLayoutItem');
PRAGMA table_info('LocationLayoutItem');
PRAGMA table_info('FactoryLayoutPoint');
PRAGMA table_info('LocationLayoutPoint');
PRAGMA table_info('MachineLayoutDesigner');
PRAGMA table_info('RobotPose');
