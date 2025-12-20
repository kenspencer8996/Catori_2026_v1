using CatoriCity2025WPF.Viewmodels;
using CatoriCity2025WPF.ViewModels;

namespace CatoriCity2025WPF.Objects
{
    public class RobberyMessage //: ValueChangedMessage<RobberyMessageDetailViewModel>
    {
        public string RobberName;
        public BankViewModel Bank ;
        public RobberyMessage(string robberName, BankViewModel bank) //: base(value)
        {
            RobberName = robberName;
            Bank = bank;
        }
    }
}
