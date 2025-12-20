using CatoriCity2025WPF.Views.Controls;
using CityAppServices.Objects.Entities;
using System.Windows.Controls;

namespace CatoriCity2025WPF.Objects.Arguments
{
    public class BuildingOpenEventArgs : EventArgs
    {
        public HouseControl House { get; set; }
        public BankControl Business { get; set; }
        public BuldingTypeEnum BuldingType { get; set; }
        public BuildingOpenEventArgs(UserControl contentView, BuldingTypeEnum buldingType)
        {
            BuldingType = buldingType;
            switch (buldingType)
            {
                case BuldingTypeEnum.House:
                    House = (HouseControl)contentView;
                    break;
                case BuldingTypeEnum.Factory:
                    Business = (BankControl)contentView;
                    break;
                case BuldingTypeEnum.Bank:
                    Business = (BankControl)contentView;
                    break;
                case BuldingTypeEnum.CarLot:
                    Business = (BankControl)contentView;
                    break;
                case BuldingTypeEnum.Retail:
                    Business = (BankControl)contentView;
                    break;
            }
        }
    }
}
