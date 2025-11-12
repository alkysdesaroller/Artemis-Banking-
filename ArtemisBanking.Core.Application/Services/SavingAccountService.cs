using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services;

public class SavingAccountService : GenericServices<string, SavingAccount, SavingAccountDto>, ISavingAccountService 
{
    private readonly ISavingAccountRepository _savingAccountRepository;
    private readonly IMapper _mapper;
    public SavingAccountService(ISavingAccountRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _savingAccountRepository = repository;
        _mapper = mapper;
    }

    public async Task<Result<SavingAccountDto>> GetMainAccountByUserIdAsync(string userId)
    {
        var account = await _savingAccountRepository.GetAllQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ClientId == userId && s.IsPrincipalAccount);

        if (account == null)
            return Result<SavingAccountDto>.Fail("No se encontró ninguna cuenta principal para ese usuario");

        if (!account.IsActive)
            return Result<SavingAccountDto>.Fail("La cuenta se encuentra desactivada/Cancelada en este momento");
        
        return Result<SavingAccountDto>.Ok(_mapper.Map<SavingAccountDto>(account));
    }

    public async Task<Result> UpdateBalanceAsync(string accountNumber, decimal amount)
    {
        // AccountNumber es la PK. El repositorio llama internamente a FindAsync, que busca por PK 
        var account = await _savingAccountRepository.GetByIdAsync(accountNumber);

        if (account == null)
            return Result<SavingAccountDto>.Fail($"No se encontró ninguna cuenta con este numero {accountNumber}");
        
        if(amount < 0)
            return Result<SavingAccountDto>.Fail($"Las cuentas no pueden tener un monto menor a cero");
        
        var accountDto = _mapper.Map<SavingAccountDto>(account);
        accountDto.Balance = amount;
        
        var updateResult = await UpdateAsync(accountDto.Id, accountDto);
        
        if (updateResult.IsFailure)
            return updateResult;

        return Result<SavingAccountDto>.Ok(updateResult.Value!);
    }
}