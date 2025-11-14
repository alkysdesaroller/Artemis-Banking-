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

        // Obtener el ID del cajero actual
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            ModelState.AddModelError("", "No se pudo identificar al cajero.");
            return View(model);
        }

        // Crear el DTO para el depósito
        var depositDto = new DepositDto
        {
            AccountNumber = model.AccountNumber,
            Amount = model.Amount,
            TellerId = user.Id
        };

        // Procesar el depósito
        var result = await _transactionService.ProcessDepositAsync(depositDto);

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
            TransactionType = "Depósito",
            AccountNumber = transactionResult.Value.Origin
        };

        return View(viewModel);
    }
}