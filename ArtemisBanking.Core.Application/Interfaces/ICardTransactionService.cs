using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface ICardTransactionService : IGenericService<int, CardTransactionDto>
{
    //Task<Result<List<CardTransactionDto>>> GetByCreditCardIdAsync(string cardNumber);
    //Task<Result<List<CardTransactionDto>>> GetByCommerceIdAsync(int commerceId, int page, int pageSize);
}