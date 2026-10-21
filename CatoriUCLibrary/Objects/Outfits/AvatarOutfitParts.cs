using CatoriUCLibrary.Views.Person;
using CatoriUCLibrary.Views;
namespace CatoriUCLibrary.Objects.Outfits;
public static class AvatarOutfitParts
{
    public static IReadOnlyList<string> Names { get; } = new[]
    {
        nameof(PersonPartType.Body),
        nameof(PersonPartType.LeftUpperArm),
        nameof(PersonPartType.RightUpperArm),
        nameof(PersonPartType.LeftForearm),
        nameof(PersonPartType.RightForearm),
        nameof(PersonPartType.LeftThigh),
        nameof(PersonPartType.RightThigh),
        nameof(PersonPartType.LeftLowerLeg),
        nameof(PersonPartType.RightLowerLeg)
    };
}
