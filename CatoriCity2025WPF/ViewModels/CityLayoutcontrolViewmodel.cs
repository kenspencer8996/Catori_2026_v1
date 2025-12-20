using System.Windows.Input;

namespace CatoriCity2025WPF.ViewModels
{
    internal class CityLayoutcontrolViewmodel : ViewmodelBase
    {
        public ICommand? NavigateCommand { get; private set; }

        private double _cityRoadWidth = 100;
        private double _pageWidth;
        private double _pageHeight;
        private double _cityRoadHorizontalLength;
        private double _cityRoadFromEdge = 150;
        private double _highwayWidth = 150;

        internal double CityRoadWidth
        {
            get
            {
                return _cityRoadWidth;
            }
            set
            {
                _cityRoadWidth = value;
                OnPropertyChanged("CityRoadWidth");
            }
        }
        internal double PageWidth
        {
            get
            {
                return _pageWidth;
            }
            set
            {
                _pageWidth = value;
                OnPropertyChanged("PageWidth");
            }
        }
        internal double PageHeight
        {
            get
            {
                return _pageHeight;
            }
            set
            {
                _pageHeight = value;
                OnPropertyChanged("PageHeight");
            }
        }

        internal double CityRoadHorizontalLength
        {
            get
            {
                return _cityRoadHorizontalLength;
            }
            set
            {
                _cityRoadHorizontalLength = value;
                OnPropertyChanged("CityRoadHorizontalLength");
            }
        }
        internal double CityRoadFromEdge
        {
            get
            {
                return _cityRoadFromEdge;
            }
            set
            {
                _cityRoadFromEdge = value;
                OnPropertyChanged("CityRoadFromEdge");
            }
        }
        internal double HighwayWidth
        {
            get
            {
                return _highwayWidth;
            }
            set
            {
                _highwayWidth = value;
                OnPropertyChanged("HighwayWidth");
            }
        }
        internal void HandleSizeChange(double width, double height)
        {
            _highwayWidth = 100;
            _pageWidth = width;
            _pageHeight = height;
            //take 20 off for corners
            // CityRoadHorizontalLength = width - (2 * _cityRoadFromEdge)-20 ;
        }
    }
}
