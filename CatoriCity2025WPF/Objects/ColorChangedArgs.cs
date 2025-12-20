namespace CatoriCity2025WPF.Objects
{
    public class ColorChangedArgs
    {
        private string _propertyName;
        private string _color;
        public ColorChangedArgs(string propertybame, string color) 
        {
            _propertyName = propertybame;
            _color = color;
        }
        public string PropertyName
        {
            get { return _propertyName; }
        }
        public string ColorName
        {
            get { return _color; }
        }
    }
}
