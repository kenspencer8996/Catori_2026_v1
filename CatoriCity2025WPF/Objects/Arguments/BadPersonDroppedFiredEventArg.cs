namespace CatoriCity2025WPF.Objects.Arguments
{
    public class BadPersonDroppedFiredEventArg
    {
        private PersonViewModel _badguy;

        public BadPersonDroppedFiredEventArg(PersonViewModel badguy)
        {
            _badguy = badguy;
        }
        public PersonViewModel Badguy
        { get { return _badguy; } }
    }
}
