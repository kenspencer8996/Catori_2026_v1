using CatoriUCLibrary.Views.Tickets;
using System.Windows;
using CatoriUCLibrary.Objects;

namespace CatoriUCLibrary.Views;

public partial class TicketKioskUC
{
    /// <summary>
    /// Gets a detached configuration snapshot. Assign an edited snapshot to apply it.
    /// Destination and Cost remain WPF binding adapters for this configuration.
    /// </summary>
    public override ControlProperties ControlProperties
    {
        get => TicketKioskProperties.Normalize(base.ControlProperties);
        set => base.ControlProperties = TicketKioskProperties.Normalize(value);
    }

    protected override void ApplyProperties(ControlProperties properties)
    {
        var normalized = TicketKioskProperties.Normalize(properties);
        SetCurrentValue(DestinationProperty, normalized.GetProperty("Destination"));
        SetCurrentValue(CostProperty, TicketKioskProperties.ReadCost(normalized));
    }

    private static void OnConfigurationPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var kiosk = (TicketKioskUC)sender;
        kiosk.OnConfigurationPropertyChanged(properties =>
        {
            properties.AddProperty("Destination", kiosk.Destination);
            properties.AddProperty("Cost", kiosk.Cost.ToString(System.Globalization.CultureInfo.InvariantCulture));
        });
    }
}
