using System;
using System.Collections.Generic;
using System.Text;
namespace CatoriApp.Game.Objects.Services.Manufacturing
{
    public class BillOfMaterialsService
    {
            private BillOfMaterialsRepository _repository;

            public BillOfMaterialsService()
            {
                _repository = new BillOfMaterialsRepository();
            }

            public async Task<BomItemViewModel?> GetByIdAsync(int bomId)
            {
                var entity = await _repository.GetByIdAsync(bomId);
                return entity == null ? null : ToViewModel(entity);
            }

            public async Task<List<BomItemViewModel>> GetComponentsForProductAsync(
                int parentProductId,
                DateTime? asOfDate = null)
            {
                var entities = await _repository.GetComponentsForProductAsync(parentProductId, asOfDate);

                return entities
                    .Select(ToViewModel)
                    .ToList();
            }

            public async Task<List<BomItemViewModel>> GetUsageForComponentAsync(
                int componentId,
                DateTime? asOfDate = null)
            {
                var entities = await _repository.GetUsageForComponentAsync(componentId, asOfDate);

                return entities
                    .Select(ToViewModel)
                    .ToList();
            }

            public async Task<List<BomItemViewModel>> GetAllAsync()
            {
                var entities = await _repository.GetAllAsync();

                return entities
                    .Select(ToViewModel)
                    .ToList();
            }

            public async Task<int> SaveAsync(BomItemViewModel vm)
            {
                Validate(vm);

                var entity = ToEntity(vm);

                if (entity.BomId <= 0)
                {
                    int newId = await _repository.InsertAsync(entity);
                    vm.BomId = newId;
                    return newId;
                }

                bool updated = await _repository.UpdateAsync(entity);

                if (!updated)
                    throw new InvalidOperationException($"BOM item {entity.BomId} was not updated.");

                return entity.BomId;
            }

            public async Task<bool> DeleteAsync(BomItemViewModel vm)
            {
                if (vm.BomId <= 0)
                    return false;

                return await _repository.DeleteAsync(vm.BomId);
            }

            public async Task<bool> DeleteAsync(int bomId)
            {
                if (bomId <= 0)
                    return false;

                return await _repository.DeleteAsync(bomId);
            }

            private static BomItemViewModel ToViewModel(BillOfMaterialsEntity entity)
            {
                return new BomItemViewModel
                {
                    BomId = entity.BomId,
                    ParentProductId = entity.ParentProductId,
                    ComponentId = entity.ComponentId,
                    Quantity = entity.Quantity,
                    ScrapFactor = entity.ScrapFactor,
                    EffectiveDate = entity.EffectiveDate,
                    ExpiryDate = entity.ExpiryDate
                };
            }

            private static BillOfMaterialsEntity ToEntity(BomItemViewModel vm)
            {
                return new BillOfMaterialsEntity
                {
                    BomId = vm.BomId,
                    ParentProductId = vm.ParentProductId,
                    ComponentId = vm.ComponentId,
                    Quantity = vm.Quantity,
                    ScrapFactor = vm.ScrapFactor,
                    EffectiveDate = vm.EffectiveDate,
                    ExpiryDate = vm.ExpiryDate
                };
            }

            private static void Validate(BomItemViewModel vm)
            {
                if (vm.ParentProductId <= 0)
                    throw new InvalidOperationException("Parent product is required.");

                if (vm.ComponentId <= 0)
                    throw new InvalidOperationException("Component is required.");

                if (vm.ParentProductId == vm.ComponentId)
                    throw new InvalidOperationException("A product cannot be a component of itself.");

                if (vm.Quantity <= 0)
                    throw new InvalidOperationException("Quantity must be greater than zero.");

                if (vm.ScrapFactor < 0 || vm.ScrapFactor > 100)
                    throw new InvalidOperationException("Scrap factor must be between 0 and 100.");

                if (vm.ExpiryDate.HasValue && vm.ExpiryDate.Value.Date < vm.EffectiveDate.Date)
                    throw new InvalidOperationException("Expiry date cannot be before effective date.");
            }
        }
    }



