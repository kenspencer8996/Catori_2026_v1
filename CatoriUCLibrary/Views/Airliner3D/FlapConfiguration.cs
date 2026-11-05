using System.IO;
using System.Text.Json;

namespace CatoriUCLibrary.Views.Airliner3D;

public enum FlapSetting
{
    NoFlap,
    HalfFlap,
    FullFlap,
}

public sealed record FlapConfiguration
{
    public required double HalfFlapDegrees { get; init; }
    public required double FullFlapDegrees { get; init; }

    public void Validate()
    {
        if (
            !double.IsFinite(HalfFlapDegrees)
            || !double.IsFinite(FullFlapDegrees)
            || HalfFlapDegrees <= 0
            || FullFlapDegrees <= HalfFlapDegrees
            || FullFlapDegrees > 85
        )
            throw new InvalidDataException(
                "Flap angles must satisfy 0 < HalfFlapDegrees < FullFlapDegrees <= 85."
            );
    }

    public static FlapConfiguration Load(string path)
    {
        try
        {
            var config =
                JsonSerializer.Deserialize<FlapConfiguration>(File.ReadAllText(path))
                ?? throw new InvalidDataException("Flap configuration cannot be null.");
            config.Validate();
            return config;
        }
        catch (Exception error)
            when (error is IOException or JsonException or UnauthorizedAccessException)
        {
            throw new InvalidDataException(
                $"Cannot load flap configuration '{path}': {error.Message}",
                error
            );
        }
    }
}
