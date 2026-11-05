namespace CatoriShared.Keyframes;
public sealed class Keyframes
{
    public List<SceneKeyframeEntity> Frames { get; set; } = new();
}
public sealed class SceneKeyframeEntity
{
    public string PathName { get; set; } = "";
    public double KeyframeDistance { get; set; }
    public List<KeyframeAction> Actions { get; set; } = new();
}
public sealed class KeyframeAction
{
    public string ControlName { get; set; } = "";
    public string ActionJson { get; set; } = "";
}
