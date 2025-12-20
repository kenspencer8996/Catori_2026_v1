using CityAppServices.Objects.Entities;

public class LogEntity
{
        private string _className = "";
        private string _methodName = ""; 
        private string _message = "";
        private decimal _runTime = 0;
        public int LoggingID { get; set; }
        public string ClassName  
        { 
            get
            {
                return _className;
            }
            set
            {
                _className = value;
            }
        }
        public string MethodName
        {
            get
            {
                return _methodName;
            }
            set
            {
                _methodName = value;
            }
        }
        internal string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }
        public decimal RunTime
        {
            get
            {
                return _runTime;
            }
            set
            {
                _runTime = value;
            }
        }


        public void Add(string className,decimal runTime, string methodName,
            string message)
        {
            _className = className;
            _message = message;
            _methodName = methodName;
            _runTime = runTime;
        }
    }
