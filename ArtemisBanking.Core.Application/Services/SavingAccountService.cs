using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services;

public class SavingAccountService : GenericServices<string, SavingAccount, SavingAccountDto>, ISavingAccountService 
{
    private readonly ISavingAccountRepository _savingAccountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBaseAccountService _accountServiceForWebApp;
    private readonly IMapper _mapper;
    public SavingAccountService(ISavingAccountRepository repository, IMapper mapper, IBaseAccountService accountServiceForWebApp, ITransactionRepository transactionRepository) : base(repository, mapper)
    {
        _savingAccountRepository = repository;
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
        _transactionRepository = transactionRepository;
    }

    public override async Task<Result<SavingAccountDto>> AddAsync(SavingAccountDto dtoModel)
    {
        var clientResult = await _accountServiceForWebApp.GetUserById(dtoModel.ClientId);
        if (clientResult.IsFailure)
        {
            return Result<SavingAccountDto>.Fail(clientResult.GeneralError!);
        }
        
        var userWhoApprovedResult = await _accountServiceForWebApp.GetUserById(dtoModel.AssignedByUserId);
        if (userWhoApprovedResult.IsFailure)
        {
            return Result<SavingAccountDto>.Fail(userWhoApprovedResult.GeneralError!);
        }

        var userWhoApproved = userWhoApprovedResult.Value!;
        if (userWhoApproved.Role != nameof(Roles.Admin))
        {

            return Result<SavingAccountDto>.Fail("No estas autorizado para asignar tarjetas de credito");
        }
        
        return await base.AddAsync(dtoModel);
    }

    public async Task<PaginatedData<SavingAccountDto>> GetSavingAccountPagedAsync(int page, int pageSize, string? identityCardNumber = null, bool? isActive = null,
        string type = null)
    {
        UserDto? client = null;
        if (!string.IsNullOrEmpty(identityCardNumber))
        {
            var clientResult = await _accountServiceForWebApp.GetByIdentityCardNumber(identityCardNumber);

            if (clientResult.IsFailure || clientResult.Value is null)
                return new PaginatedData<SavingAccountDto>([], 0, page, pageSize);

            client = clientResult.Value;
        }
        var query = _savingAccountRepository.GetAllQueryable().AsNoTracking();
        if (client is not null)
            query = query.Where(l => l.ClientId == client.Id);
        if (isActive.HasValue)
            query = query.Where(l => l.IsActive == isActive.Value);

        var usersIds = await query.Select(l => l.ClientId).Distinct().ToListAsync();
        var usersResult = await _accountServiceForWebApp.GetUsersByIds(usersIds);
        var usersDict = usersResult.Value!.ToDictionary(user => user.Id, user => user);

        var items = await query
            .OrderByDescending(l => l.IsActive)
            .ThenByDescending(l => l.CreatedAt)
            .Select(s => new  SavingAccountDto
            {
                Id = s.Id,
                ClientId = s.ClientId,
                IsActive = s.IsActive,
                Balance = s.Balance,
                CreatedAt = s.CreatedAt,
                AssignedByUserId = s.AssignedByUserId,
                IsPrincipalAccount = s.IsPrincipalAccount,
                Client = usersDict[s.ClientId],
            }).ToListAsync();

        var data = PaginatedData<SavingAccountDto>.Create(items, page, pageSize);
        return data;
    }

    public async Task<Result<bool>> HasSufficientBalanceAsync(string accountNumber, decimal amount)
    {
        var account = await _savingAccountRepository.GetByIdAsync(accountNumber);
        if (account == null)
        {
            return Result<bool>.Fail("No se encontro alguna cuenta con este identificador");
        }
        
        if (account.Balance < amount)
        {
            return Result<bool>.Fail("La cuenta no tiene suficientes fondos");
        }
        
        return Result<bool>.Ok(true);
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

    public async Task<Result> DepositToAccountAsync(string accountNumber, decimal amount)
    {
        var account = await _savingAccountRepository.GetByIdAsync(accountNumber);
        
        if (account == null)
            return Result<SavingAccountDto>.Fail("No se encontró ninguna cuenta principal para ese usuario");

        if (!account.IsActive)
            return Result<SavingAccountDto>.Fail("La cuenta se encuentra desactivada/Cancelada en este momento");
       
        account.Balance += amount;
        await _savingAccountRepository.UpdateAsync(account.Id,account);
        
        return Result.Ok();
    }

    public async Task<Result> WithdrawFromAccount(string accountNumber, decimal amount)
    {
        var account = await _savingAccountRepository.GetByIdAsync(accountNumber);
        
        if (account == null)
            return Result<SavingAccountDto>.Fail("No se encontró ninguna cuenta principal para ese usuario");

        if (account.Balance < amount)
            return Result.Fail("Fondos insuficientes");
        
        if (!account.IsActive)
            return Result<SavingAccountDto>.Fail("La cuenta se encuentra desactivada/Cancelada en este momento");
       
        account.Balance -= amount;
        await _savingAccountRepository.UpdateAsync(account.Id,account);
        
        return Result.Ok();
        
    }



public async Task<Result> CancelAccountAsync(string accountNumber, string adminWhoCanceled)
{
    var accountToCancel = await _savingAccountRepository.GetByIdAsync(accountNumber);
    if (accountToCancel == null)
        return Result.Fail("No se encontró dicha cuenta");

    var mainAccountResult = await GetMainAccountByUserIdAsync(accountToCancel.ClientId);
    if (mainAccountResult.IsFailure)
        return Result.Fail("No se encontró la cuenta principal del cliente");

    var mainAccount = mainAccountResult.Value!;

    // Si tiene dinero, moverlo a la cuenta principal
    if (accountToCancel.Balance > 0)
    {
        decimal amount = accountToCancel.Balance;
        var debitTx = new Transaction
        {
            Id = 0, 
            Amount = amount,
            CreatedById = adminWhoCanceled,
            AccountNumber = accountToCancel.Id,
            Type = TransactionType.Debit,
            Beneficiary = mainAccount.Id,
            Origin = accountToCancel.Id,
            Date = DateTime.Now,
            SubType = TransactionSubType.AccountTransfer,
            Status = TransactionStatus.Approved
        };

        await _transactionRepository.AddAsync(debitTx);

        var withdrawResult = await WithdrawFromAccount(accountToCancel.Id, amount);
        if (withdrawResult.IsFailure)
            return withdrawResult;

        var creditTx = new Transaction
        {
            Id = 0, 
            Amount = amount,
            CreatedById = adminWhoCanceled,
            AccountNumber = mainAccount.Id,
            Type = TransactionType.Credit,
            Beneficiary = mainAccount.Id,
            Origin = accountToCancel.Id,
            Date = DateTime.Now,
            SubType = TransactionSubType.AccountTransfer,
            Status = TransactionStatus.Approved
        };

        await _transactionRepository.AddAsync(creditTx);

        var depositResult = await DepositToAccountAsync(mainAccount.Id, amount);
        if (depositResult.IsFailure)
            return depositResult;
    }

    // Finalmente desactivar la cuenta
    await _savingAccountRepository.SetStatus(accountNumber, false);

    return Result.Ok();
}


    public async Task<Result<SavingAccountDto>> CreateNewSavingAccountCard(string adminWhoApproved, string clientId, decimal initialAmount)
    {
        var savingAccount = new SavingAccountDto
        {
            ClientId = clientId,
            Balance = initialAmount,
            CreatedAt = DateTime.Now,
            AssignedByUserId = adminWhoApproved,
            IsPrincipalAccount = false,
            IsActive = true
        };
        
        return await AddAsync(savingAccount);
    }
}