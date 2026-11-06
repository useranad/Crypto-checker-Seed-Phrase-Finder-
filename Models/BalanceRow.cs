namespace CryptoChecker.Models;

public sealed class BalanceRow
{
    public string Phrase { get; set; } = string.Empty;
    public string Network { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Balance { get; set; } = "0";
}
