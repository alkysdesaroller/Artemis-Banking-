using System.Security.Claims;
using ArtemisBanking.Core.Application;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.CreditCard;
using ArtemisBanking.Core.Application.ViewModels.Loan;
using ArtemisBanking.Core.Application.ViewModels.SavingAccount;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{nameof(Roles.Admin)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class SavingAccountsController : Controller
{
    private readonly IRiskService _riskService;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly IMapper _mapper;
    // GET
    public SavingAccountsController(IMapper mapper, IRiskService riskService, IAccountServiceForWebApp accountServiceForWebApp, ISavingAccountService savingAccountService, ITransactionService transactionService)
    {
        _mapper = mapper;
        _riskService = riskService;
        _accountServiceForWebApp = accountServiceForWebApp;
        _savingAccountService = savingAccountService;
        _transactionService = transactionService;
    }

    public async Task<IActionResult> Index(SavingAccountFiltersViewModel filters)
    {
        var paginatedDataDto = await _savingAccountService.GetSavingAccountPagedAsync(
            filters.Page,
            filters.PageSize,
            filters.IdentityCardNumber,
            filters.IsActive
            );
        var viewmodels = _mapper.Map<List<SavingAccountViewModel>>(paginatedDataDto.Items);
        var indexViewModel = new SavingAccountIndexViewModel() 
        {
            Data = new PaginatedData<SavingAccountViewModel>(viewmodels, paginatedDataDto.Pagination),
            Filter = filters,
        };
        return View(indexViewModel);
    }



    public async Task<IActionResult> Details(string accountNumber)
    {
        var accountResult = await _savingAccountService.GetByIdAsync(accountNumber);
        if (accountResult.IsFailure)
        {
            this.SendValidationErrorMessages(accountResult);
            return View("Details");
        }
        
        var client = await _accountServiceForWebApp.GetUserById(accountResult.Value!.ClientId);
        if (client is null)
        {
            ViewBag.Message = "No se encontro al usuario";
            return View("Details");
        }
        
        
        var transactionsResult = await _transactionService.GetByAccountNumberAsync(accountNumber);
        if (transactionsResult.IsFailure)
        {
            this.SendValidationErrorMessages(transactionsResult);
            return View("Details");
        }
        
        var account = accountResult.Value!;
        account.Transactions = transactionsResult.Value!;
        account.Client = client;
        return View(_mapper.Map<SavingAccountViewModel>(account));
    }
    

    public async Task<IActionResult> SelectUser(string? identityCardNumber = null)
    {
        // Trae los clientes activos sin préstamo activo
        var users = await _accountServiceForWebApp.GetAllUserIdsOfRole(Roles.Client);
        var usersWithDebtResult = await _riskService.GetDebtOfTheseUsers(users.Value!, identityCardNumber);
        var viewmodels = _mapper.Map<List<ClientsWithDebtViewModel>>(usersWithDebtResult.Value);
        return View(viewmodels);
        
    }
    
    [HttpPost]
    public async Task<IActionResult> SelectUserPost(string selectedClientId)
    {
        return View("Create", new CreateSavingAccountViewModel() 
        {
            ClientId = selectedClientId,
        });
        
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSavingAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Create", model);
        }

        var result = await _savingAccountService.CreateNewSavingAccountCard(
            adminWhoApproved: User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            clientId: model.ClientId,
            initialAmount: model.InitialAmount
        );
       
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);    
            return View("Create", model);
        }
        
        return RedirectToRoute(new { controller = "CreditCards", action = "Index" });
    }
    
    
    public async Task<IActionResult> Cancel(string accountNumber)
    {
        return View("Cancel", accountNumber); 
    }
    
    [HttpPost]
    public async Task<IActionResult> CancelPost(string accountNumber)
    {
        var account = await _savingAccountService.GetByIdAsync(accountNumber);
        if (account.IsFailure)
        {
            this.SendValidationErrorMessages(account);
            return View("Cancel",accountNumber);
        }

        var admin = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var result = await _savingAccountService.CancelAccountAsync(accountNumber, admin);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Cancel");
        }
        return RedirectToRoute(new { controller = "CreditCards", action = "Index" });
    }
    
}