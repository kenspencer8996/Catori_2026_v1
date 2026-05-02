using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriCity2025WPF.ViewModels
{
    public class ComponentViewModel:ViewmodelBase
    {
        private int _componentId;
        private string _componentName = "";
        private decimal _quantity;

        public int ComponentId
        {
            get => _componentId;
            set => SetProperty(ref _componentId, value);
        }

        public string ComponentName
        {
            get => _componentName;
            set => SetProperty(ref _componentName, value);
        }

        public decimal Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }
    }
}
