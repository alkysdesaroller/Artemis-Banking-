using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;

public class CreditCardRepository : GenericRepository<string, CreditCard>, ICreditCardRepository
{
    public CreditCardRepository(ArtemisContext context) : base(context)
    {
    }
}