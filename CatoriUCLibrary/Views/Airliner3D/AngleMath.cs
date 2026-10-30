namespace CatoriUCLibrary.Views.Airliner3D;

public static class AngleMath
{
    public static double Delta(double from, double to)
    {
        double difference = (to - from) % 360;
        if (difference > 180)
        {
            difference -= 360;
        }
        if (difference < -180)
        {
            difference += 360;
        }
        return difference;
    }

    public static double Unwrap(double previous, double next)
    {
        return previous + Delta(previous, next);
    }
}
