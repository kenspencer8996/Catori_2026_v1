using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects
{
    internal class MapPositionEntity
    {

        public int StreetTopSkia = 125;
        public int StreetBottomYSkia;
        public int LeftStreetInnerSkia;
        public int RightStreetOuterXSkia;
        public int RightStreetInnerXSkia;
        public int MidStreetTopRightSkia;
        public int MidStreetInnerXSkia;
        public int TeaStreetOuterXMaui
        {
            get
            {
                return RightStreetOuterXSkia;
            }
        }
        public int TeaStreetInnerXMaui
        {
            get
            {
                return RightStreetInnerXSkia;
            }
        }

        public int rightStreetouterSkia;
        public int MooStreetLeftXSkia;
        public int MidStreetToprightSkia;
        public int YouStreetStartSkia;
        public int YouStreetEndSkia;
        public int TeeaStreetStartSkia;
        public int TeaStreetEndSkia;
        public int yeaStreetEndSkia;
        public int MikStreetStartSkia;
        public int MikStreetEndSkia;
        public int YodelStreetStartSkia;
        public int YodelStreetEndSkia;
        public int MooooStreetStartSkia;
        public int MooooStreetEndSkia;
        public int TreeStreetStartSkia;
        public int TreeStreetEndSkia;
        public int BeeStreetStartSkia;
        public int MeeStreetEndSkia;
        public int MoneyStreetStartSkia;
        public int MoneyStreetEndSkia;
        public int YouStLength
        {
            get
            {
                return YouStreetEndSkia - YouStreetStartSkia;
            }
        }
        public int TeaStLength
        {
            get
            {
                return TeaStreetEndSkia - TeaStreetEndSkia;
            }
        }
        public int MooStLength
        {
            get
            {
                return MooooStreetEndSkia - MooooStreetStartSkia;
            }
        }
        public int YodelStLength
        {
            get
            {
                return YodelStreetEndSkia - YodelStreetStartSkia;
            }
        }
        public int TreeStLength
        {
            get
            {
                return TreeStreetEndSkia - TreeStreetStartSkia;
            }
        }
        public int MikStLength
        {
            get
            {
                return MikStreetEndSkia - MikStreetStartSkia;
            }
        }
        public int MooStreetLeftXMaui
        {
            get
            {
                return MooStreetLeftXSkia;
            }
        }

        public int MidStreetTopLeftMaui
        {
            get
            {
                return MooStreetLeftXSkia;
            }
        }
        public int YodelStreetInnerMaui
        {
            get
            {
                return LeftStreetInnerSkia;
            }
        }
        public int TeaStreetOuterMaui
        {
            get
            {
                return rightStreetouterSkia;
            }
        }
        public int MooStreetTopLeftMaui
        {
            get
            {
                return MooStreetLeftXSkia;
            }
        }
        public int MooStreetTopRightMaui
        {
            get
            {
                return MidStreetToprightSkia;
            }
        }
        public int YouStreetStartMaui
        {
            get
            {
                return YouStreetStartSkia;
            }
        }
        public int YouStreetEndMaui
        {
            get
            {
                return YouStreetEndSkia;
            }
        }
        public int TeaStreetStartMaui
        {
            get
            {
                return TeeaStreetStartSkia;
            }
        }
        public int TeaStreetEndMaui
        {
            get
            {
                return TeaStreetEndSkia;
            }
        }
        public int MikStreetStartMaui
        {
            get
            {
                return MikStreetStartSkia;
            }
        }
        public int MikStreetEndMaui
        {
            get
            {
                return MikStreetEndSkia;
            }
        }
        public int YodelStreetStartMaui
        {
            get
            {
                return YodelStreetStartSkia;
            }
        }
        public int YodelStreetEndMaui
        {
            get
            {
                return YodelStreetEndSkia;
            }
        }
        public int MooooStreetStartMau
        {
            get
            {
                return MooooStreetStartSkia;
            }
        }
        public int MooooStreetEndMaui
        {
            get
            {
                return MooooStreetEndSkia;
            }
        }
        public int TreeStreetStartMaui
        {
            get
            {
                return TreeStreetStartSkia;
            }
        }
        public int TreeStreetEndMaui
        {
            get
            {
                return TreeStreetEndSkia;
            }
        }
        public int BeeStreetStartMaui
        {
            get
            {
                return BeeStreetStartSkia;
            }
        }
        public int BeeStreetEndMaui
        {
            get
            {
                return BeeStreetEndMaui;
            }
        }
        public int MoneyStreetStartMaui
        {
            get
            {
                return MoneyStreetStartSkia;
            }
        }
        public int MoneyStreetEndMaui
        {
            get
            {
                return MooooStreetEndSkia;
            }
        }


        internal int MidStreetInnerXMaui
        {
            get
            {
                return MidStreetInnerXSkia;
            }
        }
        internal int StreetTopMaui
        {
            get
            {
                return StreetTopSkia;
            }
        }
        internal int StreetBottomMaui
        {
            get
            {
                return StreetBottomYSkia;
            }
        }

        internal int RightStreetInnerMaui
        {
            get
            {
                return RightStreetInnerXSkia;
            }
        }

        internal List<LotEntity> MapLocations = new List<LotEntity>();
       
    }
}
