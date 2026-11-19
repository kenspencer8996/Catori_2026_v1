namespace CatoriUCLibrary.Views.Room3D;

public sealed class RoomDoorSettings
{
    public string Wall { get; set; } = "North";
    public double Width { get; set; } = 1;
    public double Height { get; set; } = 2.2;
    public bool IsOpen { get; set; }

    public void Validate(double roomWidth, double roomLength, double roomHeight)
    {
        if (Wall is not ("North" or "South" or "East" or "West"))
        {
            throw new ArgumentException("Choose North, South, East or West for the door wall.");
        }
        double span = Wall is "North" or "South" ? roomWidth : roomLength;
        if (!double.IsFinite(Width) || !double.IsFinite(Height) || Width < 0.2 || Height < 0.2 || Width > span - 0.2 || Height > roomHeight - 0.1)
        {
            throw new ArgumentException("The door must be at least 0.2 wide and high, with 0.1 clearance at each side and above it.");
        }
    }
}
