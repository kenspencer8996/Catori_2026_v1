using CatoriCity2025WPF.Views.Controls;

namespace CatoriCity2025WPF.Objects.Arguments
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
