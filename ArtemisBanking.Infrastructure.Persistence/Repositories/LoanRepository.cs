using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;

public class LoanRepository : GenericRepository<string, Loan>, ILoanRepository 
{
    private readonly IIdentifierService _identifierService;
    public LoanRepository(ArtemisContext context, IIdentifierService identifierService) : base(context)
    {
        _identifierService = identifierService;
    }
    public override async Task<Loan> AddAsync(Loan entity)
    {
        entity.Id = await _identifierService.GenerateIdentifier(); // Genera del ID de 9 digitos 
        return await base.AddAsync(entity);
    }
}