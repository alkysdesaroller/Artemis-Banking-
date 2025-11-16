using System.Security.Claims;
using ArtemisBanking.Core.Application;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.CreditCard;
using ArtemisBanking.Core.Application.ViewModels.Loan;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{nameof(Roles.Admin)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class CreditCardsController : Controller
{
    private readonly IRiskService _riskService;
    private readonly ICardTransactionService _cardTransactionService;
    private readonly ICreditCardService _creditCardService;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IMapper _mapper;
    // GET
    public CreditCardsController(IMapper mapper, IRiskService riskService, ICreditCardService creditCardService, ICardTransactionService cardTransactionService, IAccountServiceForWebApp accountServiceForWebApp)
    {
        _mapper = mapper;
        _riskService = riskService;
        _creditCardService = creditCardService;
        _cardTransactionService = cardTransactionService;
        _accountServiceForWebApp = accountServiceForWebApp;
    }

    public async Task<IActionResult> Index(CreditCardFilterViewModel filters)
    {
        var paginatedDataDto = await _creditCardService.GetCreditCardsPagedAsync(
            filters.Page,
            filters.PageSize,
            filters.IdentityCardNumber,
            filters.IsActive
            );
        var viewmodels = _mapper.Map<List<CreditCardViewModel>>(paginatedDataDto.Items);
        var indexViewModel = new CreditCardIndexViewModel() 
        {
            Data = new PaginatedData<CreditCardViewModel>(viewmodels, paginatedDataDto.Pagination),
            Filter = filters,
        };
        return View(indexViewModel);
    }



    public async Task<IActionResult> Details(string cardNumber)
    {
        var cardTransactionResult = await _cardTransactionService.GetByCreditCardIdAsync(cardNumber);
        var cardTransaction = cardTransactionResult.Value!;
        if (cardTransactionResult.IsFailure)
        {
            this.SendValidationErrorMessages(cardTransactionResult);
            return View("Details");
        }
        return View(_mapper.Map<CreditCardViewModel>(cardTransaction));
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
        return View("CreateCreditCard", new CreateCreditCardViewModel
        {
            ClientId = selectedClientId,
        });
        
    }

    [HttpPost]
    public async Task<IActionResult> CreateCreditCard(CreateCreditCardViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("CreateCreditCard", model);
        }

        var result = await _creditCardService.CreateNewCreditCard(
            adminWhoApproved: User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            clientId: model.ClientId,
            creditLimit: model.CreditLimit
        );
       
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);    
            return View("CreateCreditCard", model);
        }
        
        return RedirectToRoute(new { controller = "CreditCards", action = "Index" });
    }
    
    public async Task<IActionResult> EditCreditLimit(string cardNumber)
    {
        var creditCardResult = await _creditCardService.GetByIdAsync(cardNumber);
        
        if (creditCardResult.IsFailure)
        {
            this.SendValidationErrorMessages(creditCardResult);
            return View("Index");
        }

        if (!creditCardResult.Value!.IsActive)
        {
            ViewBag.Message = "Esta tarjeta no se encuentra activa";
            return View("Index");
        }
            

        return View("EditCreditLimit", new EditCreditCardLimitViewModel
        {
            CardNumber = cardNumber,
            CreditLimit = creditCardResult.Value!.CreditLimit,
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> EditCreditLimit(EditCreditCardLimitViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _creditCardService.UpdateLimitAsync(model.CardNumber, model.CreditLimit);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Index");
        }
        return RedirectToRoute(new { controller = "CreditCards", action = "Index" });
   }
    
    public async Task<IActionResult> Cancel(string cardNumber)
    {
        return View("Cancel", cardNumber); 
    }
    
    [HttpPost]
    public async Task<IActionResult> CancelPost(string cardNumber)
    {
        var creditCardResult = await _creditCardService.GetByIdAsync(cardNumber);
        if (creditCardResult.IsFailure)
        {
            this.SendValidationErrorMessages(creditCardResult);
            return View("Cancel",cardNumber);
        }

        var result = await _creditCardService.CancelCardAsync(cardNumber);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Cancel");
        }
        return RedirectToRoute(new { controller = "CreditCards", action = "Index" });
    }
    
}