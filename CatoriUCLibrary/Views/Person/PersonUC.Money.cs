namespace CatoriUCLibrary.Views;

public partial class PersonUC
{
    public string MoneyAccountId
    {
        get => AvatarMoney.GetAccountId(this);
        set => AvatarMoney.SetAccountId(this, value);
    }
    public decimal WorkAmount
    {
        get => AvatarMoney.GetWorkAmount(this);
        set => AvatarMoney.SetWorkAmount(this, value);
    }
    public decimal CurrentBalance => AvatarMoney.GetBalance(this);
    public bool TryPurchase(decimal amount, string transactionId, string description = "Purchase")
    {
        return AvatarMoney.TryPurchase(this, amount, transactionId, description);
    }
    public void CompleteWork(decimal amount, string completionId)
    {
        AvatarMoney.CreditWork(this, amount, completionId);
    }
}
