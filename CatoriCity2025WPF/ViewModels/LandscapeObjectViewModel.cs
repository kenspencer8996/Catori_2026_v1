using CatoriCity2025WPF.Objects;
using CatoriServices.Objects.Entities;
using System.ComponentModel;

namespace CatoriCity2025WPF.ViewModels
{
    public class LandscapeObjectViewModel
    {
        public bool DeleteModel { get; set; } = false;  
        private LandscapeObjectEntity _entity = new LandscapeObjectEntity();
        public Stack<PathPositionModel> PathPositions { get; set; } = new Stack<PathPositionModel>();
        public void EntityToModel(LandscapeObjectEntity entity)
        {
            _entity = entity;
        }
        public LandscapeObjectEntity GetEntity()
        {
            return _entity;
        }
        public int LandScapeObjectID
        {
            get => _entity.LandScapeObjectID;
            set
            {
                if (_entity.LandScapeObjectID != value)
                {
                    _entity.LandScapeObjectID = value;
                    OnPropertyChanged(nameof(LandScapeObjectID));
                }
            }
        }

        public string Name
        {
            get => _entity.Name;
            set
            {
                if (_entity.Name != value)
                {
                    _entity.Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Description
        {
            get => _entity.Description;
            set
            {
                if (_entity.Description != value)
                {
                    _entity.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public double Height
        {
            get => _entity.Height;
            set
            {
                if (_entity.Height != value)
                {
                    _entity.Height = value;
                    OnPropertyChanged(nameof(Height));
                }
               
               
            }
        }

        public double Width
        {
            get => _entity.Width;
            set
            {
                if (_entity.Width != value)
                {
                    _entity.Width = value;
                    OnPropertyChanged(nameof(Width));
                }
                SetCenter();
            }
        }
        public double xCenter = 0;
        public double yCenter = 0;
        public void SetCenter()
        {
            if (Width > 0 && xActual > 0)
            {
                xCenter = xActual + (Width / 2);
            }
            if (Height > 0 && yActual > 0)
            {
                yCenter = yActual + (Height / 2);
            }
        }
        public double xActual
        {
            get => _entity.xActual;
            set
            {
                if (_entity.xActual != value)
                {
                    _entity.xActual = value;
                    OnPropertyChanged(nameof(xActual));
                }
                
                SetCenter();
            }
        }

        public double yActual
        {
            get => _entity.yActual;
            set
            {
                if (_entity.yActual != value)
                {
                    _entity.yActual = value;
                    OnPropertyChanged(nameof(yActual));
                }
                SetCenter();
            }
        }
        public string ImageName
        {
            get => _entity.ImageName;
            set
            {
                if (_entity.ImageName != value)
                {
                    _entity.ImageName = value;
                    OnPropertyChanged(nameof(ImageName));
                }
            }
        }

        public int GroupId
        {
            get => _entity.GroupId;
            set
            {
                if (_entity.GroupId != value)
                {
                    _entity.GroupId = value;
                    OnPropertyChanged(nameof(GroupId));
                }
            }
        }
        public bool HomeObject
        {
            get => _entity.HomeObject;
            set
            {
                if (_entity.HomeObject != value)
                {
                    _entity.HomeObject = value;
                    OnPropertyChanged(nameof(HomeObject));
                }
            }
        }
        public bool NextFromHomeObject
        {
            get => _entity.NextFromHomeObject;
            set
            {
                if (_entity.NextFromHomeObject != value)
                {
                    _entity.NextFromHomeObject = value;
                    OnPropertyChanged(nameof(NextFromHomeObject));
                }
            }
        }
        public string FeatureNote
        {
            get => _entity.FeatureNote;
            set
            {
                if (_entity.FeatureNote != value)
                {
                    _entity.FeatureNote = value;
                    OnPropertyChanged(nameof(FeatureNote));
                }
            }
        }
        public string NextLandscapeObjectName { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Optionally expose the underlying entity
        public LandscapeObjectEntity Entity => _entity;
    }
}

