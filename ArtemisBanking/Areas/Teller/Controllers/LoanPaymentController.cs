using ArtemisBanking.Core.Application.Dtos.Transaction.Teller;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Loan;
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
public class LoanPaymentController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly ILoanService _loanService;
    private readonly UserManager<AppUser> _userManager;

    public LoanPaymentController(
        ITransactionService transactionService,
        ISavingAccountService savingAccountService,
        ILoanService loanService,
        UserManager<AppUser> userManager)
    {
        _transactionService = transactionService;
        _savingAccountService = savingAccountService;
        _loanService = loanService;
        _userManager = userManager;
    }

    // PASO 1: Página inicial
    public IActionResult Index()
    {
        return View(new LoanPaymentViewModel());
    }

    // PASO 2: Validar y mostrar pantalla de confirmación previa
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoanPaymentViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Verificar cuenta
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
            ModelState.AddModelError("", "Saldo insuficiente en la cuenta para realizar el pago.");
            return View(model);
        }

        // Verificar préstamo
        var loanResult = await _loanService.GetByIdAsync(model.LoanNumber);
        if (loanResult.IsFailure || loanResult.Value == null)
        {
            ModelState.AddModelError("", "El préstamo especificado no existe.");
            return View(model);
        }

        var loan = loanResult.Value;

        // Obtener nombre del dueño del préstamo
        var client = await _userManager.FindByIdAsync(loan.ClientId);
        var clientName = client != null ? $"{client.FirstName} {client.LastName}" : "Desconocido";

        // Preparar modelo de confirmación previa
        var confirmVm = new ConfirmLoanPaymentViewModel
        {
            Payment = new LoanPaymentViewModel
            {
                SourceAccountNumber = model.SourceAccountNumber,
                LoanNumber = model.LoanNumber,
                Amount = model.Amount,
                Description = model.Description
            },
            LoanOwnerName = clientName
        };

        return View("Confirm", confirmVm);
    }

    // PASO 3: Procesar el pago del préstamo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessPayment(ConfirmLoanPaymentViewModel model)
    {
        var teller = await _userManager.GetUserAsync(User);
        if (teller == null)
        {
            ModelState.AddModelError("", "No se pudo identificar al cajero.");
            return View("Confirm", model);
        }

        var dto = new TellerLoanPaymentDto
        {
            SourceAccountNumber = model.Payment.SourceAccountNumber,
            LoanNumber = model.Payment.LoanNumber,
            Amount = model.Payment.Amount,
            TellerId = teller.Id,
        };

        var result = await _transactionService.ProcessTellerLoanPaymentAsync(dto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Confirm", model);
        }

        return RedirectToRoute(new { controller = "Withdrawal", action = "Index" });
    }

}
