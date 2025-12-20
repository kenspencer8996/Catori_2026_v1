using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.Views.Controls;
using CatoriServices.Objects.Entities;
using System.Windows.Controls;
namespace CatoriCity2025WPF.ViewModels
{

    public class BankViewModel : ViewmodelBase
    {
        private string _businessKey = "";
        private string _imageFileName = "";
        private string _name = "";
        private string _description = "";
        public decimal _currentFunds { get; set; } = 0m;
        public double X { get; set; }
        public double Y { get; set; }
        public BankControl BankUC { get; set; }
        public string Bankkey 
        {
            get
            {
                return _businessKey;
            }
        }
        public int BankId { get; set; }

        public decimal CurrentFunds
        {
            get => _currentFunds;
            set
            {
                _currentFunds = value;
            }
        }   
        public string BusinesskeyImageNameWOExtension
        {
            get => _businessKey;
            set
            {
                _businessKey = value;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
            }
        }

        public string NameAsControlName
        {
            get
            {
                return _name?.Replace(" ", "") ?? "";
            }
        }

        public string ImageFileName
        {
            get
            {
                string filename = _imageFileName;
                if (string.IsNullOrEmpty(filename))
                    return filename;

                if (filename.StartsWith(GlobalStuff.ImageFolder))
                    filename = _imageFileName;
                else
                    filename = System.IO.Path.Combine(GlobalStuff.ImageFolder, _imageFileName);

                return filename;
            }
            set
            {
                _imageFileName = value;
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
            }
        }

        public Image BankImage
        {
            get
            {
                return UIUtility.GetImageControl(_imageFileName, 50, 50, 0);
            }
        }

        public BankEntity GetEntity()
        {
            var entity = new BankEntity();
            entity.BankId = BankId;
            entity.BusinesskeyImageNameWOExtension = BusinesskeyImageNameWOExtension;
            entity.ImageFileName = ImageFileName;
            entity.Description = Description;
            return entity;
        }

        public void ToModel(BankEntity entity)
        {
            if (entity == null) return;

            BankId = entity.BankId;
            BusinesskeyImageNameWOExtension = entity.BusinesskeyImageNameWOExtension;
            ImageFileName = entity.ImageFileName;
            Description = entity.Description;
            // Derive friendly name if not provided
            if (string.IsNullOrEmpty(entity.BusinesskeyImageNameWOExtension))
                Name = System.IO.Path.GetFileName(entity.ImageFileName);
            else
                Name = entity.BusinesskeyImageNameWOExtension;
        }
    }
}
