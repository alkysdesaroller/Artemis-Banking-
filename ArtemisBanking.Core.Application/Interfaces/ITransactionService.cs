using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Dtos.Transaction.Teller;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface ITransactionService : IGenericService<int, TransactionDto>
{

    Task<Result<List<TransactionDto>>> GetByAccountNumberAsync(string accountNumber);
    Task<Result<TransactionDto>> ProcessExpressTransferAsync(ExpressTransferDto dto);
    Task<Result<TransactionDto>> ProcessBeneficiaryTransferAsync(BeneficiaryTransferDto dto);
    Task<Result<TransactionDto>> ProcessCreditCardPaymentAsync(CreditCardPaymentDto dto);
    Task<Result<TransactionDto>> ProcessLoanPaymentAsync(LoanPaymentDto dto);
    Task<Result<TransactionDto>> ProcessAccountTransferAsync(AccountTransferDto dto);
    Task<Result<TransactionDto>> ProcessDepositAsync(DepositDto dto);
    Task<Result<TransactionDto>> ProcessWithdrawalAsync(WithdrawalDto dto);
    Task<Result<TransactionDto>> ProcessTellerTransactionAsync(TellerTransactionDto dto);
    Task<Result<TransactionDto>> ProcessTellerCreditCardPaymentAsync(TellerCreditCardPaymentDto dto);
    Task<Result<TransactionDto>> ProcessTellerLoanPaymentAsync(TellerLoanPaymentDto dto);
    Task<Result<TransactionSummaryDto>> GetTransactionSummaryAsync();
    Task<Result<TransactionSummaryDto>> GetTellerTransactionSummaryAsync(string tellerId);
}