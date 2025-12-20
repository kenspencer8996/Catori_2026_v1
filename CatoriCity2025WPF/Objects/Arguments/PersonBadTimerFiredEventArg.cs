namespace CatoriCity2025WPF.Objects.Arguments
{
    public class PersonBadTimerFiredEventArg
    {
        public PersonViewModel Person { get; set; }
        public PersonBadTimerFiredEventArg(PersonViewModel person)
        {
            Person = person;
        }
    }
}
