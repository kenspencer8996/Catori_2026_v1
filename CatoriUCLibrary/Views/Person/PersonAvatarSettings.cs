using System.Text.Json;
using System.Text.Json.Serialization;

namespace CatoriUCLibrary.Views.Person;

public sealed class PersonAvatarSettings
{
    public string Name { get; set; } = "Default";
    public string Description { get; set; } = string.Empty;
    public int SchemaVersion { get; set; } = 3;
    public double DesignWidth { get; set; } = 300;
    public double DesignHeight { get; set; } = 400;
    public double ShoulderX { get; set; } = 165;
    public double ShoulderY { get; set; } = 135;
    public double ArmWidth { get; set; } = 155;
    public double ArmHeight { get; set; } = 55;
    public string ArmImagePath { get; set; } = "PersonArmStraight.png";
    public AvatarPartSettings Body { get; set; } = new() { Width=300,Height=400,ZIndex=10 };
    public AvatarPartSettings Head { get; set; } = new() { Width=140,Height=140,ZIndex=40 };
    public AvatarPartSettings LeftArm { get; set; } = new() { X=80,Y=135,Width=155,Height=55,PivotX=0,PivotY=.5,ZIndex=20 };
    public AvatarPartSettings RightArm { get; set; } = new() { X=165,Y=135,Width=155,Height=55,PivotX=0,PivotY=.5,ZIndex=30 };
    public AvatarPartSettings LeftForearm { get; set; } = new() { X=145,Y=0,Width=125,Height=45,PivotX=0,PivotY=.5,ZIndex=21 };
    public AvatarPartSettings RightForearm { get; set; } = new() { X=145,Y=0,Width=125,Height=45,PivotX=0,PivotY=.5,ZIndex=31 };
    public AvatarPartSettings LeftLeg { get; set; } = new() { X=95,Y=270,Width=55,Height=125,PivotX=.5,PivotY=0,ZIndex=5 };
    public AvatarPartSettings RightLeg { get; set; } = new() { X=155,Y=270,Width=55,Height=125,PivotX=.5,PivotY=0,ZIndex=6 };
    public AvatarPartSettings LeftLowerLeg { get; set; } = new() { X=0,Y=115,Width=50,Height=115,PivotX=.5,PivotY=0,ZIndex=4 };
    public AvatarPartSettings RightLowerLeg { get; set; } = new() { X=0,Y=115,Width=50,Height=115,PivotX=.5,PivotY=0,ZIndex=5 };
    public Dictionary<PersonActivity, PersonActivitySettings> Activities { get; set; } = new();

    public static PersonAvatarSettings CreateDefault() => new()
    {
        Activities = new Dictionary<PersonActivity, PersonActivitySettings>
        {
            [PersonActivity.Idle] = new()
            {
                FrameImagePaths = ["PersonBodyDefault.png"],
                ArmVisible = false,
                ArmAngle = 25
            },
            [PersonActivity.Walk] = new() { FrameDurationMilliseconds = 120, ArmVisible = false, ArmAngle = 15 },
            [PersonActivity.Sit] = new() { ArmVisible = false, ArmAngle = 45 },
            [PersonActivity.Dig] = new() { FrameDurationMilliseconds = 140, ArmVisible = false, ArmAngle = 65 },
            [PersonActivity.Work] = new() { ArmAngle = 0 }
        }
    };

    public static PersonAvatarSettings FromJson(string json) =>
        JsonSerializer.Deserialize<PersonAvatarSettings>(json, JsonOptions)
        ?? throw new JsonException("Person avatar settings were empty.");

    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };
}

public sealed class AvatarPartSettings
{
    public string ImagePath { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 100;
    public double Height { get; set; } = 100;
    public double PivotX { get; set; } = .5;
    public double PivotY { get; set; } = .5;
    public double InitialAngle { get; set; }
    public double Offset { get; set; }
    public int ZIndex { get; set; }
}

public sealed class PersonActivitySettings
{
    public List<string> FrameImagePaths { get; set; } = new();
    public int FrameDurationMilliseconds { get; set; } = 150;
    public bool Loop { get; set; }
    public bool ArmVisible { get; set; } = true;
    public double ArmAngle { get; set; }
}
