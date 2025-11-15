using ArtemisBanking.Core.Application.Dtos.Email;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Enums;
using ArtemisBanking.Core.Application.Helpers;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;

namespace ArtemisBanking.Core.Application.Services;

public class TransactionService : GenericServices<int, Transaction, TransactionDto>, ITransactionService 
{
    
    private readonly ILoanRepository _loanRepository;
    
    private readonly IEmailService _emailService;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly ISavingAccountService _savingAccountService;
    public TransactionService(ITransactionRepository repository, IMapper mapper, ILoanRepository loanService, ISavingAccountService savingAccountService, IEmailService emailService, IAccountServiceForWebApp accountServiceForWebApp) : base(repository, mapper)
    {
        _loanRepository = loanService;
        _savingAccountService = savingAccountService;
        _emailService = emailService;
        _accountServiceForWebApp = accountServiceForWebApp;
    }

    public async Task<Result<TransactionDto>> ProcessLoanDisbursementTransfer(LoanDisbursementTransactionDto dto)
    {
        var loan = await _loanRepository.GetAllQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == dto.SourceLoanNumber);
        
        if (loan == null)
            return Result<TransactionDto>.Fail("No se encontró ningún préstamo con ese ID");

        var savingAccountResult = await _savingAccountService.GetByIdAsync(dto.DestinationAccountNumber);
        if (savingAccountResult.IsFailure)
            return Result<TransactionDto>.Fail(savingAccountResult.GeneralError!);

        if (!savingAccountResult.Value!.IsPrincipalAccount)
            return Result<TransactionDto>.Fail("esta no es una cuenta principal");
        
        var transactionDto = new TransactionDto
        {
            Id = 0,
            Amount = dto.Amount,
            CreatedById = dto.AprovedByAdminId,
            AccountNumber = dto.DestinationAccountNumber,
            Type = TransactionType.Credit,
            Beneficiary = dto.DestinationAccountNumber,
            Origin = dto.SourceLoanNumber,
            Date = DateTime.Now,
            SubType = TransactionSubType.Deposit,
            Status = TransactionStatus.Approved,
        };
        
        var transactionResult = await AddAsync(transactionDto);
        
        if (transactionResult.IsFailure) 
            return transactionResult;
        
        var depositResult = await _savingAccountService.DepositToAccountAsync(dto.DestinationAccountNumber, dto.Amount);
        
        if (depositResult.IsFailure)
            return Result<TransactionDto>.Fail(depositResult.GeneralError!);

        // para el email
        var getClient = await _accountServiceForWebApp.GetUserById(loan.ClientId);
        if (getClient.IsFailure)
        {
            return Result<TransactionDto>.Fail(getClient.GeneralError!);
        }
        var client = getClient.Value!;
        var account = savingAccountResult.Value!;
        
        var newMontly = MontlyPayment.Calculate(loan.Amount, loan.AnualRate, loan.TermMonths);

        await _emailService.SendTemplateEmailAsync(new EmailTemplateData
        {
            Type = EmailType.LoanApproved,
            To = client.Email,
            Variables =
            {
                ["LoanNumber"] = loan.Id,
                ["Amount"] = loan.Amount.ToString("N2"),
                ["Term"] = loan.TermMonths.ToString(),
                ["InterestRate"] = loan.TermMonths.ToString(),
                ["MonthlyPayment"] = newMontly.ToString("N2"),
            }
        });
        
        return transactionResult;
    }
}