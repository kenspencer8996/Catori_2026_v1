using System.Windows;

namespace CatoriUCLibrary.Views;

public partial class AvatarMoneyWindow : Window
{
    public AvatarMoneyWindow()
    {
        InitializeComponent();
    }

    public void ShowAvatar(PersonUC? avatar)
    {
        DataContext = avatar;
        AvatarNameText.Text = avatar == null ? "No active avatar"
            : string.IsNullOrWhiteSpace(avatar.ToolTip as string) ? avatar.Name : (string)avatar.ToolTip;
    }
}
