using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;

public class LoanRepository : GenericRepository<string, Loan>, ILoanRepository 
{
    public LoanRepository(ArtemisContext context) : base(context)
    {
    }
}