using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CatoriUCLibrary.Views;

public partial class PersonUC : CatoriInterfaces.IKeyframeAnimatable
{
    public double KeyframeScale => AvatarKeyframeScale.ScaleX;

    public void SetScale(double scale, TimeSpan duration)
    {
        Dispatcher.VerifyAccess();
        if (!double.IsFinite(scale) || scale <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be finite and greater than zero.");
        }
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        foreach (var property in new[] { ScaleTransform.ScaleXProperty, ScaleTransform.ScaleYProperty })
        {
            double current = (double)AvatarKeyframeScale.GetValue(property);
            AvatarKeyframeScale.BeginAnimation(property, null);
            AvatarKeyframeScale.SetValue(property, scale);
            if (duration > TimeSpan.Zero)
            {
                AvatarKeyframeScale.BeginAnimation(property, new DoubleAnimation(current, scale, duration)
                {
                    FillBehavior = FillBehavior.Stop
                }, HandoffBehavior.SnapshotAndReplace);
            }
        }
    }

    public void ExecuteKeyframe(string json)
    {
        Dispatcher.VerifyAccess();
        var payload = JsonSerializer.Deserialize<AvatarKeyframePayload>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Empty avatar keyframe JSON.");
        if (payload.Actions is null || payload.Actions.Any(action => action is null || action.Action != "SetScale"
            || !double.IsFinite(action.Scale) || action.Scale <= 0 || !double.IsFinite(action.Duration) || action.Duration < 0))
        {
            throw new InvalidOperationException("Avatar keyframes support SetScale with a positive scale and a non-negative duration.");
        }
        foreach (var action in payload.Actions)
        {
            SetScale(action.Scale, TimeSpan.FromSeconds(action.Duration));
        }
    }

    private sealed class AvatarKeyframePayload
    {
        public List<AvatarScaleAction>? Actions { get; set; } = new();
    }

    private sealed class AvatarScaleAction
    {
        public string Action { get; set; } = "";
        public double Scale { get; set; } = 1;
        public double Duration { get; set; }
    }
}
