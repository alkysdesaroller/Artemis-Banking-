using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class SavingAccountService : GenericServices<string, SavingAccount, SavingAccountDto>, ISavingAccountService 
{
    private readonly ISavingAccountRepository _savingAccountRepository;
    public SavingAccountService(ISavingAccountRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _savingAccountRepository = repository;
    }
}