namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    /// <summary>Remove base and manual bank without changing pitch or heading.</summary>
    public void WingsLevel()
    {
        WingsLevel(TimeSpan.FromSeconds(.6));
    }

    public void WingsLevel(TimeSpan duration)
    {
        Dispatcher.VerifyAccess();
        SetAttitudeChannel(RollRotation, 0, duration);
    }
}
