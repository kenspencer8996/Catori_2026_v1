using System;
using System.Collections.Generic;
using System.Text;
namespace CatoriApp.Game.Objects.DTOs
{
    public class ProductDTO
    {
        public static ProductViewModel ToViewModel(ProductEntity entity)
        {
            return new ProductViewModel
            {
                ProductId = entity.ProductId,
                ProductName = entity.ProductName,
                ProductCode = entity.ProductCode,
                ProductType = entity.ProductType,
                UnitOfMeasure = entity.UnitOfMeasure,
                CostPerUnit = entity.CostPerUnit,
                CreatedAt = entity.CreatedAt
            };
        }

      

        public static ProductEntity ToEntity(ProductViewModel vm)
        {
            return new ProductEntity
            {
                ProductId = vm.ProductId,
                ProductName = vm.ProductName,
                ProductCode = vm.ProductCode,
                ProductType = vm.ProductType,
                UnitOfMeasure = vm.UnitOfMeasure,
                CostPerUnit = vm.CostPerUnit,
                CreatedAt = vm.CreatedAt
            };
        }
    }
}


