using System.Text.Json;

namespace CatoriUCLibrary.Views.Room3D;

public static class RoomSceneCodec
{
    public static RoomSceneDefinition Read(string json)
    {
        return string.IsNullOrWhiteSpace(json) ? new RoomSceneDefinition() : JsonSerializer.Deserialize(json, RoomSceneJsonContext.Default.RoomSceneDefinition) ?? throw new ArgumentException("Invalid 3D scene definition.");
    }

    public static string Write(RoomSceneDefinition definition)
    {
        return JsonSerializer.Serialize(definition, RoomSceneJsonContext.Default.RoomSceneDefinition);
    }
}
