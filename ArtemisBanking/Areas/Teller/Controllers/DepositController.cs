using System.Security.Claims;
using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Teller;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Extensions;
using ArtemisBanking.Infrastructure.Identity.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Areas.Teller.Controllers;

[Area("Teller")]
[Authorize(Roles = $"{nameof(Roles.Atm)}")]
public class DepositController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public DepositController(
        ITransactionService transactionService,
        ISavingAccountService savingAccountService,
        UserManager<AppUser> userManager,
        IMapper mapper)
    {
        _transactionService = transactionService;
        _savingAccountService = savingAccountService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        return View(new DepositViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(DepositViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var accountResult = await _savingAccountService.GetByIdAsync(model.AccountNumber);
        if (accountResult.IsFailure || accountResult.Value == null)
        {
            ModelState.AddModelError("", "La cuenta especificada no existe.");
            return View(model);
        }

        var account = accountResult.Value;
        if (!account.IsActive)
        {
            ModelState.AddModelError("", "La cuenta especificada está inactiva.");
            return View(model);
        }
        
        var client = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == account.ClientId);
        if (client == null)
        {
            ModelState.AddModelError("", "No se encontro al cliente de esta cuenta");
            return View(model);
        }


        var confirmDeposit = new ConfirmDepositViewModel()
        {
            Deposit = new DepositViewModel()
            {
                AccountNumber = model.AccountNumber,
                Amount = model.Amount,
                Description = "",
                
            },
            ClientName = $"{client.FirstName} {client.LastName}"
        };
        
        return View("Confirm", confirmDeposit);

    }
    
    public async Task<IActionResult> Confirm(ConfirmDepositViewModel model)
    {

        return View(model);
        
        
 
    }

    [HttpPost]
    public async Task<IActionResult> ProcessDeposit(ConfirmDepositViewModel model)
    {
        var depositDto = new DepositDto
        {
            AccountNumber = model.Deposit.AccountNumber,
            Amount = model.Deposit.Amount,
            TellerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
        };
        
        var result = await _transactionService.ProcessDepositAsync(depositDto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Index", new DepositViewModel());
        }

        return RedirectToRoute(new {controller ="Deposit", action = "Index"});
    }
}