namespace CityAppServices.Objects.Entities
{
    public class PersonEntity
    {
        public string ImagesFolder;
        private string _Name = "";
        private int _personId = 0;
        private decimal _currentPay;
        private PersonEnum _personRole;
        public decimal Funds;
        public bool Active { get; set; }
        public bool IsUser { get; set; }
        public bool Robber { get; set; }
        private string _fileNameOptional;
        public string FileNameOptional 
        { 
            get
            { return _fileNameOptional; }
            set
            {
                _fileNameOptional = value;
            }
        }
        public string Name
        { 
            get
                { return _Name; }
            set { _Name = value; }
        }

        public int PersonId
        {
            get
            {
                return _personId;
            }
            set
            {
                _personId = value;
            }
        }
    

        public PersonEnum PersonRole
        {
            get
            {
                return _personRole;
            }
            set
            {
                _personRole = value;
            }
        }
        public void Add(string name, decimal funds,
            PersonEnum role)
        {
            _Name = name;
            Funds = funds;
            _personRole = role;
        }
    }
}
