using ArtemisBanking.Core.Domain.Entities;

namespace ArtemisBanking.Core.Domain.Interfaces;

public interface ISavingAccountRepository : IGenericRepository<string, SavingAccount>
{
    Task<object> GetByAccountNumberAsync(string dtoAccountNumber);
}