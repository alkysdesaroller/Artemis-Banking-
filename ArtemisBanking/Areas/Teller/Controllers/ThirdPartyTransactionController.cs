using ArtemisBanking.Core.Application.Dtos.Transaction.Teller;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Teller;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Extensions;
using ArtemisBanking.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Teller.Controllers;

[Area("Teller")]
[Authorize(Roles = $"{nameof(Roles.Atm)}")]
public class ThirdPartyTransactionController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly UserManager<AppUser> _userManager;

    public ThirdPartyTransactionController(
        ITransactionService transactionService,
        ISavingAccountService savingAccountService,
        UserManager<AppUser> userManager)
    {
        _transactionService = transactionService;
        _savingAccountService = savingAccountService;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View(new ThirdPartyTransactionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ThirdPartyTransactionViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.SourceAccountNumber == model.DestinationAccountNumber)
        {
            ModelState.AddModelError("", "La cuenta origen y destino no pueden ser la misma.");
            return View(model);
        }

        var sourceAccountResult = await _savingAccountService.GetByIdAsync(model.SourceAccountNumber);
        if (sourceAccountResult.IsFailure || sourceAccountResult.Value == null)
        {
            ModelState.AddModelError("", "La cuenta origen no existe.");
            return View(model);
        }

        var source = sourceAccountResult.Value;

        if (!source.IsActive)
        {
            ModelState.AddModelError("", "La cuenta origen está inactiva.");
            return View(model);
        }

        if (source.Balance < model.Amount)
        {
            ModelState.AddModelError("", "Saldo insuficiente en cuenta origen.");
            return View(model);
        }

        var destAccountResult = await _savingAccountService.GetByIdAsync(model.DestinationAccountNumber);
        if (destAccountResult.IsFailure || destAccountResult.Value == null)
        {
            ModelState.AddModelError("", "La cuenta destino no existe.");
            return View(model);
        }

        var dest = destAccountResult.Value;

        if (!dest.IsActive)
        {
            ModelState.AddModelError("", "La cuenta destino está inactiva.");
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(dest.ClientId);
        var receiverName = user != null ? $"{user.FirstName} {user.LastName}" : "Desconocido";

        var confirmVm = new ConfirmThirdPartyTransactionViewModel
        {
            Transaction = new ThirdPartyTransactionViewModel
            {
                SourceAccountNumber = model.SourceAccountNumber,
                DestinationAccountNumber = model.DestinationAccountNumber,
                Amount = model.Amount,
                Description = model.Description
            },
            DestinationClientName = receiverName
        };

        return View("Confirm", confirmVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessTransfer(ConfirmThirdPartyTransactionViewModel model)
    {
        var teller = await _userManager.GetUserAsync(User);
        if (teller == null)
        {
            ModelState.AddModelError("", "No se pudo identificar al cajero.");
            return View("Confirm", model);
        }

        var dto = new TellerTransactionDto
        {
            SourceAccountNumber = model.Transaction.SourceAccountNumber,
            DestinationAccountNumber = model.Transaction.DestinationAccountNumber,
            Amount = model.Transaction.Amount,
            TellerId = teller.Id,
        };

        var result = await _transactionService.ProcessTellerTransactionAsync(dto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Confirm", model);
        }

        return RedirectToRoute(new { controller = "ThirdPartyTransaction", action = "Index" });
    }


}
