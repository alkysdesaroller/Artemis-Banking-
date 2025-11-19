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
public class CreditCardPaymentController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly ICreditCardService _creditCardService;
    private readonly UserManager<AppUser> _userManager;

    public CreditCardPaymentController(
        ITransactionService transactionService,
        ISavingAccountService savingAccountService,
        ICreditCardService creditCardService,
        UserManager<AppUser> userManager)
    {
        _transactionService = transactionService;
        _savingAccountService = savingAccountService;
        _creditCardService = creditCardService;
        _userManager = userManager;
    }

    // PASO 1 — Página inicial
    public IActionResult Index()
    {
        return View(new CreditCardPaymentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CreditCardPaymentViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Verificar cuenta origen
        var accountResult = await _savingAccountService.GetByIdAsync(model.SourceAccountNumber);
        if (accountResult.IsFailure || accountResult.Value == null)
        {
            ModelState.AddModelError("", "La cuenta origen especificada no existe.");
            return View(model);
        }

        var account = accountResult.Value;
        if (!account.IsActive)
        {
            ModelState.AddModelError("", "La cuenta origen está inactiva.");
            return View(model);
        }

        if (account.Balance < model.Amount)
        {
            ModelState.AddModelError("", "Saldo insuficiente en la cuenta.");
            return View(model);
        }

        var cardResult = await _creditCardService.GetByIdAsync(model.CreditCardNumber);
        if (cardResult.IsFailure || cardResult.Value == null)
        {
            ModelState.AddModelError("", "La tarjeta de crédito especificada no existe.");
            return View(model);
        }

        var creditCard = cardResult.Value;

        var user = await _userManager.FindByIdAsync(creditCard.ClientId);
        var clientName = user != null ? $"{user.FirstName} {user.LastName}" : "Desconocido";

        var confirmVm = new ConfirmCreditCardPaymentViewModel
        {
            Payment = new CreditCardPaymentViewModel
            {
                SourceAccountNumber = model.SourceAccountNumber,
                CreditCardNumber = model.CreditCardNumber,
                Amount = model.Amount,
                Description = model.Description
            },
            CardOwnerName = clientName
        };

        return View("Confirm", confirmVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPayment(ConfirmCreditCardPaymentViewModel model)
    {
        var teller = await _userManager.GetUserAsync(User);
        if (teller == null)
        {
            ModelState.AddModelError("", "No se pudo identificar al cajero.");
            return View("Confirm", model);
        }

        var dto = new TellerCreditCardPaymentDto
        {
            SourceAccountNumber = model.Payment.SourceAccountNumber,
            CreditCardNumber = model.Payment.CreditCardNumber,
            Amount = model.Payment.Amount,
            TellerId = teller.Id
        };

        var result = await _transactionService.ProcessTellerCreditCardPaymentAsync(dto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Confirm", model);
        }

        return RedirectToRoute(new { controller = "CreditCardPayment", action = "Index" });
    }


}
