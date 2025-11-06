using ArtemisBanking.Core.Application.Dtos.LoanInstallment;
using ArtemisBanking.Core.Domain.Entities;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface ILoanInstallmentService : IGenericService<int, LoanInstallmentDto>
{
    Task<Result<List<LoanInstallmentDto>>> GetByLoanIdAsync(int loanId);
    Task<Result> GenerateAmortizationTableAsync(int loanId, decimal amount, int durationMonths, decimal annualRate);
    Task<Result<decimal>> ProcessPaymentAsync(int loanId, decimal amount, string accountNumber);
    Task<Result> MarkOverdueInstallmentsAsync();
    Task<Result<decimal>> CalculateMonthlyPaymentAsync(decimal principal, decimal annualRate, int numberOfPayments);
    Task<Result<AmortizationTableDto>> GetAmortizationTableAsync(int loanId);
    Task<Result> RecalculateFutureInstallmentsAsync(int loanId, decimal newAnnualRate);
}