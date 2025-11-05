using ArtemisBanking.Core.Domain.Entities;

namespace ArtemisBanking.Core.Domain.Interfaces;

public interface ICreditCardRepository : IGenericRepository<string, CreditCard>
{
    
}