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
public class WithdrawalController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly ISavingAccountService _savingAccountService;
    private readonly UserManager<AppUser> _userManager;

    public WithdrawalController(
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
        return View(new WithdrawalViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(WithdrawalViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

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

        if (account.Balance < model.Amount)
        {
            ModelState.AddModelError("", "La cuenta no tiene suficiente saldo para este retiro.");
            return View(model);
        }

        var client = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == account.ClientId);
        if (client == null)
        {
            ModelState.AddModelError("", "No se encontró al cliente de esta cuenta.");
            return View(model);
        }

        var confirmModel = new ConfirmWithdrawalViewModel
        {
            Withdrawal = new WithdrawalViewModel
            {
                AccountNumber = model.AccountNumber,
                Amount = model.Amount,
                Description = model.Description
            },
            ClientName = $"{client.FirstName} {client.LastName}"
        };

        return View("Confirm", confirmModel);
    }

    public IActionResult Confirm(ConfirmWithdrawalViewModel model)
    {
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ProcessWithdrawal(ConfirmWithdrawalViewModel model)
    {
        var dto = new WithdrawalDto
        {
            AccountNumber = model.Withdrawal.AccountNumber,
            Amount = model.Withdrawal.Amount,
            TellerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? ""
        };

        var result = await _transactionService.ProcessWithdrawalAsync(dto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Index", new WithdrawalViewModel());
        }

        return RedirectToRoute(new { controller = "Withdrawal", action = "Index" });
    }
}
