using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.LoanInstallment;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class LoanService : GenericServices<string, Loan, LoanDto>, ILoanService 
{
    private readonly ILoanRepository _loanRepository;
    public LoanService(ILoanRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _loanRepository = repository;
    }
}