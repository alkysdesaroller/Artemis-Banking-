using ArtemisBanking.Core.Application.Dtos.SavingAccount;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface ISavingAccountService : IGenericService<string, SavingAccountDto>
{ 
    //
    // Task<Result<List<SavingAccountDto>>> GetByUserIdAsync(string userId, bool? isActive = null, bool? isMainAccount = null);
    // Task<Result<bool>> HasSufficientBalanceAsync(string accountNumber, decimal amount);
    // Task<Result> UpdateBalanceAsync(string accountNumber, decimal amount, bool isCredit);
    // Task<Result> CancelAccountAsync(int id);
    // Task<Result<SavingAccountDto>> GetByAccountNumberAsync(string accountNumber);
    // Task<Result<SavingAccountDto>> GetMainAccountByUserIdAsync(string userId);
    // Task<Result<bool>> AccountNumberExistsAsync(string accountNumber);
    //
    
    /*
     *  Task<PagedResult<SavingAccountDto>> GetPagedAsync(int page, int pageSize, string cedula = null, string status = null, string type = null);
     */

}