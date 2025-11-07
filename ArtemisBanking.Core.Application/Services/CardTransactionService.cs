using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class CardTransactionService : GenericServices<int, CardTransaction, CardTransactionDto>, ICardTransactionService 
{
    private readonly ICardTransactionRepository _cardTransactionRepository;
    public CardTransactionService(ICardTransactionRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _cardTransactionRepository = repository;
    }


}