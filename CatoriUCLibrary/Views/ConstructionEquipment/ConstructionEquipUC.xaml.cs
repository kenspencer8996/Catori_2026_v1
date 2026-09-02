using System.Windows.Controls;
using CatoriUCLibrary.Views.ConstructionMachine;
using CatoriUCLibrary.Views.VisualParts;

namespace CatoriUCLibrary.Views.ConstructionEquipment;

public enum ConstructionEquipmentType { Bulldozer, Crane, Excavator, Loader, Custom }

public partial class ConstructionEquipUC : UserControl
{
    public ConstructionEquipUC() { InitializeComponent(); }
    public ConstructionEquipmentType EquipmentType { get; set; } = ConstructionEquipmentType.Custom;
    public VisualPartAnimatorUC Animator => Machine.Animator;
    public void ApplySettings(ConstructionMachineSettings settings) => Machine.ApplySettings(settings);
    public Task RaiseBoomAsync(TimeSpan duration,CancellationToken token=default) => Machine.Raise1(duration,token);
    public Task LowerBoomAsync(TimeSpan duration,CancellationToken token=default) => Machine.Lower1(duration,token);
    public Task RaiseArmAsync(TimeSpan duration,CancellationToken token=default) => Machine.Raise2(duration,token);
    public Task LowerArmAsync(TimeSpan duration,CancellationToken token=default) => Machine.Lower2(duration,token);
    public Task OpenAttachmentAsync(TimeSpan duration,CancellationToken token=default) => Machine.Raise3(duration,token);
    public Task CloseAttachmentAsync(TimeSpan duration,CancellationToken token=default) => Machine.Lower3(duration,token);
    public void Move(double x,double y) => Machine.Move(x,y);
    public void Reset() => Machine.Reset();
}
