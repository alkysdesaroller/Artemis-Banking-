using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;

public class SavingAccountRepository : GenericRepository<string, SavingAccount>, ISavingAccountRepository
{
    public SavingAccountRepository(ArtemisContext context) : base(context)
    {
    }
}