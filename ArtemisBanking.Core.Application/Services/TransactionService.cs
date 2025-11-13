using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.LoanInstallment;
using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Dtos.Transaction.Teller;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    ISavingAccountRepository accountRepository,
    ICreditCardRepository creditCardRepository,
    ILoanRepository loanRepository,
    IMapper mapper)
    : GenericServices<int, Transaction, TransactionDto>(transactionRepository, mapper), ITransactionService
{
    private readonly ITransactionRepository _transactionRepository = transactionRepository;
    private readonly ISavingAccountRepository _accountRepository = accountRepository;
    private readonly ICreditCardRepository _creditCardRepository = creditCardRepository;
    private readonly ILoanRepository _loanRepository = loanRepository;

    public async Task<Result<List<TransactionDto>>> GetByAccountNumberAsync(string accountNumber)
    {
        throw new NotImplementedException();
    }

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
    }

    public async Task<Result<TransactionDto>> ProcessDepositAsync(DepositDto dto)
    {
        throw new NotImplementedException();
    /*    var result = new Result<TransactionDto>();
        //busqueda de la cuenta bancaria destino
        var account = await _accountRepository.GetByAccountNumberAsync(dto.AccountNumber);
        if (account == null!)
        {
            result
        }
        */
    }

    public async Task<Result<TransactionDto>> ProcessWithdrawalAsync(WithdrawalDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionDto>> ProcessTellerTransactionAsync(TellerTransactionDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionDto>> ProcessTellerCreditCardPaymentAsync(TellerCreditCardPaymentDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionDto>> ProcessTellerLoanPaymentAsync(TellerLoanPaymentDto dto)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionSummaryDto>> GetTransactionSummaryAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<TransactionSummaryDto>> GetTellerTransactionSummaryAsync(string tellerId)
    {
        throw new NotImplementedException();
    }
}