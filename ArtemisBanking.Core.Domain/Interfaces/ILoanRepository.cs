using ArtemisBanking.Core.Domain.Entities;

namespace ArtemisBanking.Core.Domain.Interfaces;

public interface ILoanRepository : IGenericRepository<string, Loan>
{
    
}