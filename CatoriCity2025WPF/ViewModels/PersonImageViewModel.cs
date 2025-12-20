using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.ViewModels
{
    public class PersonImageViewModel
    {
        private PersonImageTypeEnum _personImageType;
        public PersonImageStatusEnum PersonImageStatus { get; set; }

        private string _Name;

        private string _FilePath;

        private string _ImageType;
        private int _fkpersonId = 0;
        private PersonImageStatusEnum per;
        public int PersonImageID { get; set; }
        public int FKPersonId
        {
            get
            {
                return _fkpersonId;
            }
            set
            {
                _fkpersonId = value;
            }
        }
        public PersonImageTypeEnum PersonImageType
        {
            get
            {
                return _personImageType;
            }
            set
            {
                _personImageType = value;
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                _FilePath = value;
            }
        }
        public string ImageType
        {
            get
            {
                return _ImageType;
            }
            set
            {
                _ImageType = value;
            }
        }
        public void Add(string Name, PersonImageTypeEnum personImageType, PersonImageStatusEnum PersonImageStatus,
         string FilePath, string ImageType,
         int fkpersonId)
        {
            _personImageType = personImageType;
            this.PersonImageStatus = PersonImageStatus;
            _Name = Name;
            _FilePath = FilePath;
            _ImageType = ImageType;
            _fkpersonId = fkpersonId;
        }
        public PersonImageEntity GetEntity()
        {
            PersonImageEntity entity = new PersonImageEntity();
            entity.Name = _Name;
            entity.FKPersonId = _fkpersonId;
            entity.PersonImageID = PersonImageID;
            entity.PersonImageType = PersonImageType;
            entity.ImageType = ImageType;
            entity.FilePath = FilePath;
            return entity;
        }
        public void ToModel(PersonImageEntity entity)
        {
            FKPersonId = entity.FKPersonId;
            PersonImageID = entity.PersonImageID;
            PersonImageType = entity.PersonImageType;
            ImageType = entity.ImageType;
            FilePath = entity.FilePath;
            _Name = entity.Name;
        }
    }
}
