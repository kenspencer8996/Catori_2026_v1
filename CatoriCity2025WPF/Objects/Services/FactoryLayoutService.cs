using CatoriServices.Objects.database;
using CatoriServices.Objects.Entities;
using System.Collections.ObjectModel;

namespace CatoriCity2025WPF.Objects.Services
{
    public class FactoryLayoutService
    {
        private readonly FactoryRepository _factoryRepository;
        private readonly FactoryLayoutRepository _layoutRepository;
        private readonly FactoryLayoutItemRepository _itemRepository;
        private readonly FactoryLayoutPointRepository _pointRepository;
        private readonly FactoryPartRouteRepository _routeRepository;

        public FactoryLayoutService()
        {
            _factoryRepository = new FactoryRepository();
            _layoutRepository = new FactoryLayoutRepository();
            _itemRepository = new FactoryLayoutItemRepository();
            _pointRepository = new FactoryLayoutPointRepository();
            _routeRepository = new FactoryPartRouteRepository();
        }

        public async Task<FactoryDesignerViewModel?> GetDesignerByLayoutIdAsync(long factoryLayoutId)
        {
            var layout = await _layoutRepository.GetByIdAsync(factoryLayoutId);
            if (layout == null)
                return null;

            var factory = await _factoryRepository.GetByIdAsync((int)layout.FactoryId);
            return await BuildDesignerAsync(factory, layout);
        }

        public async Task<FactoryDesignerViewModel?> GetActiveDesignerByFactoryIdAsync(long factoryId)
        {
            var factory = await _factoryRepository.GetByIdAsync((int)factoryId);
            var layout = await _layoutRepository.GetActiveByFactoryIdAsync(factoryId);

            if (layout == null)
                return null;

            return await BuildDesignerAsync(factory, layout);
        }

