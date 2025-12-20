using CatoriCity2025WPF.Objects;
using CatoriCity2025WPF.ViewModels;
using CatoriCity2025WPF.Views.Controls;

namespace CatoriCity2025WPF.Viewmodels
{
    internal class RobberyMessageDetailViewModel: ViewmodelBase
    {
        private string _RobberName;
        private BadPersonControl _badGuy;
        private BusinessControlAndLocXYEntity _businessLocation;
        private BankControl _business;
        private FundsViewModel _funds;
        public string RobberName
        {
            get
            { return _RobberName; }
        }
        public FundsViewModel Funds
        {
            get { return _funds; }
            set { _funds = value; }
        }
        public BadPersonControl BadGuy
        {
            get
            { return _badGuy; }
        }
        public BusinessControlAndLocXYEntity BusinessLocation
        {
            get
            { return _businessLocation; }
        }
        public BankControl Business
        {
            get
            { return _business; }
        }

        internal RobberyMessageDetailViewModel(
            string RobberName, 
            BadPersonControl BadGuy,
            BusinessControlAndLocXYEntity BusinessLocation,
            BankControl business,
            FundsViewModel funds)
        {
            _business = business;
            this._RobberName = RobberName;
            if (this._RobberName != null && this._RobberName != "")
            {
                this._RobberName = RobberName.Trim();
            }
            this._badGuy = BadGuy;
            this._businessLocation = BusinessLocation;
            Funds = funds;
        }
    }
}
