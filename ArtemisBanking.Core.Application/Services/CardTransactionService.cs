using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Helpers;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enums;
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
    private readonly ICreditCardService _creditCardService;
    private readonly  IMapper _mapper;
    public CardTransactionService(ICardTransactionRepository repository, IMapper mapper, ICreditCardRepository cardRepository, IAccountServiceForWebApp accountServiceForWebApp, ICreditCardService creditCardService) : base(repository, mapper)
    {
        _cardTransactionRepository = repository;
        _mapper = mapper;
        _cardRepository = cardRepository;
        _accountServiceForWebApp = accountServiceForWebApp;
        _creditCardService = creditCardService;
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

    public async Task<Result<CardTransactionDto>> ProcessCardTransactionAsync(ProcessCardTransactionDto dto, int commerceId)
    {
        try
        {
            // Validar que la tarjeta existe
            var creditCard = await _cardRepository.GetByCardNumberAsync(dto.CreditCardNumber);
            if (creditCard == null)
            {
                return Result<CardTransactionDto>.Fail("La tarjeta de crédito especificada no existe.");
            }

            if (!creditCard.IsActive)
            {
                return Result<CardTransactionDto>.Fail("La tarjeta de crédito especificada está inactiva.");
            }

            // Validar fecha de expiración
            var currentDate = DateTime.Now;
            if (creditCard.ExpirationYear < currentDate.Year || 
                (creditCard.ExpirationYear == currentDate.Year && creditCard.ExpirationMonth < currentDate.Month))
            {
                return Result<CardTransactionDto>.Fail("La tarjeta de crédito ha expirado.");
            }

            // Validar mes y año de expiración
            if (creditCard.ExpirationMonth != dto.ExpirationMonth || creditCard.ExpirationYear != dto.ExpirationYear)
            {
                return Result<CardTransactionDto>.Fail("La fecha de expiración proporcionada no coincide con la tarjeta.");
            }

            // Validar CVC (hashear el CVC proporcionado y comparar con el almacenado)
            var providedCvcHash = CvcEncryptation.ComputeSha256Hash(dto.Cvc);
            if (creditCard.CvcHashed != providedCvcHash)
            {
                return Result<CardTransactionDto>.Fail("El CVC proporcionado es incorrecto.");
            }

            // Validar que hay crédito disponible
            var availableCredit = creditCard.CreditLimit - creditCard.Balance;
            if (availableCredit < dto.Amount)
            {
                return Result<CardTransactionDto>.Fail("La tarjeta no tiene suficiente crédito disponible para esta transacción.");
            }

            // Actualizar el balance de la tarjeta (aumentar deuda)
            var newBalance = creditCard.Balance + dto.Amount;
            creditCard.Balance = newBalance;
            
            // Actualizar la tarjeta directamente en el repositorio
            await _cardRepository.UpdateAsync(creditCard.CardNumber, creditCard);

            // Crear la transacción de tarjeta
            var cardTransaction = new CardTransaction
            {
                CreditCardNumber = dto.CreditCardNumber,
                Amount = dto.Amount,
                CommerceId = commerceId,
                IsCashAdvance = dto.IsCashAdvance,
                Date = DateTime.Now,
                Status = CreditCardTransactionStatus.Approved
            };

            var savedTransaction = await _cardTransactionRepository.AddAsync(cardTransaction);
            var transactionDto = _mapper.Map<CardTransactionDto>(savedTransaction);

            return Result<CardTransactionDto>.Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return Result<CardTransactionDto>.Fail($"Error al procesar la transacción de tarjeta: {ex.Message}");
        }
    }
}