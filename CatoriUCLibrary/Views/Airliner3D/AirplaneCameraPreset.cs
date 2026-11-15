using System.Text.Json.Serialization;

namespace CatoriUCLibrary.Views.Airliner3D;



[JsonConverter(typeof(JsonStringEnumConverter<AirplaneCameraPreset>))]
public enum AirplaneCameraPreset

{

    Side,

    Overhead,

    ThreeQuarter,

    Custom

}

