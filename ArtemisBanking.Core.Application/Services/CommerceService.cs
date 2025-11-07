using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.Commerce;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class CommerceService : GenericServices<int, Commerce, CommerceDto>, ICommerceService 
{
    private readonly ICommerceRepository _commerceRepository;
    public CommerceService(ICommerceRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _commerceRepository = repository;
    }


}