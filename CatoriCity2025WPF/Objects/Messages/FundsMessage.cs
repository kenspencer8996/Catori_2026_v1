using CatoriApp.ViewModels;

namespace CatoriApp.Objects
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

