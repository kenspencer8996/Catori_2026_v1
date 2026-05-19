namespace CatoriApp.Objects.Messages
{
    internal class PostOfficeInteriorhowMessage
    {
        PersonViewModel _model;
        public PostOfficeInteriorhowMessage( PersonViewModel model)
        {
             Model = model;
        }
        public PersonViewModel Model { get; internal set; }
    }
}


