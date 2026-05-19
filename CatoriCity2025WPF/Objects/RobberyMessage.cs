using CatoriApp.Viewmodels;
using CatoriApp.ViewModels;

namespace CatoriApp.Objects
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

