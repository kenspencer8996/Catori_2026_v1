using CatoriApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
namespace CatoriApp.Controllers.House
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


