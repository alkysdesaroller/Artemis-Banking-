using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

// Fijate en como se hereda de Generic Service y se implementan los metodos faltantes de IBeneficiaryService
public class BeneficiaryService : GenericServices<int, Beneficiary, BeneficiaryDto>, IBeneficiaryService 
{
    // Cambia el parametro GenericRepositorio<int, Beneficiary, BeneficiaryDto> repository
    // por la interfaz Repository adecuada. En este caso fue IBeneficiaryRepository
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    public BeneficiaryService(IBeneficiaryRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _beneficiaryRepository = repository;
    }

    public Task<Result<List<BeneficiaryDto>>> GetByUserId(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AccountNumberExists(string accountNumber)
    {
        throw new NotImplementedException();
    }
}