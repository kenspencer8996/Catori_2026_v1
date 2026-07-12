namespace CatoriApp.Game.Objects.Services.Locations
{
    public class LocationLayoutItemService
    {
        private readonly LocationLayoutItemRepository _itemRepository;
        private readonly LocationLayoutPointRepository _pointRepository;

        public LocationLayoutItemService()
        {
            _itemRepository = new LocationLayoutItemRepository();
            _pointRepository = new LocationLayoutPointRepository();
        }

        public async Task<LocationLayoutItemViewModel?> GetByIdAsync(long itemId, bool includePoints = true)
        {
            var entity = await _itemRepository.GetByIdAsync(itemId);
            if (entity == null)
                return null;

            var viewModel = ToViewModel(entity);

            if (includePoints)
                await LoadPointsAsync(viewModel);

            return viewModel;
        }

        public async Task<List<LocationLayoutItemViewModel>> GetByLocationIdAsync(long locationId, bool includePoints = true)
        {
            var entities = await _itemRepository.GetByLocationIdAsync(locationId);
            var viewModels = entities.Select(ToViewModel).ToList();

            if (includePoints)
            {
                foreach (var viewModel in viewModels)
                    await LoadPointsAsync(viewModel);
            }

            return viewModels;
        }

        public async Task<long> SaveAsync(LocationLayoutItemViewModel viewModel, bool savePoints = true)
        {
            Validate(viewModel);

            var entity = ToEntity(viewModel);

            if (entity.LocationLayoutItemId <= 0)
                viewModel.LocationLayoutItemId = await _itemRepository.InsertAsync(entity);
            else
                await _itemRepository.UpdateAsync(entity);

            if (savePoints)
                await ReplacePointsAsync(viewModel);

            return viewModel.LocationLayoutItemId;
        }

        public async Task<bool> DeleteAsync(LocationLayoutItemViewModel viewModel)
        {
            if (viewModel.LocationLayoutItemId <= 0)
                return false;

            return await DeleteAsync(viewModel.LocationLayoutItemId);
        }

        public async Task<bool> DeleteAsync(long itemId)
        {
            if (itemId <= 0)
                return false;

            await _pointRepository.DeleteByItemIdAsync(itemId);
            return await _itemRepository.DeleteAsync(itemId);
        }

        public async Task LoadPointsAsync(LocationLayoutItemViewModel viewModel)
        {
            viewModel.Points.Clear();

            if (viewModel.LocationLayoutItemId <= 0)
                return;

            var points = await _pointRepository.GetByItemIdAsync(viewModel.LocationLayoutItemId);
            foreach (var point in points)
                viewModel.Points.Add(ToViewModel(point));
        }

        public async Task ReplacePointsAsync(LocationLayoutItemViewModel viewModel)
        {
            if (viewModel.LocationLayoutItemId <= 0)
                throw new InvalidOperationException("Save the location layout item before saving points.");

            await _pointRepository.DeleteByItemIdAsync(viewModel.LocationLayoutItemId);

            for (int i = 0; i < viewModel.Points.Count; i++)
            {
                var pointViewModel = viewModel.Points[i];
                var point = ToEntity(pointViewModel);
                point.LocationLayoutItemId = viewModel.LocationLayoutItemId;
                point.LocationId = viewModel.LocationId;
                point.PointIndex = i;

                pointViewModel.LocationLayoutItemId = viewModel.LocationLayoutItemId;
                pointViewModel.LocationId = viewModel.LocationId;
                pointViewModel.PointIndex = i;
                pointViewModel.LocationLayoutPointId = await _pointRepository.InsertAsync(point);
            }
        }

        private static void Validate(LocationLayoutItemViewModel viewModel)
        {
            if (viewModel.LocationId <= 0)
                throw new InvalidOperationException("Location id is required.");

            if (string.IsNullOrWhiteSpace(viewModel.ItemName))
                throw new InvalidOperationException("Item name is required.");
        }

        private static LocationLayoutItemEntity ToEntity(LocationLayoutItemViewModel viewModel)
        {
            return new LocationLayoutItemEntity
            {
                LocationLayoutItemId = viewModel.LocationLayoutItemId,
                LocationId = viewModel.LocationId,
                ItemName = viewModel.ItemName.Trim(),
                ItemType = viewModel.ItemType,
                MajorItemType = string.IsNullOrWhiteSpace(viewModel.MajorItemType) ? null : viewModel.MajorItemType.Trim(),
                X = viewModel.X,
                Y = viewModel.Y,
                Z = viewModel.Z,
                Width = viewModel.Width,
                Height = viewModel.Height,
                RotationDegrees = viewModel.RotationDegrees,
                ZIndex = viewModel.ZIndex,
                IsLocked = viewModel.IsLocked,
                MetadataJson = viewModel.MetadataJson
            };
        }

        private static LocationLayoutPointEntity ToEntity(LocationLayoutPointViewModel viewModel)
        {
            return new LocationLayoutPointEntity
            {
                LocationLayoutPointId = viewModel.LocationLayoutPointId,
                LocationLayoutItemId = viewModel.LocationLayoutItemId,
                LocationId = viewModel.LocationId,
                PointIndex = viewModel.PointIndex,
                PointRole = viewModel.PointRole,
                X = viewModel.X,
                Y = viewModel.Y,
                Z = viewModel.Z,
                SegmentKind = viewModel.SegmentKind,
                Control1X = viewModel.Control1X,
                Control1Y = viewModel.Control1Y,
                Control2X = viewModel.Control2X,
                Control2Y = viewModel.Control2Y,
                RotationDegrees = viewModel.RotationDegrees
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
                MajorItemType = entity.MajorItemType,
                X = entity.X,
                Y = entity.Y,
                Z = entity.Z,
                Width = entity.Width,
                Height = entity.Height,
                RotationDegrees = entity.RotationDegrees,
                ZIndex = entity.ZIndex,
                IsLocked = entity.IsLocked,
                MetadataJson = entity.MetadataJson
            };
        }

        private static LocationLayoutPointViewModel ToViewModel(LocationLayoutPointEntity entity)
        {
            return new LocationLayoutPointViewModel
            {
                LocationLayoutPointId = entity.LocationLayoutPointId,
                LocationLayoutItemId = entity.LocationLayoutItemId,
                LocationId = entity.LocationId,
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
    }
}
