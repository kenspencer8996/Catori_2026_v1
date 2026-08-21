namespace CatoriUCLibrary.Views.Person;

public sealed class PersonActivityCompletedEventArgs(PersonActivity activity) : EventArgs
{
    public PersonActivity Activity { get; } = activity;
}
