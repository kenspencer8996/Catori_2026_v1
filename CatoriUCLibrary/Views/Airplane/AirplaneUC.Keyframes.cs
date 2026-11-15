using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CatoriUCLibrary.Views.Airliner3D;

namespace CatoriUCLibrary.Views;

public partial class AirplaneUC : CatoriInterfaces.IKeyframeAnimatable
{
    public void ExecuteKeyframe(string json)
    {
        Dispatcher.VerifyAccess();
        var payload = JsonSerializer.Deserialize<KeyframeActionsEntity>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Empty airplane keyframe action JSON.");
        foreach (var action in payload.Actions)
        {
            if (action.Action == ActionEnum.Disembark)
            {
                _ = DisembarkFromKeyframeAsync();
            }
        }
    }

    private async Task DisembarkFromKeyframeAsync()
    {
        try
        {
            DependencyObject? parent = VisualTreeHelper.GetParent(this);
            while (parent is not null && parent is not Canvas)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if (parent is not Canvas scene)
            {
                throw new InvalidOperationException("The airplane must be on a scene to disembark its passengers and crew.");
            }
            await ParkAndDisembarkAsync(scene);
        }
        catch (Exception exception)
        {
            CatoriShared.Diagnostics.GameSafetyLog.Error("Keyframe", "Airplane disembarking failed.", exception);
        }
    }
}