        public async Task<long> SaveDesignerAsync(FactoryDesignerViewModel vm)
        {
            var factory = new FactoryEntity
            {
                FactoryId = vm.FactoryId,
                FactoryName = vm.FactoryName,
                BackgroundImagePath = vm.BackgroundImagePath,
                CreatedAt = DateTime.Now
            };

            if (factory.FactoryId <= 0)
                vm.FactoryId = await _factoryRepository.InsertAsync(factory);
            else
                await _factoryRepository.UpdateAsync(factory);

            var layout = new FactoryLayoutEntity
            {
                FactoryLayoutId = vm.FactoryLayoutId,
                FactoryId = vm.FactoryId,
                LayoutName = vm.LayoutName,
                CanvasWidth = vm.CanvasWidth,
                CanvasHeight = vm.CanvasHeight,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            if (layout.FactoryLayoutId <= 0)
                vm.FactoryLayoutId = await _layoutRepository.InsertAsync(layout);
            else
                await _layoutRepository.UpdateAsync(layout);

            foreach (var itemVm in vm.Items)
            {
                itemVm.FactoryLayoutId = vm.FactoryLayoutId;
                var item = ToEntity(itemVm);

                if (item.FactoryLayoutItemId <= 0)
                    itemVm.FactoryLayoutItemId = await _itemRepository.InsertAsync(item);
                else
                    await _itemRepository.UpdateAsync(item);

                await _pointRepository.DeleteByItemIdAsync(itemVm.FactoryLayoutItemId);
                for (int i = 0; i < itemVm.Points.Count; i++)
                {
                    var point = ToEntity(itemVm.Points[i]);
                    point.FactoryLayoutItemId = itemVm.FactoryLayoutItemId;
                    point.PointIndex = i;
                    itemVm.Points[i].FactoryLayoutPointId = await _pointRepository.InsertAsync(point);
                }
            }

            return vm.FactoryLayoutId;
        }

        public FactoryLayoutViewModel? GetByFactoryInteriorName(string factoryInteriorName)
        {
            return GetByFactoryInteriorNameAsync(factoryInteriorName).GetAwaiter().GetResult();
        }

        public async Task<FactoryLayoutViewModel?> GetByFactoryInteriorNameAsync(string factoryInteriorName)
        {
            var factories = await _factoryRepository.GetAllAsync();
            var factory = factories.FirstOrDefault(f => string.Equals(f.FactoryName, factoryInteriorName, StringComparison.OrdinalIgnoreCase));

            if (factory == null)
            {
                factory = new FactoryEntity
                {
                    FactoryName = factoryInteriorName,
                    CreatedAt = DateTime.Now
                };
                factory.FactoryId = await _factoryRepository.InsertAsync(factory);
            }

            var layout = await _layoutRepository.GetActiveByFactoryIdAsync(factory.FactoryId);
            if (layout == null)
            {
                layout = new FactoryLayoutEntity
                {
                    FactoryId = factory.FactoryId,
                    LayoutName = factoryInteriorName + " Layout",
                    CanvasWidth = 1920,
                    CanvasHeight = 1080,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                layout.FactoryLayoutId = await _layoutRepository.InsertAsync(layout);
            }

            return new FactoryLayoutViewModel
            {
                FactoryLayoutId = layout.FactoryLayoutId,
                FactoryInteriorName = factoryInteriorName,
                Points = await GetLegacyConveyorPointsAsync(layout.FactoryLayoutId)
            };
        }

        public List<FactoryAssemblyRobotEntity> GetRobotsForFactory(long factoryLayoutId)
        {
            return GetRobotsForFactoryAsync(factoryLayoutId).GetAwaiter().GetResult();
        }

        public async Task<List<FactoryAssemblyRobotEntity>> GetRobotsForFactoryAsync(long factoryLayoutId)
        {
            var items = await _itemRepository.GetByLayoutIdAsync(factoryLayoutId);

            return items
                .Where(i => i.ItemType == FactoryLayoutItemType.Robot)
                .Select(ToRobot)
                .ToList();
        }

        public FactoryAssemblyRobotEntity CreateRobot(string name, long factoryLayoutId, string robotType, long x, long y, double width, double height)
        {
            return CreateRobotAsync(name, factoryLayoutId, robotType, x, y, width, height).GetAwaiter().GetResult();
        }

        public async Task<FactoryAssemblyRobotEntity> CreateRobotAsync(string name, long factoryLayoutId, string robotType, long x, long y, double width, double height)
        {
            var item = new FactoryLayoutItemEntity
            {
                FactoryLayoutId = factoryLayoutId,
                ItemName = name,
                ItemType = FactoryLayoutItemType.Robot,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                MetadataJson = $"{{\"robotType\":\"{robotType}\"}}"
            };

            item.FactoryLayoutItemId = await _itemRepository.InsertAsync(item);
            return ToRobot(item);
        }

        public void UpdateRobot(FactoryAssemblyRobotEntity robot)
        {
            UpdateRobotAsync(robot).GetAwaiter().GetResult();
        }

        public async Task<bool> UpdateRobotAsync(FactoryAssemblyRobotEntity robot)
        {
            var item = robot.FactoryLayoutItemId > 0
                ? await _itemRepository.GetByIdAsync(robot.FactoryLayoutItemId)
                : null;

            if (item == null)
            {
                await CreateRobotAsync(robot.Name, robot.FactoryLayoutId, robot.RobotType, robot.Xloc, robot.Yloc, robot.Width, robot.Height);
                return true;
            }

            item.ItemName = robot.Name;
            item.X = robot.Xloc;
            item.Y = robot.Yloc;
            item.Width = robot.Width;
            item.Height = robot.Height;
            item.RotationDegrees = robot.RotationDegrees;
            item.MetadataJson = $"{{\"robotType\":\"{robot.RobotType}\"}}";

            return await _itemRepository.UpdateAsync(item);
        }

        public void UpdatePoints(long factoryLayoutId, FactoryLayoutPointEntity point)
        {
            UpdatePointsAsync(factoryLayoutId, point).GetAwaiter().GetResult();
        }

        public async Task UpdatePointsAsync(long factoryLayoutId, FactoryLayoutPointEntity point)
        {
            string conveyorName = string.IsNullOrWhiteSpace(point.PointType)
                ? "Conveyor"
                : "Conveyor " + point.PointType.Trim();

            var items = await _itemRepository.GetByLayoutIdAsync(factoryLayoutId);
            var item = items.FirstOrDefault(i =>
                i.ItemType == FactoryLayoutItemType.Conveyor &&
                string.Equals(i.ItemName, conveyorName, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                item = new FactoryLayoutItemEntity
                {
                    FactoryLayoutId = factoryLayoutId,
                    ItemName = conveyorName,
                    ItemType = FactoryLayoutItemType.Conveyor
                };
                item.FactoryLayoutItemId = await _itemRepository.InsertAsync(item);
            }

            await _pointRepository.DeleteByItemIdAsync(item.FactoryLayoutItemId);

            await _pointRepository.InsertAsync(new FactoryLayoutPointEntity
            {
                FactoryLayoutItemId = item.FactoryLayoutItemId,
                FactoryLayoutId = factoryLayoutId,
                PointIndex = 0,
                PointRole = point.PointType,
                X = point.XLoc,
                Y = point.YLoc,
                SegmentKind = FactoryLayoutSegmentKind.Line
            });

            await _pointRepository.InsertAsync(new FactoryLayoutPointEntity
            {
                FactoryLayoutItemId = item.FactoryLayoutItemId,
                FactoryLayoutId = factoryLayoutId,
                PointIndex = 1,
                PointRole = point.PointType,
                X = point.XLocEnd,
                Y = point.YLocEnd,
                SegmentKind = FactoryLayoutSegmentKind.Line
            });
        }

        private async Task<FactoryDesignerViewModel> BuildDesignerAsync(FactoryEntity? factory, FactoryLayoutEntity layout)
        {
            var vm = new FactoryDesignerViewModel
            {
                FactoryId = layout.FactoryId,
                FactoryLayoutId = layout.FactoryLayoutId,
                FactoryName = factory?.FactoryName ?? "",
                BackgroundImagePath = factory?.BackgroundImagePath,
                LayoutName = layout.LayoutName,
                CanvasWidth = layout.CanvasWidth,
                CanvasHeight = layout.CanvasHeight
            };

            var items = await _itemRepository.GetByLayoutIdAsync(layout.FactoryLayoutId);
            foreach (var item in items)
            {
                var itemVm = ToViewModel(item);
                var points = await _pointRepository.GetByItemIdAsync(item.FactoryLayoutItemId);
                foreach (var point in points)
                    itemVm.Points.Add(ToViewModel(point));

                vm.Items.Add(itemVm);
            }

            var routes = await _routeRepository.GetByLayoutIdAsync(layout.FactoryLayoutId);
            foreach (var route in routes)
            {
                var routeVm = ToViewModel(route);
                var routePoints = await _routeRepository.GetPointsAsync(route.FactoryPartRouteId);
                foreach (var point in routePoints)
                    routeVm.Points.Add(ToViewModel(point));

                vm.Routes.Add(routeVm);
            }

            return vm;
        }

        private async Task<List<FactoryLayoutPointEntity>> GetLegacyConveyorPointsAsync(long factoryLayoutId)
        {
            var items = await _itemRepository.GetByLayoutIdAsync(factoryLayoutId);
            var results = new List<FactoryLayoutPointEntity>();

            foreach (var item in items.Where(i => i.ItemType == FactoryLayoutItemType.Conveyor))
            {
                var points = await _pointRepository.GetByItemIdAsync(item.FactoryLayoutItemId);
                if (points.Count < 2)
                    continue;

                string pointType = points[0].PointRole ?? item.ItemName.Replace("Conveyor", "").Trim();
                results.Add(new FactoryLayoutPointEntity
                {
                    FactoryLayoutPointId = points[0].FactoryLayoutPointId,
                    FactoryLayoutItemId = item.FactoryLayoutItemId,
                    FactoryLayoutId = factoryLayoutId,
                    PointType = pointType,
                    XLoc = points[0].X,
                    YLoc = points[0].Y,
                    XLocEnd = points[1].X,
                    YLocEnd = points[1].Y
                });
            }

            return results;
        }

        private static FactoryLayoutItemEntity ToEntity(FactoryLayoutItemViewModel vm)
        {
            return new FactoryLayoutItemEntity
            {
                FactoryLayoutItemId = vm.FactoryLayoutItemId,
                FactoryLayoutId = vm.FactoryLayoutId,
                ItemName = vm.ItemName,
                ItemType = vm.ItemType,
                X = vm.X,
                Y = vm.Y,
                Z = vm.Z,
                Width = vm.Width,
                Height = vm.Height,
                RotationDegrees = vm.RotationDegrees,
                ZIndex = vm.ZIndex,
                IsLocked = vm.IsLocked,
                ImagePath = vm.ImagePath,
                MetadataJson = vm.MetadataJson
            };
        }

        private static FactoryLayoutPointEntity ToEntity(FactoryLayoutPointViewModel vm)
        {
            return new FactoryLayoutPointEntity
            {
                FactoryLayoutPointId = vm.FactoryLayoutPointId,
                FactoryLayoutItemId = vm.FactoryLayoutItemId,
                PointIndex = vm.PointIndex,
                PointRole = vm.PointRole,
                X = vm.X,
                Y = vm.Y,
                Z = vm.Z,
                SegmentKind = vm.SegmentKind,
                Control1X = vm.Control1X,
                Control1Y = vm.Control1Y,
                Control2X = vm.Control2X,
                Control2Y = vm.Control2Y,
                RotationDegrees = vm.RotationDegrees
            };
        }

        private static FactoryLayoutItemViewModel ToViewModel(FactoryLayoutItemEntity entity)
        {
            return new FactoryLayoutItemViewModel
            {
                FactoryLayoutItemId = entity.FactoryLayoutItemId,
                FactoryLayoutId = entity.FactoryLayoutId,
                ItemName = entity.ItemName,
                ItemType = entity.ItemType,
                X = entity.X,
                Y = entity.Y,
                Z = entity.Z,
                Width = entity.Width,
                Height = entity.Height,
                RotationDegrees = entity.RotationDegrees,
                ZIndex = entity.ZIndex,
                IsLocked = entity.IsLocked,
                ImagePath = entity.ImagePath,
                MetadataJson = entity.MetadataJson
            };
        }

        private static FactoryLayoutPointViewModel ToViewModel(FactoryLayoutPointEntity entity)
        {
            return new FactoryLayoutPointViewModel
            {
                FactoryLayoutPointId = entity.FactoryLayoutPointId,
                FactoryLayoutItemId = entity.FactoryLayoutItemId,
                PointIndex = entity.PointIndex,
                PointRole = entity.PointRole,
                X = entity.X,
                Y = entity.Y,
                Z = entity.Z,
                SegmentKind = entity.SegmentKind,
                Control1X = entity.Control1X,
                Control1Y = entity.Control1Y,
                Control2X = entity.Control2X,
                Control2Y = entity.Control2Y,
                RotationDegrees = entity.RotationDegrees
            };
        }

        private static FactoryPartRouteViewModel ToViewModel(FactoryPartRouteEntity entity)
        {
            return new FactoryPartRouteViewModel
            {
                FactoryPartRouteId = entity.FactoryPartRouteId,
                FactoryLayoutId = entity.FactoryLayoutId,
                RouteName = entity.RouteName,
                ProductId = entity.ProductId,
                FromItemId = entity.FromItemId,
                ToItemId = entity.ToItemId,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        private static FactoryPartRoutePointViewModel ToViewModel(FactoryPartRoutePointEntity entity)
        {
            return new FactoryPartRoutePointViewModel
            {
                FactoryPartRoutePointId = entity.FactoryPartRoutePointId,
                FactoryPartRouteId = entity.FactoryPartRouteId,
                PointIndex = entity.PointIndex,
                X = entity.X,
                Y = entity.Y,
                Z = entity.Z,
                SecondsFromStart = entity.SecondsFromStart
            };
        }

        private static FactoryAssemblyRobotEntity ToRobot(FactoryLayoutItemEntity item)
        {
            return new FactoryAssemblyRobotEntity
            {
                FactoryAssemblyRobotId = item.FactoryLayoutItemId,
                FactoryLayoutId = item.FactoryLayoutId,
                FactoryLayoutItemId = item.FactoryLayoutItemId,
                Name = item.ItemName,
                RobotType = "Arm",
                Xloc = (long)item.X,
                Yloc = (long)item.Y,
                Width = item.Width,
                Height = item.Height,
                RotationDegrees = item.RotationDegrees
            };
        }
    }
}
