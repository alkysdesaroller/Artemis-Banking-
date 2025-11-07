using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class BeneficiaryService : GenericServices<int, Beneficiary, BeneficiaryDto>, IBeneficiaryService 
{
    private readonly IBeneficiaryRepository _beneficiaryRepository;
    public BeneficiaryService(IBeneficiaryRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _beneficiaryRepository = repository;
    }


}