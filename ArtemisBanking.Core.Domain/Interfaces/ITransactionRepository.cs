using ArtemisBanking.Core.Domain.Entities;

namespace ArtemisBanking.Core.Domain.Interfaces;

public interface ITransactionRepository : IGenericRepository<int, Transaction>
{
}