namespace CatoriApp.Core.Objects.Messages
{
    public class PostOfficeInteriorhowMessage
    {
        PersonViewModel _model;
        public PostOfficeInteriorhowMessage( PersonViewModel model)
        {
             Model = model;
        }
        public PersonViewModel Model { get; set; }
    }
}


