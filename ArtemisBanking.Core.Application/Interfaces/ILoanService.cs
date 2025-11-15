using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.User;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface ILoanService : IGenericService<string, LoanDto>
{
    // Task<Result<List<LoanDto>>> GetByUserIdAsync(string userId, bool? isActive = null);
    // Task<Result<LoanDto>> AssignLoanAsync(AssignLoanDto dto);
    // Task<Result<bool>> HasActiveLoanAsync(string userId);


    Task<List<UserDto>> GetClientsWithOutLoans();
    Task<Result<List<ClientsWithDebtDto>>> GetClientsWithoutActiveLoan();
    Task<PaginatedData<LoanSummaryDto>> GetLoansPagedAsync(int page, int pageSize, string? identityCardNumber = null, bool? isCompleted = null);
    Task<Result<decimal>> GetAverageClientDebtAsync();
    //Task<Result> UpdateInterestRateAsync(int id, decimal newRate);
    //Task<Result<bool>> IsHighRiskClientAsync(string userId, decimal newLoanAmount);
}