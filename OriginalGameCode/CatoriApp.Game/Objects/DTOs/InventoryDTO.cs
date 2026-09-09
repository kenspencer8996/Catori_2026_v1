using System;
using System.Collections.Generic;
using System.Text;
namespace CatoriApp.Game.Objects.DTOs
{
    public class InventoryDTO
    {

        public static InventoryItemViewModel ToViewModel(InventoryEntity entity)
        {
            return new InventoryItemViewModel
            {
                InventoryId = entity.InventoryId,
                ProductId = entity.ProductId,
                ProductName = entity.ProductName,
                Location = entity.Location,
                QuantityOnHand = entity.QuantityOnHand,
                LastUpdated = entity.LastUpdated
            };
        }

      
        public static InventoryEntity ToEntity(InventoryItemViewModel vm)
        {
            return new InventoryEntity
            {
                InventoryId = vm.InventoryId,
                ProductId = vm.ProductId,
                Location = vm.Location,
                QuantityOnHand = vm.QuantityOnHand,
                LastUpdated = vm.LastUpdated
            };
        }
    }
}


