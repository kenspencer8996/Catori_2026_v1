namespace CatoriUCLibrary.Views.SpaceshipGame.GameObjects;

public readonly record struct ControllerReading(bool Connected, double X, double Y, bool Fire, ushort Buttons);

