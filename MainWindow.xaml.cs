using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CryptoChecker.Models;
using CryptoChecker.Services;

namespace CryptoChecker;

public partial class MainWindow : Window
{
    private readonly IBalanceService _balanceService = new DemoBalanceService();
    private readonly ObservableCollection<BalanceRow> _rows = [];
    private CancellationTokenSource? _cts;

    public MainWindow()
    {
        InitializeComponent();
        ResultsGrid.ItemsSource = _rows;
    }

    private async void CheckBalances_Click(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _rows.Clear();
        var phrases = PhraseNormalizer.SplitLines(InputBox.Text).ToList();
        if (phrases.Count == 0)
        {
            StatusText.Text = "Nothing to check";
            return;
        }

        var networks = GetSelectedNetworks();
        if (networks.Count == 0)
        {
            StatusText.Text = "Select at least one network";
            return;
        }

        StatusText.Text = "Checking...";
        var withBalance = 0;
        var processed = 0;

        try
        {
            foreach (var phrase in phrases)
            {
                token.ThrowIfCancellationRequested();
                processed++;

                foreach (var network in networks)
                {
                    var address = DemoBalanceService.FakeAddress(network, phrase);
                    var balance = await _balanceService.GetBalanceAsync(network, address, token);

                    _rows.Add(new BalanceRow
                    {
                        Phrase = Truncate(phrase, 42),
                        Network = network,
                        Address = address,
                        Balance = balance.ToString("0.00000000")
                    });

                    if (balance > 0)
                        withBalance++;
                }

                StatsText.Text = $"Processed: {processed} | With balance: {withBalance}";
            }

            StatusText.Text = "Done (demo balances)";
        }
        catch (OperationCanceledException)
        {
            StatusText.Text = "Cancelled";
        }
        catch (Exception ex)
        {
            StatusText.Text = $"Error: {ex.Message}";
        }
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        InputBox.Clear();
        _rows.Clear();
        StatsText.Text = "Processed: 0 | With balance: 0";
        StatusText.Text = "Ready";
    }

    private List<string> GetSelectedNetworks()
    {
        var map = new (CheckBox Box, string Name)[]
        {
            (CheckBtc, "BTC"),
            (CheckEth, "ETH"),
            (CheckBsc, "BSC"),
            (CheckLtc, "LTC"),
            (CheckSol, "SOL"),
            (CheckPolygon, "POLYGON"),
            (CheckAvalanche, "AVALANCHE"),
            (CheckArbitrum, "ARBITRUM"),
            (CheckBase, "BASE"),
            (CheckOptimism, "OPTIMISM"),
            (CheckGnosis, "GNOSIS"),
            (CheckUnichain, "UNICHAIN"),
            (CheckBerachain, "BERACHAIN"),
            (CheckXrp, "XRP"),
            (CheckTrx, "TRX"),
        };

        return map.Where(x => x.Box.IsChecked == true).Select(x => x.Name).ToList();
    }

    private static string Truncate(string value, int max)
        => value.Length <= max ? value : value[..max] + "...";
}
