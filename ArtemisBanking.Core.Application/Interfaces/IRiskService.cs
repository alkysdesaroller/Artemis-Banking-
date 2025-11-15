namespace ArtemisBanking.Core.Application.Interfaces;

public interface IRiskService
{
    Task<decimal> GetSystemAverageClientDebtAsync();
    Task<decimal> CalculateClientTotalDebt(string userId);
    Task<decimal> CalculateClientLoanDebtAsync(string clientId);
    Task<decimal> CalculateClientCreditCardDebtAsync(string clientId);
    decimal CalculateTotalLoanDebt(decimal capital, decimal anualRate, int termMonths);
}