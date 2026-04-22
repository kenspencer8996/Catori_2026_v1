namespace CatoriCity2025WPF.Objects.Arguments
{
    public class TreasureStepArgs
    {
        public TreasureStepEnum TreasureStep { get; set; }
        public string Name { get; set; } = "";  
        public int StepNumber { get; set; } 
        public bool ClearList { get; set; } = false;
    }
}
