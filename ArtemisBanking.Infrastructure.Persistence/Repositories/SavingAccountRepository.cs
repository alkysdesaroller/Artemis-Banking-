using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;

public class SavingAccountRepository(
    ArtemisContext context,
    IIdentifierService identifierService,
    ISavingAccountRepository savingAccountRepository)
    : GenericRepository<string, SavingAccount>(context), ISavingAccountRepository
{
    private readonly ISavingAccountRepository _savingAccountRepository = savingAccountRepository;

    public override async Task<SavingAccount> AddAsync(SavingAccount entity)
    {
        entity.Id = await identifierService.GenerateIdentifier(); // Genera del ID de 9 digitos 
        return await base.AddAsync(entity);
    }

    public async Task<object> GetByAccountNumberAsync(string dtoAccountNumber)
    {
        throw new NotImplementedException();
    }
}