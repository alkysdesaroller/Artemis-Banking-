using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;
using Microsoft.Extensions.Configuration;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;

public class CreditCardRepository : GenericRepository<string, CreditCard>, ICreditCardRepository
{
    private readonly IIdentifierService _identifierService;
    public CreditCardRepository(ArtemisContext context, IIdentifierService identifierService) : base(context)
    {
        _identifierService = identifierService;
    }

    public override async Task<CreditCard> AddAsync(CreditCard entity)
    {
        entity.CardNumber = await _identifierService.GenerateCreditCardNumber();
        return await base.AddAsync(entity);
    }
}