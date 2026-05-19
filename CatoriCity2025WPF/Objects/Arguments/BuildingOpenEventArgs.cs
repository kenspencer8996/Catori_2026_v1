using CatoriApp.Views.Controls;
using CityAppServices.Objects.Entities;
using System.Windows.Controls;

namespace CatoriApp.Objects.Arguments
{
    public class BuildingOpenEventArgs : EventArgs
    {
        public HouseControl House { get; set; }
        public BankControl Business { get; set; }
        public BuildingTypeEnum BuldingType { get; set; }
        public BuildingOpenEventArgs(UserControl contentView, BuildingTypeEnum buldingType)
        {
            BuldingType = buldingType;
            switch (buldingType)
            {
                case BuildingTypeEnum.House:
                    House = (HouseControl)contentView;
                    break;
                case BuildingTypeEnum.Location:
                    Business = (BankControl)contentView;
                    break;
                case BuildingTypeEnum.Bank:
                    Business = (BankControl)contentView;
                    break;
                case BuildingTypeEnum.CarLot:
                    Business = (BankControl)contentView;
                    break;
                case BuildingTypeEnum.Retail:
                    Business = (BankControl)contentView;
                    break;
            }
        }
    }
}

