using System.ComponentModel;
using System.Runtime.CompilerServices;

public class FactoryLayoutViewModel : INotifyPropertyChanged
{
    private long _factoryLayoutId;
    private long _factoryId;
    private string _name = string.Empty;
    private string _description = string.Empty;

    public long FactoryLayoutId
    {
        get => _factoryLayoutId;
        set { _factoryLayoutId = value; OnPropertyChanged(); }
    }

    public long FactoryId
    {
        get => _factoryId;
        set { _factoryId = value; OnPropertyChanged(); }
    }

    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    public List<FactoryLayoutPointEntity> Points { get; set; } = new List<FactoryLayoutPointEntity>();

    public void ToViewModel(FactoryLayoutEntity e)
    {

        FactoryLayoutId = e.FactoryLayoutId;
        FactoryId = e.FactoryId;
        Name = e.Name;
        Description = e.Description;
        Points = e.Points;
    }
    public List<FactoryAssemblyRobotEntity> AssemblyRobots { get; set; }
    public FactoryLayoutEntity ToEntity() =>
        new FactoryLayoutEntity
        {
            FactoryLayoutId = FactoryLayoutId,
            FactoryId = FactoryId,
            Name = Name,
            Description = Description,
            Points = Points
        };
}
