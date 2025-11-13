using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;

public class SavingAccountRepository : GenericRepository<string, SavingAccount>, ISavingAccountRepository
{
    private readonly IIdentifierService _identifierService;
    public SavingAccountRepository(ArtemisContext context, IIdentifierService identifierService) : base(context)
    {
        _identifierService = identifierService;
    }

    public override async Task<SavingAccount> AddAsync(SavingAccount entity)
    {
        entity.Id = await _identifierService.GenerateIdentifier(); // Genera del ID de 9 digitos 
        return await base.AddAsync(entity);
    }

    public async Task<object> GetByAccountNumberAsync(string dtoAccountNumber)
    {
        throw new NotImplementedException();
    }
}