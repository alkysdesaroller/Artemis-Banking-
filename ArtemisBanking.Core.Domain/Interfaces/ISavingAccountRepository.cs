using ArtemisBanking.Core.Domain.Entities;

namespace ArtemisBanking.Core.Domain.Interfaces;

public interface ISavingAccountRepository : IGenericRepository<string, SavingAccount>
{
    Task SetStatus(string accountNumber, bool statusToSet);
}