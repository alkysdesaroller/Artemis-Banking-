using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.LoanInstallment;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services;

public class LoanService : GenericServices<string, Loan, LoanDto>, ILoanService 
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    public LoanService(ILoanRepository repository, IMapper mapper, IAccountServiceForWebApp accountServiceForWebApp) : base(repository, mapper)
    {
        _loanRepository = repository;
        _accountServiceForWebApp = accountServiceForWebApp;
    }
    
    public async Task<PaginatedData<LoanSummaryDto>> GetLoansPagedAsync(
        int page,
        int pageSize,
        string? identityCardNumber = null,
        bool? isCompleted = null)
    {
        UserDto? client = null;
        if (!string.IsNullOrEmpty(identityCardNumber))
        {
            var clientResult = await _accountServiceForWebApp.GetByIdentityCardNumber(identityCardNumber);

            if (clientResult.IsFailure || clientResult.Value is null)
                return new PaginatedData<LoanSummaryDto>([], 0, page, pageSize);

            client = clientResult.Value;
        }

        var query = _loanRepository.GetAllQueryable().AsNoTracking();

        if (client is not null)
            query = query.Where(l => l.ClientId == client.Id);

        if (isCompleted.HasValue)
            query = query.Where(l => l.Completed == isCompleted.Value);

        var totalCount = await query.CountAsync();

        var items = query
            .OrderByDescending(l => l.CreatedAt) 
            .Select(l => new LoanSummaryDto
            {
                Id = l.Id,
                Amount = l.Amount,
                CreatedAt = l.CreatedAt,
                Completed = l.Completed,
                ClientId = l.ClientId,
                ApprovedByUserId = l.ApprovedByUserId,
                TermMonths = l.TermMonths,
                InstallmentsCount = l.LoanInstallments.Count(),
                InstallmentsPaidCount = l.LoanInstallments.Count(installment => installment.IsPaid),
                RemainingBalanceToPay = l.Amount - (l.LoanInstallments
                                            .Where(inst => inst.IsPaid)
                                            .Sum(inst => (decimal?)inst.Amount) ?? 0),
                AnualRate = l.AnualRate,
                IsDue = l.IsDue
            });
        
        var data = PaginatedData<LoanSummaryDto>.Create(items, page, pageSize);
        return data;
    }

}