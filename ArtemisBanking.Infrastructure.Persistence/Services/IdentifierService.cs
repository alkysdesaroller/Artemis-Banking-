using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Infrastructure.Persistence.Services;

public class IdentifierService : IIdentifierService
{
    private readonly ArtemisContext _context;

    public IdentifierService(ArtemisContext context)
    {
        _context = context;
    }

    public async Task<string> GenerateIdentifier()
    {
         var nextValue =  await _context.Database.SqlQueryRaw<long>("SELECT NEXT VALUE FOR SeqLoanAndAccountNumbersID")
             .SingleAsync();

         return nextValue.ToString("D9");
    }
    public async Task<string> GenerateCreditCardNumber()
    {
        var nextValue =  await _context.Database.SqlQueryRaw<long>("SELECT NEXT VALUE FOR SeqCreditCardsID")
            .SingleAsync();

        return nextValue.ToString("D16");
    }
}