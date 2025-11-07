using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.LoanInstallment;
using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class TransactionService : GenericServices<int, Transaction, TransactionDto>, ITransactionService 
{
    private readonly ITransactionRepository _transactionRepository;
    public TransactionService(ITransactionRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _transactionRepository = repository;
    }
}