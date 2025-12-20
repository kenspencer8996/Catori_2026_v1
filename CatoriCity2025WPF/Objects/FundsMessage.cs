using CatoriCity2025WPF.ViewModels;

namespace CatoriCity2025WPF.Objects
{
    internal class FundsMessage
    {
        internal FundsViewModel Funds;
        internal FundsMessage(FundsViewModel funds)
        {
            Funds = funds;
        }
    }
}
