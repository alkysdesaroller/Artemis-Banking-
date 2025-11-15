using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.LoanInstallment;
using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Dtos.Transaction.Teller;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ISavingAccountRepository accountRepository,
    ICreditCardRepository creditCardRepository,
    ILoanRepository loanRepository,
    ISavingAccountService savingAccountService,
    IMapper mapper)
    : GenericServices<int, Transaction, TransactionDto>(transactionRepository, mapper), ITransactionService
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly ISavingAccountRepository _accountRepository = accountRepository;
    private readonly ICreditCardRepository _creditCardRepository = creditCardRepository;
    private readonly ILoanRepository _loanRepository = loanRepository;
    private readonly ISavingAccountService _savingAccountService = savingAccountService;

    public async Task<Result<List<TransactionDto>>> GetByAccountNumberAsync(string accountNumber)
    {
        try
        {
            var transactions = await _transactionRepository.GetByAccountNumberAsync(accountNumber);
            var dtos = mapper.Map<List<TransactionDto>>(transactions);
            return Result<List<TransactionDto>>.Ok(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<TransactionDto>>.Fail($"Error al obtener transacciones: {ex.Message}");
        }
    }
/*
 * Hacer aqui la implementacion de los metodos los comente para que me pudiera dejar correr la app 
 * 
    public async Task<Result<TransactionDto>> ProcessExpressTransferAsync(ExpressTransferDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionDto>> ProcessBeneficiaryTransferAsync(BeneficiaryTransferDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionDto>> ProcessCreditCardPaymentAsync(CreditCardPaymentDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionDto>> ProcessLoanPaymentAsync(LoanPaymentDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionDto>> ProcessAccountTransferAsync(AccountTransferDto dto)
    {
        throw new NotImplementedException();
    }*/

    public async Task<Result<TransactionDto>> ProcessDepositAsync(DepositDto dto)
    {
        try
        {
            // Validar que la cuenta existe y está activa
            var account = await _accountRepository.GetByAccountNumberAsync(dto.AccountNumber);
            if (account == null)
            {
                return Result<TransactionDto>.Fail("La cuenta especificada no existe.");
            }

            if (!account.IsActive)
            {
                return Result<TransactionDto>.Fail("La cuenta especificada está inactiva.");
            }

            // Actualizar el balance de la cuenta (aumentar)
            var newBalance = account.Balance + dto.Amount;
            var updateResult = await _savingAccountService.UpdateBalanceAsync(dto.AccountNumber, newBalance);
            if (updateResult.IsFailure)
            {
                return Result<TransactionDto>.Fail(updateResult.GeneralError ?? "Error al actualizar el balance de la cuenta.");
            }

            // Crear la transacción
            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Type = TransactionType.Credit,
                Origin = $"TELLER_{dto.TellerId}",
                Beneficiary = dto.AccountNumber,
                Date = DateTime.Now,
                Status = TransactionStatus.Approved,
                AccountNumber =  dto.AccountNumber,
                CreatedById = dto.TellerId,
                SubType = TransactionSubType.ExpressTransfer
            };

            var savedTransaction = await _transactionRepository.AddAsync(transaction);
            var transactionDto = mapper.Map<TransactionDto>(savedTransaction);

            return Result<TransactionDto>.Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return Result<TransactionDto>.Fail($"Error al procesar el depósito: {ex.Message}");
        }
    }

    public async Task<Result<TransactionDto>> ProcessWithdrawalAsync(WithdrawalDto dto)
    {
        try
        {
            // Validar que la cuenta existe y está activa
            var account = await _accountRepository.GetByAccountNumberAsync(dto.AccountNumber);
            if (account == null)
            {
                return Result<TransactionDto>.Fail("La cuenta especificada no existe.");
            }

            if (!account.IsActive)
            {
                return Result<TransactionDto>.Fail("La cuenta especificada está inactiva.");
            }

            // Validar que hay suficiente saldo
            if (account.Balance < dto.Amount)
            {
                return Result<TransactionDto>.Fail("La cuenta no tiene suficiente saldo para realizar este retiro.");
            }

            // Actualizar el balance de la cuenta (disminuir)
            var newBalance = account.Balance - dto.Amount;
            var updateResult = await _savingAccountService.UpdateBalanceAsync(dto.AccountNumber, newBalance);
            if (updateResult.IsFailure)
            {
                return Result<TransactionDto>.Fail(updateResult.GeneralError ?? "Error al actualizar el balance de la cuenta.");
            }

            // Crear la transacción
            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Type = TransactionType.Debit,
                Origin = dto.AccountNumber,
                Beneficiary = "CASH",
                Date = DateTime.Now,
                Status = TransactionStatus.Approved,
                AccountNumber = dto.AccountNumber,
                CreatedById = null!,
                SubType = TransactionSubType.ExpressTransfer
            };

            var savedTransaction = await _transactionRepository.AddAsync(transaction);
            var transactionDto = mapper.Map<TransactionDto>(savedTransaction);

            return Result<TransactionDto>.Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return Result<TransactionDto>.Fail($"Error al procesar el retiro: {ex.Message}");
        }
    }

    public async Task<Result<TransactionDto>> ProcessTellerTransactionAsync(TellerTransactionDto dto)
    {
        try
        {
            // Validar que las cuentas no sean la misma
            if (dto.SourceAccountNumber == dto.DestinationAccountNumber)
            {
                return Result<TransactionDto>.Fail("La cuenta origen y destino no pueden ser la misma.");
            }

            // Validar cuenta origen
            var sourceAccount = await _accountRepository.GetByAccountNumberAsync(dto.SourceAccountNumber);
            if (sourceAccount == null)
            {
                return Result<TransactionDto>.Fail("La cuenta origen especificada no existe.");
            }

            if (!sourceAccount.IsActive)
            {
                return Result<TransactionDto>.Fail("La cuenta origen especificada está inactiva.");
            }

            // Validar que hay suficiente saldo
            if (sourceAccount.Balance < dto.Amount)
            {
                return Result<TransactionDto>.Fail("La cuenta origen no tiene suficiente saldo para realizar esta transacción.");
            }

            // Validar cuenta destino
            var destinationAccount = await _accountRepository.GetByAccountNumberAsync(dto.DestinationAccountNumber);
            if (destinationAccount == null)
            {
                return Result<TransactionDto>.Fail("La cuenta destino especificada no existe.");
            }

            if (!destinationAccount.IsActive)
            {
                return Result<TransactionDto>.Fail("La cuenta destino especificada está inactiva.");
            }

            // Actualizar balance de cuenta origen (disminuir)
            var sourceNewBalance = sourceAccount.Balance - dto.Amount;
            var sourceUpdateResult = await _savingAccountService.UpdateBalanceAsync(dto.SourceAccountNumber, sourceNewBalance);
            if (sourceUpdateResult.IsFailure)
            {
                return Result<TransactionDto>.Fail(sourceUpdateResult.GeneralError ?? "Error al actualizar el balance de la cuenta origen.");
            }

            // Actualizar balance de cuenta destino (aumentar)
            var destinationNewBalance = destinationAccount.Balance + dto.Amount;
            var destinationUpdateResult = await _savingAccountService.UpdateBalanceAsync(dto.DestinationAccountNumber, destinationNewBalance);
            if (destinationUpdateResult.IsFailure)
            {
                // Rollback: revertir el cambio en la cuenta origen
                await _savingAccountService.UpdateBalanceAsync(dto.SourceAccountNumber, sourceAccount.Balance);
                return Result<TransactionDto>.Fail(destinationUpdateResult.GeneralError ?? "Error al actualizar el balance de la cuenta destino.");
            }

            // Crear la transacción
            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Type = TransactionType.Debit,
                Origin = dto.SourceAccountNumber,
                Beneficiary = dto.DestinationAccountNumber,
                Date = DateTime.Now,
                Status = TransactionStatus.Approved,
                AccountNumber = null,
                CreatedById = null,
                SubType = TransactionSubType.ExpressTransfer
            };

            var savedTransaction = await _transactionRepository.AddAsync(transaction);
            var transactionDto = mapper.Map<TransactionDto>(savedTransaction);

            return Result<TransactionDto>.Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return Result<TransactionDto>.Fail($"Error al procesar la transferencia: {ex.Message}");
        }
    }

    public async Task<Result<TransactionDto>> ProcessTellerCreditCardPaymentAsync(TellerCreditCardPaymentDto dto)
    {
        try
        {
            // Validar cuenta origen
            var sourceAccount = await _accountRepository.GetByAccountNumberAsync(dto.SourceAccountNumber);
            if (sourceAccount == null)
            {
                return Result<TransactionDto>.Fail("La cuenta origen especificada no existe.");
            }

            if (!sourceAccount.IsActive)
            {
                return Result<TransactionDto>.Fail("La cuenta origen especificada está inactiva.");
            }

            // Validar que hay suficiente saldo
            if (sourceAccount.Balance < dto.Amount)
            {
                return Result<TransactionDto>.Fail("La cuenta origen no tiene suficiente saldo para realizar este pago.");
            }

            // Validar tarjeta de crédito
            var creditCard = await _creditCardRepository.GetByCardNumberAsync(dto.CreditCardNumber);
            if (creditCard == null)
            {
                return Result<TransactionDto>.Fail("La tarjeta de crédito especificada no existe.");
            }

            if (!creditCard.IsActive)
            {
                return Result<TransactionDto>.Fail("La tarjeta de crédito especificada está inactiva.");
            }

            // Actualizar balance de cuenta origen (disminuir)
            var sourceNewBalance = sourceAccount.Balance - dto.Amount;
            var sourceUpdateResult = await _savingAccountService.UpdateBalanceAsync(dto.SourceAccountNumber, sourceNewBalance);
            if (sourceUpdateResult.IsFailure)
            {
                return Result<TransactionDto>.Fail(sourceUpdateResult.GeneralError ?? "Error al actualizar el balance de la cuenta origen.");
            }

            // Actualizar balance de la tarjeta de crédito (disminuir deuda)
            var creditCardNewBalance = creditCard.Balance - dto.Amount;
            if (creditCardNewBalance < 0)
            {
                creditCardNewBalance = 0; // No puede ser negativo
            }

            var creditCardDto = mapper.Map<CreditCardDto>(creditCard);
            creditCardDto.Balance = creditCardNewBalance;
            // Nota: Necesitarías un servicio de tarjeta de crédito para actualizar el balance
            // Por ahora, solo registramos la transacción

            // Crear la transacción
            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Type = TransactionType.Debit,
                Origin = dto.SourceAccountNumber,
                Beneficiary = dto.CreditCardNumber,
                Date = DateTime.Now,
                Status = TransactionStatus.Approved,
                AccountNumber = null,
                CreatedById = null,
                SubType = TransactionSubType.ExpressTransfer
            };

            var savedTransaction = await _transactionRepository.AddAsync(transaction);
            var transactionDto = mapper.Map<TransactionDto>(savedTransaction);

            return Result<TransactionDto>.Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return Result<TransactionDto>.Fail($"Error al procesar el pago de tarjeta de crédito: {ex.Message}");
        }
    }

    public async Task<Result<TransactionDto>> ProcessTellerLoanPaymentAsync(TellerLoanPaymentDto dto)
    {
        try
        {
            // Validar cuenta origen
            var sourceAccount = await _accountRepository.GetByAccountNumberAsync(dto.SourceAccountNumber);
            if (sourceAccount == null)
            {
                return Result<TransactionDto>.Fail("La cuenta origen especificada no existe.");
            }

            if (!sourceAccount.IsActive)
            {
                return Result<TransactionDto>.Fail("La cuenta origen especificada está inactiva.");
            }

            // Validar que hay suficiente saldo
            if (sourceAccount.Balance < dto.Amount)
            {
                return Result<TransactionDto>.Fail("La cuenta origen no tiene suficiente saldo para realizar este pago.");
            }

            // Validar préstamo
            var loan = await _loanRepository.GetByLoanNumberAsync(dto.LoanNumber);
            if (loan == null)
            {
                return Result<TransactionDto>.Fail("El préstamo especificado no existe.");
            }

            if (loan.Completed)
            {
                return Result<TransactionDto>.Fail("El préstamo especificado ya está completado.");
            }

            // Actualizar balance de cuenta origen (disminuir)
            var sourceNewBalance = sourceAccount.Balance - dto.Amount;
            var sourceUpdateResult = await _savingAccountService.UpdateBalanceAsync(dto.SourceAccountNumber, sourceNewBalance);
            if (sourceUpdateResult.IsFailure)
            {
                return Result<TransactionDto>.Fail(sourceUpdateResult.GeneralError ?? "Error al actualizar el balance de la cuenta origen.");
            }

            // Nota: Aquí deberías actualizar el préstamo (disminuir el monto pendiente)
            // Por ahora, solo registramos la transacción

            // Crear la transacción
            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Type = TransactionType.Debit,
                Origin = dto.SourceAccountNumber,
                Beneficiary = dto.LoanNumber,
                Date = DateTime.Now,
                Status = TransactionStatus.Approved,
                AccountNumber = null,
                CreatedById = null,
                SubType = TransactionSubType.ExpressTransfer
            };

            var savedTransaction = await _transactionRepository.AddAsync(transaction);
            var transactionDto = mapper.Map<TransactionDto>(savedTransaction);

            return Result<TransactionDto>.Ok(transactionDto);
        }
        catch (Exception ex)
        {
            return Result<TransactionDto>.Fail($"Error al procesar el pago de préstamo: {ex.Message}");
        }
    }

    public async Task<Result<TransactionSummaryDto>> GetTransactionSummaryAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionSummaryDto>> GetTellerTransactionSummaryAsync(string tellerId)
    {
        try
        {
            var summary = new TransactionSummaryDto
            {
                TotalTransactions = await _transactionRepository.GetTotalTransactionsByTellerAsync(tellerId),
                TodayTransactions = await _transactionRepository.GetTodayTransactionsByTellerAsync(tellerId),
                TotalDeposits = await _transactionRepository.GetTotalDepositsByTellerAsync(tellerId),
                TodayDeposits = await _transactionRepository.GetTodayDepositsByTellerAsync(tellerId),
                TotalWithdrawals = await _transactionRepository.GetTotalWithdrawalsByTellerAsync(tellerId),
                TodayWithdrawals = await _transactionRepository.GetTodayWithdrawalsByTellerAsync(tellerId),
                TotalPayments = await _transactionRepository.GetTotalPaymentsByTellerAsync(tellerId),
                TodayPayments = await _transactionRepository.GetTodayPaymentsByTellerAsync(tellerId)
            };

            return Result<TransactionSummaryDto>.Ok(summary);
        }
        catch (Exception ex)
        {
            return Result<TransactionSummaryDto>.Fail($"Error al obtener el resumen de transacciones: {ex.Message}");
        }
    }
}