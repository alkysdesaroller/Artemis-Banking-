using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services;

public class CardTransactionService : GenericServices<int, CardTransaction, CardTransactionDto>, ICardTransactionService 
{
    private readonly ICreditCardRepository _cardRepository;
    private readonly ICardTransactionRepository _cardTransactionRepository;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly  IMapper _mapper;
    public CardTransactionService(ICardTransactionRepository repository, IMapper mapper, ICreditCardRepository cardRepository, IAccountServiceForWebApp accountServiceForWebApp) : base(repository, mapper)
    {
        _cardTransactionRepository = repository;
        _mapper = mapper;
        _cardRepository = cardRepository;
        _accountServiceForWebApp = accountServiceForWebApp;
    }

    public async Task<Result<CreditCardDto>> GetByCreditCardIdAsync(string cardNumber)
    {
        var card = await _cardRepository.GetAllQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);

        if (card == null)
            return Result<CreditCardDto>.Fail("Card not found");

        var clientResult = await _accountServiceForWebApp.GetUserById(card.ClientId);

        if (clientResult.IsFailure)
            return Result<CreditCardDto>.Fail(clientResult.GeneralError!);
            


        var transactions = await _cardTransactionRepository.GetAllQueryable()
            .AsNoTracking()
            .Where(ct => ct.CreditCardNumber == cardNumber)
            .OrderByDescending(ct => ct.Date)
            .ToListAsync();

        var cardDto = _mapper.Map<CreditCardDto>(card);
        cardDto.CardTransactions = _mapper.Map<List<CardTransactionDto>>(transactions);
        cardDto.Client = clientResult.Value!;

        return Result<CreditCardDto>.Ok(cardDto);
    }


    public async Task<Result<List<CardTransactionDto>>> GetByCommerceIdAsync(int commerceId, int page, int pageSize)
    {
        var transactions = await _cardTransactionRepository.GetAllQueryable().AsNoTracking()
            .Where(ct => ct.CommerceId == commerceId)
            .OrderByDescending(ct => ct.Date)
            .Select(ct => _mapper.Map<CardTransactionDto>(ct))
            .ToListAsync();
        
        return Result<List<CardTransactionDto>>.Ok(transactions);
    }
}