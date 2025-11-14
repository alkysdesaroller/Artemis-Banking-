using ArtemisBanking.Core.Application.Dtos.Transaction.Teller;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Teller;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Extensions;
using ArtemisBanking.Infrastructure.Identity.Entities;
using AutoMapper;
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
    private readonly IMapper _mapper;

    public CreditCardPaymentController(
        ITransactionService transactionService,
        ISavingAccountService savingAccountService,
        ICreditCardService creditCardService,
        UserManager<AppUser> userManager,
        IMapper mapper)
    {
        _transactionService = transactionService;
        _savingAccountService = savingAccountService;
        _creditCardService = creditCardService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        return View(new CreditCardPaymentViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(CreditCardPaymentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Verificar que la cuenta origen existe
        var accountResult = await _savingAccountService.GetByIdAsync(model.SourceAccountNumber);
        if (accountResult.IsFailure || accountResult.Value == null)
        {
            ModelState.AddModelError("", "La cuenta origen especificada no existe.");
            return View(model);
        }

        var account = accountResult.Value;
        if (!account.IsActive)
        {
            ModelState.AddModelError("", "La cuenta origen especificada está inactiva.");
            return View(model);
        }

        // Verificar que hay suficiente saldo
        if (account.Balance < model.Amount)
        {
            ModelState.AddModelError("", "La cuenta no tiene suficiente saldo para realizar este pago.");
            return View(model);
        }

        // Verificar que la tarjeta de crédito existe
        var creditCardResult = await _creditCardService.GetByIdAsync(model.CreditCardNumber);
        if (creditCardResult.IsFailure || creditCardResult.Value == null)
        {
            ModelState.AddModelError("", "La tarjeta de crédito especificada no existe.");
            return View(model);
        }

        // Obtener el ID del cajero actual
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            ModelState.AddModelError("", "No se pudo identificar al cajero.");
            return View(model);
        }

        // Crear el DTO para el pago de tarjeta de crédito
        var paymentDto = new TellerCreditCardPaymentDto
        {
            SourceAccountNumber = model.SourceAccountNumber,
            CreditCardNumber = model.CreditCardNumber,
            Amount = model.Amount,
            TellerId = user.Id
        };

        // Procesar el pago
        var result = await _transactionService.ProcessTellerCreditCardPaymentAsync(paymentDto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View(model);
        }

        // Redirigir a la página de confirmación
        return RedirectToAction("Confirm", new { transactionId = result.Value.Id });
    }

    public async Task<IActionResult> Confirm(int transactionId)
    {
        var transactionResult = await _transactionService.GetByIdAsync(transactionId);
        if (transactionResult.IsFailure || transactionResult.Value == null)
        {
            return RedirectToAction("Index");
        }

        var viewModel = new TransactionConfirmationViewModel
        {
            Transaction = transactionResult.Value,
            TransactionType = "Pago de Tarjeta de Crédito",
            AccountNumber = transactionResult.Value.Origin,
            CreditCardNumber = transactionResult.Value.Beneficiary
        };

        return View(viewModel);
    }
}