using ArtemisBanking.Core.Application.Dtos.Loan;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface ILoanService : IGenericService<string, LoanDto>
{
    // Task<Result<List<LoanDto>>> GetByUserIdAsync(string userId, bool? isActive = null);
    // Task<Result> UpdateInterestRateAsync(int id, decimal newRate);
    // Task<Result<LoanDto>> AssignLoanAsync(AssignLoanDto dto);
    // Task<Result<bool>> IsHighRiskClientAsync(string userId, decimal newLoanAmount);
    // Task<Result<bool>> HasActiveLoanAsync(string userId);
    // Task<Result<LoanDto>> GetByLoanNumberAsync(string loanNumber);
    // Task<Result<string>> GenerateUniqueLoanNumberAsync();
    
    Task<Result<decimal>> GetAverageClientDebtAsync();
    
     Task<PaginatedData<LoanSummaryDto>> GetLoansPagedAsync(int page, int pageSize, string? identityCardNumber = null, bool? isCompleted = null);
     
}