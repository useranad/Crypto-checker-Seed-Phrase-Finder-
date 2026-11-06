namespace CryptoChecker.Services;

/// <summary>
/// Demo stub. Real builds use public blockchain explorers / RPC providers.
/// </summary>
public sealed class DemoBalanceService : IBalanceService
{
    private static readonly string[] DemoAddresses =
    [
        "0xDemo000000000000000000000000000000000001",
        "bc1qdemobtcaddressxxxxxxxxxxxxxxxxxx",
        "TDemoTronAddressxxxxxxxxxxxxxxxxxxx"
    ];

    public Task<decimal> GetBalanceAsync(string network, string address, CancellationToken cancellationToken = default)
    {
        // Deterministic fake balance for UI preview only
        var hash = Math.Abs((network + address).GetHashCode());
        var value = (hash % 1000) / 100000m;
        return Task.FromResult(value);
    }

    public static string FakeAddress(string network, string phrase)
    {
        var seed = Math.Abs((network + phrase).GetHashCode());
        return network switch
        {
            "BTC" => $"bc1q{seed:x8}demobtcaddr",
            "SOL" => $"SoL{seed:x8}DemoAddress",
            "TRX" => $"T{seed:x8}DemoTronAddr",
            "XRP" => $"r{seed:x8}DemoXrpAddr",
            _ => DemoAddresses[seed % DemoAddresses.Length]
        };
    }
}
