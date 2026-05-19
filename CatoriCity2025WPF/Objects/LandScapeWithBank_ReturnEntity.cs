using CatoriApp.Viewmodels;
using CatoriApp.ViewModels;

namespace CatoriApp.Objects
{
    public class LandScapeWithBank_ReturnEntity
    {
        public Stack<LandscapeObjectViewModel> LandscapeObjectsStack { get; set; }
        public LandscapeObjectViewModel financial { get; set; }
        public BankViewModel BankVM { get; set; } = new BankViewModel();

    }
}

