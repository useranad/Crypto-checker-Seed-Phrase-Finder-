namespace CryptoChecker.Services;

public interface IBalanceService
{
    Task<decimal> GetBalanceAsync(string network, string address, CancellationToken cancellationToken = default);
}
