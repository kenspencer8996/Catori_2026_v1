namespace CatoriUCLibrary.Views.Airliner3D;
public partial class Airplane3DUC : CatoriInterfaces.IKeyframeAnimatable
{
    public void ExecuteKeyframe(string json)
    {
        Dispatcher.VerifyAccess();
        if (json is null)
        {
            throw new ArgumentNullException(nameof(json));
        }
        var keyframe = System.Text.Json.JsonSerializer.Deserialize<KeyframeActionsEntity>(json,
            new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            }) ?? throw new InvalidOperationException("Empty keyframe action JSON.");



        foreach (var action in keyframe.Actions)
        {
            double actionDuration = -1;
            if (action.Duration > 0)
                actionDuration = action.Duration;
            switch (action.Action)
            {
                case ActionEnum.BankLeft:
                    if (actionDuration > -1)
                        SetRoll(action.Angle);
                    else
                        SetRoll(action.Angle);
                    break;
                case ActionEnum.BankRight:
                    SetRoll(action.Angle);
                    break;
               case ActionEnum.InvertLeft:
                    if (actionDuration > -1)
                        SetRoll(180);
                    else
                        SetRoll(180);
                    break;
                case ActionEnum.InvertRight:
                    if (actionDuration > -1)
                        SetRoll(-180);
                    else
                        SetRoll(-180);

                    break;
                case ActionEnum.StartEngine1:
                    StartEngine(1);
                    break;
                case ActionEnum.StartEngine2:
                    StartEngine(2);
                    break;
                case ActionEnum.StopEngine1:
                    StopEngine(1);
                    break;
                case ActionEnum.StopEngine2:
                    StopEngine(2);
                    break;
                case ActionEnum.SetSpeed:
                    //sets(keyframe.Speed);   
                    //speed = keyframe.Speed;
                    break;
                case ActionEnum.NoseUp:
                    if (actionDuration > -1)
                        SetPitch(action.Angle);
                    else
                        SetPitch(action.Angle);
                    break;
                case ActionEnum.NoseDown:
                    if (actionDuration > -1)
                        SetPitch(action.Angle);
                    else
                        SetPitch(action.Angle);
                    break;
                case ActionEnum.GearUp:
                    if (actionDuration > -1)
                        _ = SetLandingGearAsync(false);
                    break;
                case ActionEnum.GearDown:
                    if (actionDuration > -1)
                        _ = SetLandingGearAsync(true);
                    break;
                case ActionEnum.FlapUp:
                    if (actionDuration > -1)
                        SetFlaps(FlapSetting.NoFlap);
                    else
                        SetFlaps(FlapSetting.NoFlap);
                    break;
                case ActionEnum.FlapHalf:
                    if (actionDuration > -1)
                        SetFlaps(FlapSetting.HalfFlap);
                    else
                        SetFlaps(FlapSetting.HalfFlap);
                    break;
                case ActionEnum.FlapDown:
                    if (actionDuration > -1)
                        SetFlaps(FlapSetting.FullFlap);
                    else
                        SetFlaps(FlapSetting.FullFlap);
                    break;
                case ActionEnum.SetHeading:
                    SetHeading(action.Angle, TimeSpan.FromSeconds(action.Duration));
                    break;
                default:
                    break;
                }
        }
        //keyframe.  
    }

    public void SetHeading(double degrees, TimeSpan duration)
    {
        Dispatcher.VerifyAccess();
        if (duration < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }
        ValidateAngle(degrees);
        double current = YawRotation.Angle;
        double target = AngleMath.Unwrap(current, SceneProjection.ToYaw(degrees));
        CancelKeyframeHeading();
        if (duration == TimeSpan.Zero)
        {
            YawRotation.Angle = target;
            return;
        }
        YawRotation.BeginAnimation(System.Windows.Media.Media3D.AxisAngleRotation3D.AngleProperty,
            new System.Windows.Media.Animation.DoubleAnimation(current, target, duration)
            {
                FillBehavior = System.Windows.Media.Animation.FillBehavior.HoldEnd
            }, System.Windows.Media.Animation.HandoffBehavior.SnapshotAndReplace);
    }

    private void CancelKeyframeHeading()
    {
        double current = YawRotation.Angle;
        YawRotation.BeginAnimation(System.Windows.Media.Media3D.AxisAngleRotation3D.AngleProperty, null);
        YawRotation.Angle = current;
    }
}
