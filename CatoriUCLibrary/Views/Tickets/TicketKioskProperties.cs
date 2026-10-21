using CatoriUCLibrary.Views;
using System.Globalization;
using CatoriUCLibrary.Objects;

namespace CatoriUCLibrary.Views.Tickets;

/// <summary>Versioned, culture-independent configuration for a ticket kiosk.</summary>
public static class TicketKioskProperties
{
    public static ControlProperties Create(string destination = "Beach", decimal cost = 0m)
    {
        var properties = new ControlProperties();
        properties.AddProperty("SchemaVersion", "1");
        properties.AddProperty("ControlType", "TicketKioskUC");
        properties.AddProperty("Destination", destination);
        properties.AddProperty("Cost", cost.ToString(CultureInfo.InvariantCulture));
        return Normalize(properties);
    }

    public static ControlProperties Normalize(ControlProperties properties)
    {
        ArgumentNullException.ThrowIfNull(properties);
        if (properties.Properties is null)
        {
            throw new ArgumentException("ControlProperties.Properties cannot be null.", nameof(properties));
        }
        var result = new ControlProperties
        {
            Properties = new Dictionary<string, string>(properties.Properties, StringComparer.Ordinal)
        };
        string? version = result.GetProperty("SchemaVersion");
        string? type = result.GetProperty("ControlType");
        if ((version is not null && version != "1") || (type is not null && type != "TicketKioskUC"))
        {
            throw new ArgumentException("Unsupported ticket kiosk configuration.", nameof(properties));
        }
        string destination = result.GetProperty("Destination") ?? "Beach";
        string costText = result.GetProperty("Cost") ?? "0";
        if (string.IsNullOrWhiteSpace(destination))
        {
            throw new ArgumentException("A ticket destination is required.", nameof(properties));
        }
        if (!decimal.TryParse(costText, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out decimal cost) || cost < 0)
        {
            throw new ArgumentException("Ticket cost must be a non-negative invariant decimal.", nameof(properties));
        }
        result.AddProperty("SchemaVersion", "1");
        result.AddProperty("ControlType", "TicketKioskUC");
        result.AddProperty("Destination", destination);
        result.AddProperty("Cost", cost.ToString(CultureInfo.InvariantCulture));
        return result;
    }

    public static decimal ReadCost(ControlProperties properties)
    {
        return decimal.Parse(properties.GetProperty("Cost")!, CultureInfo.InvariantCulture);
    }
}
