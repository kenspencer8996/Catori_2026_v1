using CatoriApp.Game.Objects.Travel;
using CatoriServices.Objects.Services.Finance;

namespace CatoriApp.Game.Views.Controls.Travel;

public sealed class AirTicketCounterControl : UserControl
{
    private readonly StackPanel _ticketPanel;
    private readonly TextBlock _status;
    private bool _purchaseInProgress;

    public AirTicketCounterControl(IEnumerable<TravelDestination> destinations)
    {
        ArgumentNullException.ThrowIfNull(destinations);

        _ticketPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
        _status = new TextBlock
        {
            Margin = new Thickness(8, 4, 8, 8),
            Foreground = Brushes.White,
            FontWeight = FontWeights.SemiBold,
            TextAlignment = TextAlignment.Center
        };

        var root = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(225, 18, 44, 73)),
            BorderBrush = Brushes.LightSkyBlue,
            BorderThickness = new Thickness(3),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(10),
            Child = new StackPanel
            {
                Children =
                {
                    new TextBlock
                    {
                        Text = "AIR TICKETS",
                        Foreground = Brushes.White,
                        FontSize = 24,
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(4)
                    },
                    _ticketPanel,
                    _status
                }
            }
        };

        Content = root;
        foreach (TravelDestination destination in destinations.Where(item => item.Mode == TravelMode.Air))
            _ticketPanel.Children.Add(CreateTicketButton(destination));
    }

    public event EventHandler<TravelTicket>? TicketPurchased;

    private Button CreateTicketButton(TravelDestination destination)
    {
        var button = new Button
        {
            Tag = destination,
            MinWidth = 190,
            Margin = new Thickness(7),
            Padding = new Thickness(12, 8, 12, 8),
            Cursor = Cursors.Hand,
            Content = new StackPanel
            {
                Children =
                {
                    new TextBlock { Text = $"{destination.Icon}  {destination.DisplayName}", FontSize = 20, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center },
                    new TextBlock { Text = destination.Price.ToString("C"), FontSize = 17, HorizontalAlignment = HorizontalAlignment.Center }
                }
            }
        };
        button.Click += BuyTicket_Click;
        return button;
    }

    private void BuyTicket_Click(object sender, RoutedEventArgs e)
    {
        if (_purchaseInProgress || sender is not Button { Tag: TravelDestination destination })
            return;

        if (GlobalGame.CurrentPerson is null)
        {
            _status.Text = "Choose an avatar before buying a ticket.";
            return;
        }

        _purchaseInProgress = true;
        SetButtonsEnabled(false);
        try
        {
            TravelTicket ticket = TravelService.Current.BuyTicket(GlobalGame.CurrentPerson.PersonId, destination);
            GlobalGame.CurrentPerson.Funds = FinanceService.FromCents(ticket.WalletBalanceCents);
            _status.Text = $"Ticket purchased. Your plane is set for {destination.DisplayName}.";
            TicketPurchased?.Invoke(this, ticket);
        }
        catch (InvalidOperationException exception)
        {
            _status.Text = exception.Message;
        }
        catch (Exception exception)
        {
            cLogger.Log($"Air ticket purchase failed for {destination.Key}: {exception}");
            _status.Text = "The ticket machine is unavailable. No ticket was issued.";
        }
        finally
        {
            _purchaseInProgress = false;
            SetButtonsEnabled(true);
        }
    }

    private void SetButtonsEnabled(bool isEnabled)
    {
        foreach (Button button in _ticketPanel.Children.OfType<Button>())
            button.IsEnabled = isEnabled;
    }
}
