using System.Xml.Linq;
using ArtemisBanking.Core.Application.Dtos;
using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.Saves;
using ArtemisBanking.Core.Domain.Entities;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface IBeneficiaryService : IGenericService<Beneficiary, SaveBeneficiaryDto,  BeneficiaryDto>
{
    Task<List<BeneficiaryDto>> GetByUserId(string userId);
    Task<bool> AccountNumberExists(string accountNumber);
}