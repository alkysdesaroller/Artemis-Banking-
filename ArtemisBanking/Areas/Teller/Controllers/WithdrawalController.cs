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

namespace ArtemisBanking.Areas.Teller.Controllers;

[Area("Teller")]
[Authorize(Roles = $"{nameof(Roles.Atm)}")]
public class WithdrawalController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public WithdrawalController(
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
        return View(new WithdrawalViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(WithdrawalViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Verificar que la cuenta existe
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

        // Verificar que hay suficiente saldo
        if (account.Balance < model.Amount)
        {
            ModelState.AddModelError("", "La cuenta no tiene suficiente saldo para realizar este retiro.");
            return View(model);
        }

        // Obtener el ID del cajero actual
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            ModelState.AddModelError("", "No se pudo identificar al cajero.");
            return View(model);
        }

        // Crear el DTO para el retiro
        var withdrawalDto = new WithdrawalDto
        {
            AccountNumber = model.AccountNumber,
            Amount = model.Amount
        };

        // Procesar el retiro
        var result = await _transactionService.ProcessWithdrawalAsync(withdrawalDto);

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
            TransactionType = "Retiro",
            AccountNumber = transactionResult.Value.Origin
        };

        return View(viewModel);
    }
}