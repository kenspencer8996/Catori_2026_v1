using CatoriApp.Views.Controls;

namespace CatoriApp.Objects.Arguments
{
    public class DeleteLandscapeArg: EventArgs
    {
        public LandscapeObjectControl _landscapeObjectControl { get; set; } = new LandscapeObjectControl();
        public DeleteLandscapeArg(LandscapeObjectControl LandscapeObjectControl) 
        {
            _landscapeObjectControl = LandscapeObjectControl;
        }
    }
}

