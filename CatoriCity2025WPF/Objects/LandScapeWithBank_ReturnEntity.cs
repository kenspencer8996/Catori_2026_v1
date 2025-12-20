using CatoriCity2025WPF.Viewmodels;
using CatoriCity2025WPF.ViewModels;

namespace CatoriCity2025WPF.Objects
{
    public class LandScapeWithBank_ReturnEntity
    {
        public Stack<LandscapeObjectViewModel> LandscapeObjectsStack { get; set; }
        public LandscapeObjectViewModel financial { get; set; }
        public BankViewModel BankVM { get; set; } = new BankViewModel();

    }
}
