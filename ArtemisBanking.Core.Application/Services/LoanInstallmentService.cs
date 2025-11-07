using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.LoanInstallment;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class LoanInstallmentService : GenericServices<int, LoanInstallment, LoanInstallmentDto>, ILoanInstallmentService 
{
    private readonly ILoanInstallmentRepository _loanInstallmentRepository;
    public LoanInstallmentService(ILoanInstallmentRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _loanInstallmentRepository = repository;
    }
}