using System;
using System.Collections.Generic;
using System.Text;
using CatoriServices.Objects.Entities;
namespace CatoriCity2025WPF.ViewModels
{
    public class ShelfLocationViewModel: ViewmodelBase
    {
        private int _shelfLocationID;
        private string _storeType = string.Empty;
        private string _aisle = string.Empty;
        private string _shelf = string.Empty;
        private double _positionX;
        private double _positionY;
        private double _positionZ;
        private double _width;
        private double _height;
        private int _shopItemId;

        public ShelfLocationViewModel()
        {
        }

        public ShelfLocationViewModel(ShelfLocationEntity entity)
        {
            ToModel(entity);
        }

        public int ShelfLocationID
        {
            get => _shelfLocationID;
            set
            {
                if (_shelfLocationID != value)
                {
                    _shelfLocationID = value;
                    OnPropertyChanged(nameof(ShelfLocationID));
                }
            }
        }

        public string StoreType
        {
            get => _storeType;
            set
            {
                if (_storeType != value)
                {
                    _storeType = value;
                    OnPropertyChanged(nameof(StoreType));
                }
            }
        }

        public string Aisle
        {
            get => _aisle;
            set
            {
                if (_aisle != value)
                {
                    _aisle = value;
                    OnPropertyChanged(nameof(Aisle));
                }
            }
        }

        public string Shelf
        {
            get => _shelf;
            set
            {
                if (_shelf != value)
                {
                    _shelf = value;
                    OnPropertyChanged(nameof(Shelf));
                }
            }
        }

        public double PositionX
        {
            get => _positionX;
            set
            {
                if (_positionX != value)
                {
                    _positionX = value;
                    OnPropertyChanged(nameof(PositionX));
                }
            }
        }

        public double PositionY
        {
            get => _positionY;
            set
            {
                if (_positionY != value)
                {
                    _positionY = value;
                    OnPropertyChanged(nameof(PositionY));
                }
            }
        }

        public double PositionZ
        {
            get => _positionZ;
            set
            {
                if (_positionZ != value)
                {
                    _positionZ = value;
                    OnPropertyChanged(nameof(PositionZ));
                }
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged(nameof(Width));
                }
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                if (_height != value)
                {
                    _height = value;
                    OnPropertyChanged(nameof(Height));
                }
            }
        }

        public int ShopItemId
        {
            get => _shopItemId;
            set
            {
                if (_shopItemId != value)
                {
                    _shopItemId = value;
                    OnPropertyChanged(nameof(ShopItemId));
                }
            }
        }

        // Computed property for display purposes
        public string LocationDisplay => $"{Aisle} - {Shelf}";

        public ShelfLocationEntity GetEntity()
        {
            return new ShelfLocationEntity
            {
                ShelfLocationID = this.ShelfLocationID,
                StoreType = this.StoreType ?? string.Empty,
                Aisle = this.Aisle ?? string.Empty,
                Shelf = this.Shelf ?? string.Empty,
                PositionX = this.PositionX,
                PositionY = this.PositionY,
                PositionZ = this.PositionZ,
                Width = this.Width,
                Height = this.Height,
                ShopItemId = this.ShopItemId
            };
        }

        public void ToModel(ShelfLocationEntity entity)
        {
            if (entity == null) return;

            ShelfLocationID = entity.ShelfLocationID;
            StoreType = entity.StoreType ?? string.Empty;
            Aisle = entity.Aisle ?? string.Empty;
            Shelf = entity.Shelf ?? string.Empty;
            PositionX = entity.PositionX;
            PositionY = entity.PositionY;
            PositionZ = entity.PositionZ;
            Width = entity.Width;
            Height = entity.Height;
            ShopItemId = entity.ShopItemId;
        }
    }
}
