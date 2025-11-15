using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.Commerce;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class CreditCardService : GenericServices<string, CreditCard, CreditCardDto>, ICreditCardService 
{
    private readonly ICreditCardRepository _creditCardRepository;
    public CreditCardService(ICreditCardRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _creditCardRepository = repository;
    }


    public Task<Result<decimal>> GetAvailableCreditAsync(int creditCardId)
    {
        throw new NotImplementedException();
    }

    public Task<Result> UpdateLimitAsync(string creditCardNumber, decimal newLimit)
    {
        throw new NotImplementedException();
    }

    public Task<Result> CancelCardAsync(string creditCardNumber)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ProcessCashAdvanceAsync(CashAdvanceDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> GenerateEncryptedCVCAsync()
    {
        throw new NotImplementedException();
    }
}