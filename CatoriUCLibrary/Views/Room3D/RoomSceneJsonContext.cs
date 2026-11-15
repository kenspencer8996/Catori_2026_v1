using System.Text.Json.Serialization;

namespace CatoriUCLibrary.Views.Room3D;

[JsonSerializable(typeof(RoomSceneDefinition))]
internal partial class RoomSceneJsonContext : JsonSerializerContext
{
}
