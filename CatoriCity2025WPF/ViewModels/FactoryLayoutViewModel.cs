namespace CatoriCity2025WPF.ViewModels
{
    public class FactoryLayoutViewModel : ViewmodelBase
    {
        public long FactoryLayoutId { get; set; }
        public string FactoryInteriorName { get; set; } = "";
        public List<FactoryLayoutPointEntity> Points { get; set; } = new();
    }
}
