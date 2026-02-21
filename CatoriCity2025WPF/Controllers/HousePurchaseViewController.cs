using CatoriCity2025WPF.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriCity2025WPF.Controllers
{
    public class HousePurchaseViewController
    {
        HousePurchaseView _view;
        HouseViewModel _model;
        public HousePurchaseViewController(HousePurchaseView view, HouseViewModel model)
        {
            _view = view;
            _model = model;
        }   
    }
}
