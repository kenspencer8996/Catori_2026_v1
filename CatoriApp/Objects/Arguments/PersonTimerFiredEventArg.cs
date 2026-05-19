namespace CatoriApp.Objects.Arguments
{
    public class PersonTimerFiredEventArg
    {
        public PersonViewModel Person { get; set; }
        public PersonTimerFiredEventArg(PersonViewModel person)
        {
            Person = person;
        }
    }
}

