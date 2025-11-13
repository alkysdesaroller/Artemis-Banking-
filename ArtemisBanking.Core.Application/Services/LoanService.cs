using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services;

public class LoanService : GenericServices<string, Loan, LoanDto>, ILoanService 
{
    private readonly ILoanRepository _loanRepository;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IMapper _mapper;
    public LoanService(ILoanRepository repository, IMapper mapper, IAccountServiceForWebApp accountServiceForWebApp) : base(repository, mapper)
    {
        _loanRepository = repository;
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
    }

    public Task<List<UserDto>> GetClientsWithOutLoans()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<List<ClientsWithDebtDto>>> GetClientsWithoutActiveLoan()
    {
        var allClientsResult = await _accountServiceForWebApp.GetAllUserIdsOfRole(Roles.Client);
        var allClientIds = allClientsResult.Value!.ToList(); // materializa completamente

        var clientsWithActiveLoans = await _loanRepository.GetAllQueryable()
            .AsNoTracking()
            .Where(l => !l.Completed)   // préstamos activos
            .Select(l => l.ClientId)
            .Distinct()
            .ToListAsync();

        var clientsWithoutActiveLoansIds = allClientIds.Except(clientsWithActiveLoans).ToList();

        if (!clientsWithoutActiveLoansIds.Any())
        {
            return Result<List<ClientsWithDebtDto>>.Ok(new List<ClientsWithDebtDto>());
        }

        // Si el listado trae a todos los clientes que no tienen un prestamo, y un cliente solamente puede tener
        // un prestamo, pues... los clientes sin prestamos no tienen deuda alguna
        var usersResult = await _accountServiceForWebApp.GetUsersByIds(clientsWithoutActiveLoansIds);
        var clientsWithoutLoans = usersResult.Value!
            .Select(u => new ClientsWithDebtDto
            {
                Client = u,
                Debt = 0m // siempre 0 porque no tienen préstamos activos
            })
            .ToList();

        return Result<List<ClientsWithDebtDto>>.Ok(clientsWithoutLoans);
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

        var query = _loanRepository.GetAllQueryable().AsNoTracking(); // Construir el query, o la consulta

        if (client is not null)
            query = query.Where(l => l.ClientId == client.Id); // Se a;adio un filtro a mi consulta

        if (isCompleted.HasValue)
            query = query.Where(l => l.Completed == isCompleted.Value); // se a;ade ese filtro a la consulta.
        

        var totalCount = await query.CountAsync();

        var items = query
            .OrderByDescending(l => l.Completed) 
            .ThenByDescending(l => l.CreatedAt) 
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

    public async Task<Result<decimal>> GetAverageClientDebtAsync()
    {
        var query = _loanRepository.GetAllQueryable()
            .AsNoTracking()
            .Select(l => new
            {
                Debt = l.Amount - ((l.Amount / l.TermMonths) * l.LoanInstallments.Count(inst => inst.IsPaid))
            });

        var any = await query.AnyAsync();
        if (!any)
            return Result<decimal>.Ok(0m);

        var averageDebt = await query.AverageAsync(x => x.Debt);
        return Result<decimal>.Ok(averageDebt);
    }

}