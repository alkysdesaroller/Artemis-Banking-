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
public class ThirdPartyTransactionController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public ThirdPartyTransactionController(
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
        return View(new ThirdPartyTransactionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ThirdPartyTransactionViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Verificar que las cuentas no sean la misma
        if (model.SourceAccountNumber == model.DestinationAccountNumber)
        {
            ModelState.AddModelError("", "La cuenta origen y destino no pueden ser la misma.");
            return View(model);
        }

        // Verificar que la cuenta origen existe
        var sourceAccountResult = await _savingAccountService.GetByIdAsync(model.SourceAccountNumber);
        if (sourceAccountResult.IsFailure || sourceAccountResult.Value == null)
        {
            ModelState.AddModelError("", "La cuenta origen especificada no existe.");
            return View(model);
        }

        var sourceAccount = sourceAccountResult.Value;
        if (!sourceAccount.IsActive)
        {
            ModelState.AddModelError("", "La cuenta origen especificada está inactiva.");
            return View(model);
        }

        // Verificar que hay suficiente saldo
        if (sourceAccount.Balance < model.Amount)
        {
            ModelState.AddModelError("", "La cuenta origen no tiene suficiente saldo para realizar esta transacción.");
            return View(model);
        }

        // Verificar que la cuenta destino existe
        var destinationAccountResult = await _savingAccountService.GetByIdAsync(model.DestinationAccountNumber);
        if (destinationAccountResult.IsFailure || destinationAccountResult.Value == null)
        {
            ModelState.AddModelError("", "La cuenta destino especificada no existe.");
            return View(model);
        }

        var destinationAccount = destinationAccountResult.Value;
        if (!destinationAccount.IsActive)
        {
            ModelState.AddModelError("", "La cuenta destino especificada está inactiva.");
            return View(model);
        }

        // Obtener el ID del cajero actual
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            ModelState.AddModelError("", "No se pudo identificar al cajero.");
            return View(model);
        }

        // Crear el DTO para la transacción
        var transactionDto = new TellerTransactionDto
        {
            SourceAccountNumber = model.SourceAccountNumber,
            DestinationAccountNumber = model.DestinationAccountNumber,
            Amount = model.Amount,
            TellerId = user.Id
        };

        // Procesar la transacción
        var result = await _transactionService.ProcessTellerTransactionAsync(transactionDto);

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
            TransactionType = "Transferencia a Terceros",
            AccountNumber = transactionResult.Value.Origin,
            DestinationAccountNumber = transactionResult.Value.Beneficiary
        };

        return View(viewModel);
    }
}