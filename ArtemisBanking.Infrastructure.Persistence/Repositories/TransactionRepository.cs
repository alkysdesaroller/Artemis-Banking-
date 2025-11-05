using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;

public class TransactionRepository : GenericRepository<int, Transaction>, ITransactionRepository
{
    public TransactionRepository(ArtemisContext context) : base(context)
    {
    }
}