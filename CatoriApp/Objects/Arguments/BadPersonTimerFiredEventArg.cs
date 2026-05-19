namespace CatoriApp.Objects.Arguments
{
    public class BadPersonTimerFiredEventArg
    {
        public PersonViewModel Person { get; set; }
        public BadPersonTimerFiredEventArg(PersonViewModel person)
        {
            Person = person;
        }
    }
}

