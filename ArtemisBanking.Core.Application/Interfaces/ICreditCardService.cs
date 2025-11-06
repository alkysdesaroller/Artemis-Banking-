using ArtemisBanking.Core.Application.Dtos.CreditCard;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface ICreditCardService : IGenericService<int, CreditCardDto>
{
    Task<Result<List<CreditCardDto>>> GetByUserIdAsync(string userId, bool? isActive = null);
    Task<Result> UpdateLimitAsync(int id, decimal newLimit);
    Task<Result> CancelCardAsync(int id);
    Task<Result<string>> GenerateUniqueCardNumberAsync();
    Task<Result<string>> GenerateEncryptedCVCAsync(); 
    Task<Result<decimal>> GetAvailableCreditAsync(int creditCardId);
    Task<Result> ProcessPaymentAsync(int creditCardId, decimal amount, string accountNumber);
    Task<Result> ProcessCashAdvanceAsync(CashAdvanceDto dto);
    Task<Result<CreditCardDto>> GetByCardNumberAsync(string cardNumber);
    Task<Result<bool>> ValidateCardAsync(string cardNumber, string month, string year, string cvc);
    
    /*
    *Task<PagedResult<CreditCardDto>> GetPagedAsync(int page, int pageSize, string cedula = null, string status = null);
    */
}