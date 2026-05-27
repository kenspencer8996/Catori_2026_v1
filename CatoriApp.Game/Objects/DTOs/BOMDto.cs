using System;
using System.Collections.Generic;
using System.Text;
namespace CatoriApp.Game.Objects.DTOs
{
    public class BOMDto
    {
  

        public static BomItemViewModel ToViewModel(BillOfMaterialsEntity entity)
        {
            return new BomItemViewModel
            {
                BomId = entity.BomId,
                ParentProductId = entity.ParentProductId,
                ComponentId = entity.ComponentId,
                //ComponentName = entity.ComponentName,
                //ComponentCode = entity.ComponentCode,
                Quantity = entity.Quantity,
                ScrapFactor = entity.ScrapFactor,
                EffectiveDate = entity.EffectiveDate,
                ExpiryDate = entity.ExpiryDate
            };
        }

  

        public static BillOfMaterialsEntity ToEntity(BomItemViewModel vm)
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
    }
}


