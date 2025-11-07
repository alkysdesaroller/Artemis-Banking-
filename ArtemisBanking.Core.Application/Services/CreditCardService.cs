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


}