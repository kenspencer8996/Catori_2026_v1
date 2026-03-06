using System.Reflection;

namespace CatoriCity2025WPF.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        string _name;
        string _versionnumber;
        public AboutView()
        {
            InitializeComponent();
            var version = Assembly.GetExecutingAssembly()
                             .GetName()
                             .Version;

            var assembly = Assembly.GetExecutingAssembly();

            // Option 1: Read <Version> (Informational Version)
            var infoVersion = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            _name = Assembly.GetExecutingAssembly().GetName().Name ?? "CatoriCity2025WPF";
            _versionnumber = version.ToString();

            NameLabel.Content = _name;
            VersionNumberLabel.Content = _versionnumber;  
        }
    }
}
