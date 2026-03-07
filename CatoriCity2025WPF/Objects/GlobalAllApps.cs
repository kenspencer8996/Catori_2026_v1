namespace CatoriCity2025WPF.Objects
{
    internal class GlobalAllApps
    {
        public static PersonViewModel CurrentPerson { get; set; }
        public static List<PersonViewModel> AllPersons { get; set; } = new List<PersonViewModel>();

    }
}
