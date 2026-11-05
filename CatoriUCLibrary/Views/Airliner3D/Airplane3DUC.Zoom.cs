namespace CatoriUCLibrary.Views.Airliner3D;

public partial class Airplane3DUC
{
    private double zoom = 1;
    public double Zoom
    {
        get => zoom;
        set
        {
            Dispatcher.VerifyAccess();
            if (!double.IsFinite(value) || value < 1 || value > 3)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Zoom must be between 1 and 3."
                );
            }
            zoom = value;
            UpdateCameraView();
        }
    }
}
