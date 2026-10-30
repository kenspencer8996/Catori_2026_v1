namespace CatoriUCLibrary.Views.Airliner3D;

/// <summary>The host retains the actual avatar; the airplane remembers identity and role only.</summary>
public sealed record AirplaneOccupant(string Id, string Name, AirplaneOccupantRole Role);
