using CatoriCity2025WPF.Viewmodels;

namespace CatoriCity2025WPF.Objects.Arguments
{

    public class PersonMoveFiredEventArg
    {
        public bool MoveNext = false;
        public double _x;
        public double _y;
        public PersonViewModel Person;
        public PersonMoveFiredEventArg(double x, double y, PersonViewModel person)
        {
            _x = x;
            _y = y;
            Person = person;
        }
       
    }
}
