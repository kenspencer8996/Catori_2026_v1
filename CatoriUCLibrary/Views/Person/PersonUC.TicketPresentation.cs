using System.Windows;

namespace CatoriUCLibrary.Views;

public partial class PersonUC
{
    public void HideHeldTicket()
    {
        HeldTicketViewbox.Visibility = Visibility.Collapsed;
        HeldTicketContentControl.Content = null;
    }
}
