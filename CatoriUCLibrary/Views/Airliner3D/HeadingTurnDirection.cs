using System.Text.Json.Serialization;

namespace CatoriUCLibrary.Views.Airliner3D;

/// <summary>Physical yaw direction viewed from above; independent of camera elevation.</summary>
[JsonConverter(typeof(JsonStringEnumConverter<HeadingTurnDirection>))]
public enum HeadingTurnDirection
{
    Shortest = 0,
    Clockwise = 1,
    Counterclockwise = 2
}
