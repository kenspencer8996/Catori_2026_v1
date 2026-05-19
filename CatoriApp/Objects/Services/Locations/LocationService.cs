namespace CatoriApp.Objects.Services.Locations
{
    public class LocationService
    {
        private readonly LocationRepository _locationRepository;
        private readonly LocationLayoutItemRepository _itemRepository;
        private readonly LocationLayoutPointRepository _pointRepository;
        private readonly LocationPartRouteRepository _routeRepository;

        public LocationService()
        {
            _locationRepository = new LocationRepository();
            _itemRepository = new LocationLayoutItemRepository();
            _pointRepository = new LocationLayoutPointRepository();
            _routeRepository = new LocationPartRouteRepository();
        }

        public async Task<LocationDesignerViewModel?> GetDesignerByLayoutIdAsync(long LocationId)
        {
            var layout = await _locationRepository.GetLayoutByIdAsync(LocationId);
            if (layout == null)
                return null;

            var location = await _locationRepository.GetByIdAsync((int)layout.LocationId);
            return await BuildDesignerAsync(location, layout);
        }

        public async Task<LocationDesignerViewModel?> GetActiveDesignerByLocationIdAsync(long locationId)
        {
            var location = await _locationRepository.GetByIdAsync((int)locationId);
            var layout = await _locationRepository.GetActiveLayoutByLocationIdAsync(locationId);

            if (layout == null)
                return null;

            return await BuildDesignerAsync(location, layout);
        }

        public async Task<long> SaveDesignerAsync(LocationDesignerViewModel vm)
        {
            var location = new LocationEntity
            {
                LocationId = vm.LocationId,
                LocationName = vm.LocationName,
                BackgroundImagePath = vm.BackgroundImagePath ?? "",
                InteriorType = nameof(LocationEntity),
                DesignWidth = vm.CanvasWidth,
                DesignHeight = vm.CanvasHeight,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            if (location.LocationId <= 0)
                vm.LocationId = await _locationRepository.InsertAsync(location);
            else
                await _locationRepository.UpdateAsync(location);

            var layout = new LocationLayoutEntity
            {
                 LocationId = vm.LocationId,
                LayoutName = vm.LayoutName,
                CanvasWidth = vm.CanvasWidth,
                CanvasHeight = vm.CanvasHeight,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            if (layout.LocationId <= 0)
                vm.LocationId = await _locationRepository.InsertLayoutAsync(layout);
            else
                await _locationRepository.UpdateLayoutAsync(layout);

            foreach (var itemVm in vm.Items)
            {
                itemVm.LocationId = vm.LocationId;
                var item = ToEntity(itemVm);

                if (item.LocationLayoutItemId <= 0)
                    itemVm.LocationLayoutItemId = await _itemRepository.InsertAsync(item);
                else
                    await _itemRepository.UpdateAsync(item);

                await _pointRepository.DeleteByItemIdAsync(itemVm.LocationLayoutItemId);
                for (int i = 0; i < itemVm.Points.Count; i++)
                {
                    var point = ToEntity(itemVm.Points[i]);
                    point.LocationLayoutItemId = itemVm.LocationLayoutItemId;
                    point.PointIndex = i;
                    itemVm.Points[i].LocationLayoutPointId = await _pointRepository.InsertAsync(point);
                }
            }

            return vm.LocationId;
        }

        public LocationViewModel? GetByLocationName(string locationName)
        {
            LocationViewModel model = GetByLocationNameAsync(locationName).GetAwaiter().GetResult();
            return model;
        }

        public async Task<LocationViewModel?> GetByLocationNameAsync(string locationName)
        {
            try
            {
                LocationEntity? location = await _locationRepository.GetByNameAsync(locationName);

                if (location == null)
                {
                    location = new LocationEntity
                    {
                        LocationName = locationName,
                        BackgroundImagePath = System.IO.Path.Combine(GlobalAllApps.ImageFolder, "Factory", locationName + ".png"),
                        InteriorType = nameof(LocationEntity),
                        DesignWidth = 1920,
                        DesignHeight = 1080,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    location.LocationId = await _locationRepository.InsertAsync(location);
                }

                var layout = await _locationRepository.GetActiveLayoutByLocationIdAsync(location.LocationId);
                if (layout == null)
                {
                    layout = new LocationLayoutEntity
                    {
                        LocationId = location.LocationId,
                        LayoutName = locationName + " Layout",
                        CanvasWidth = location.DesignWidth,
                        CanvasHeight = location.DesignHeight,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };
                    layout.LocationId = await _locationRepository.InsertLayoutAsync(layout);
                }

                return new LocationViewModel
                {
                     LocationId = layout.LocationId,
                    LocationName = locationName,
                    Points = await GetLegacyConveyorPointsAsync(layout.LocationId),
                    Description = location.Description,
                    BackgroundImagePath = location.BackgroundImagePath,
                    InteriorType = location.InteriorType,
                    WorldMapImagePath = location.WorldMapImagePath  ,
                    HotspotLeft = location.HotspotLeft,
                    HotspotTop = location.HotspotTop,
                    HotspotWidth = location.HotspotWidth,
                    HotspotHeight = location.HotspotHeight,
                    DesignWidth = location.DesignWidth,
                    DesignHeight = location.DesignHeight,
                    DefaultRobotX = location.DefaultRobotX,
                    DefaultRobotY = location.DefaultRobotY,
                    IsActive = location.IsActive,
                    SortOrder = location.SortOrder,
                    CreatedAt = location.CreatedAt,
                    UpdatedDate = location.UpdatedDate
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public LocationViewModel? GetByLocationId(long locationId)
        {
            return GetByLocationIdAsync(locationId).GetAwaiter().GetResult();
        }

        public async Task<LocationViewModel?> GetByLocationIdAsync(long locationId)
        {
            try
            {
                var location = await _locationRepository.GetByIdAsync((int)locationId);
                if (location == null)
                    return null;

                return new LocationViewModel
                {
                    LocationId = location.LocationId,
                    LocationName = location.LocationName,
                    BackgroundImagePath = location.BackgroundImagePath,
                    Description = location.Description,
                    InteriorType = location.InteriorType,
                    WorldMapImagePath = location.WorldMapImagePath,
                    HotspotLeft = location.HotspotLeft,
                    HotspotTop = location.HotspotTop,
                    HotspotWidth = location.HotspotWidth,
                    HotspotHeight = location.HotspotHeight,
                    DesignWidth = location.DesignWidth,
                    DesignHeight = location.DesignHeight,
                    DefaultRobotX = location.DefaultRobotX,
                    DefaultRobotY = location.DefaultRobotY,
                    IsActive = location.IsActive,
                    SortOrder = location.SortOrder,
                    Points = await GetLegacyConveyorPointsAsync(location.LocationId)
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public List<LocationAssemblyRobotEntity> GetRobotsForLocation(long LocationId)
        {
            return GetRobotsForLocationAsync(LocationId).GetAwaiter().GetResult();
        }

        public async Task<List<LocationAssemblyRobotEntity>> GetRobotsForLocationAsync(long LocationId)
        {
            var items = await _itemRepository.GetLayoutItemsByIdAsync(LocationId);

            return items
                .Where(i => i.ItemType == LocationLayoutItemType.Robot)
                .Select(ToRobot)
                .ToList();
        }

        public LocationAssemblyRobotEntity CreateRobot(string name, long LocationId, string robotType, long x, long y, double width, double height)
        {
            return CreateRobotAsync(name, LocationId, robotType, x, y, width, height).GetAwaiter().GetResult();
        }

        public async Task<LocationAssemblyRobotEntity> CreateRobotAsync(string name, long LocationId, string robotType, long x, long y, double width, double height)
        {
            var item = new LocationLayoutItemEntity
            {
                LocationId = LocationId,
                ItemName = name,
                ItemType = LocationLayoutItemType.Robot,
                X = x,
                Y = y,
                Width = width,
                Height = height,
                MetadataJson = $"{{\"robotType\":\"{robotType}\"}}"
            };

            item.LocationLayoutItemId = await _itemRepository.InsertAsync(item);
            return ToRobot(item);
        }

        public void UpdateRobot(LocationAssemblyRobotEntity robot)
        {
            UpdateRobotAsync(robot).GetAwaiter().GetResult();
        }

        public async Task<bool> UpdateRobotAsync(LocationAssemblyRobotEntity robot)
        {
            var item = robot.LocationLayoutItemId > 0
                ? await _itemRepository.GetByIdAsync(robot.LocationLayoutItemId)
                : null;

            if (item == null)
            {
                await CreateRobotAsync(robot.Name, robot.LocationId, robot.RobotType, robot.Xloc, robot.Yloc, robot.Width, robot.Height);
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

        public void UpdatePoints(long LocationId, LocationLayoutPointEntity point)
        {
            UpdatePointsAsync(LocationId, point).GetAwaiter().GetResult();
        }

        public async Task UpdatePointsAsync(long LocationId, LocationLayoutPointEntity point)
        {
            string conveyorName = string.IsNullOrWhiteSpace(point.PointType)
                ? "Conveyor"
                : "Conveyor " + point.PointType.Trim();

            var items = await _itemRepository.GetLayoutItemsByIdAsync(LocationId);
            var item = items.FirstOrDefault(i =>
                i.ItemType == LocationLayoutItemType.Conveyor &&
                string.Equals(i.ItemName, conveyorName, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                item = new LocationLayoutItemEntity
                {
                    LocationId = LocationId,
                    ItemName = conveyorName,
                    ItemType = LocationLayoutItemType.Conveyor
                };
                item.LocationLayoutItemId = await _itemRepository.InsertAsync(item);
            }

            await _pointRepository.DeleteByItemIdAsync(item.LocationLayoutItemId);

            await _pointRepository.InsertAsync(new LocationLayoutPointEntity
            {
                LocationLayoutItemId = item.LocationLayoutItemId,
                LocationId = LocationId,
                PointIndex = 0,
                PointRole = point.PointType,
                X = point.XLoc,
                Y = point.YLoc,
                SegmentKind = LocationLayoutSegmentKind.Line
            });

            await _pointRepository.InsertAsync(new LocationLayoutPointEntity
            {
                LocationLayoutItemId = item.LocationLayoutItemId,
                LocationId = LocationId,
                PointIndex = 1,
                PointRole = point.PointType,
                X = point.XLocEnd,
                Y = point.YLocEnd,
                SegmentKind = LocationLayoutSegmentKind.Line
            });
        }

        private async Task<LocationDesignerViewModel> BuildDesignerAsync(LocationEntity? location, LocationLayoutEntity layout)
        {
            var vm = new LocationDesignerViewModel
            {
                LocationId = layout.LocationId,
                LocationName = location?.LocationName ?? "",
                BackgroundImagePath = location?.BackgroundImagePath,
                LayoutName = layout.LayoutName,
                CanvasWidth = layout.CanvasWidth,
                CanvasHeight = layout.CanvasHeight
            };

            var items = await _itemRepository.GetLayoutItemsByIdAsync(layout.LocationId);
            foreach (var item in items)
            {
                var itemVm = ToViewModel(item);
                var points = await _pointRepository.GetByItemIdAsync(item.LocationLayoutItemId);
                foreach (var point in points)
                    itemVm.Points.Add(ToViewModel(point));

                vm.Items.Add(itemVm);
            }

            var routes = await _routeRepository.GetByLayoutIdAsync(layout.LocationId);
            foreach (var route in routes)
            {
                var routeVm = ToViewModel(route);
                var routePoints = await _routeRepository.GetPointsAsync(route.LocationPartRouteId);
                foreach (var point in routePoints)
                    routeVm.Points.Add(ToViewModel(point));

                vm.Routes.Add(routeVm);
            }

            return vm;
        }

        private async Task<List<LocationLayoutPointEntity>> GetLegacyConveyorPointsAsync(long LocationId)
        {
            var items = await _itemRepository.GetLayoutItemsByIdAsync(LocationId);
            var results = new List<LocationLayoutPointEntity>();

            foreach (var item in items.Where(i => i.ItemType == LocationLayoutItemType.Conveyor))
            {
                var points = await _pointRepository.GetByItemIdAsync(item.LocationLayoutItemId);
                if (points.Count < 2)
                    continue;

                string pointType = points[0].PointRole ?? item.ItemName.Replace("Conveyor", "").Trim();
                results.Add(new LocationLayoutPointEntity
                {
                    LocationLayoutPointId = points[0].LocationLayoutPointId,
                    LocationLayoutItemId = item.LocationLayoutItemId,
                    LocationId = LocationId,
                    PointType = pointType,
                    XLoc = points[0].X,
                    YLoc = points[0].Y,
                    XLocEnd = points[1].X,
                    YLocEnd = points[1].Y
                });
            }

            return results;
        }

        private static LocationLayoutItemEntity ToEntity(LocationLayoutItemViewModel vm)
        {
            return new LocationLayoutItemEntity
            {
                LocationLayoutItemId = vm.LocationLayoutItemId,
                LocationId = vm.LocationId,
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

        private static LocationLayoutPointEntity ToEntity(LocationLayoutPointViewModel vm)
        {
            return new LocationLayoutPointEntity
            {
                LocationLayoutPointId = vm.LocationLayoutPointId,
                LocationLayoutItemId = vm.LocationLayoutItemId,
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

        private static LocationLayoutItemViewModel ToViewModel(LocationLayoutItemEntity entity)
        {
            return new LocationLayoutItemViewModel
            {
                LocationLayoutItemId = entity.LocationLayoutItemId,
                LocationId = entity.LocationId,
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

        private static LocationLayoutPointViewModel ToViewModel(LocationLayoutPointEntity entity)
        {
            return new LocationLayoutPointViewModel
            {
                LocationLayoutPointId = entity.LocationLayoutPointId,
                LocationLayoutItemId = entity.LocationLayoutItemId,
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

        private static LocationPartRouteViewModel ToViewModel(LocationPartRouteEntity entity)
        {
            return new LocationPartRouteViewModel
            {
                LocationPartRouteId = entity.LocationPartRouteId,
                LocationId = entity.LocationId,
                RouteName = entity.RouteName,
                ProductId = entity.ProductId,
                FromItemId = entity.FromItemId,
                ToItemId = entity.ToItemId,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            };
        }

        private static LocationPartRoutePointViewModel ToViewModel(LocationPartRoutePointEntity entity)
        {
            return new LocationPartRoutePointViewModel
            {
                LocationPartRoutePointId = entity.LocationPartRoutePointId,
                LocationPartRouteId = entity.LocationPartRouteId,
                PointIndex = entity.PointIndex,
                X = entity.X,
                Y = entity.Y,
                Z = entity.Z,
                SecondsFromStart = entity.SecondsFromStart
            };
        }

        private static LocationAssemblyRobotEntity ToRobot(LocationLayoutItemEntity item)
        {
            return new LocationAssemblyRobotEntity
            {
                LocationAssemblyRobotId = item.LocationLayoutItemId,
                LocationId = item.LocationId,
                LocationLayoutItemId = item.LocationLayoutItemId,
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


