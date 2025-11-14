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
public class HomeController : Controller
{
    private readonly ITransactionService _transactionService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public HomeController(
        ITransactionService transactionService,
        UserManager<AppUser> userManager,
        IMapper mapper)
    {
        _transactionService = transactionService;
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<IActionResult> Dashboard()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Index", "Login", new { area = "" });
        }

        var summaryResult = await _transactionService.GetTellerTransactionSummaryAsync(user.Id);

        var viewModel = new TellerDashboardViewModel
        {
            Summary = summaryResult.IsSuccess ? summaryResult.Value : null,
            TellerName = $"{user.FirstName} {user.LastName}"
        };

        if (summaryResult.IsFailure)
        {
            this.SendValidationErrorMessages(summaryResult);
        }

        return View(viewModel);
    }

    public IActionResult Index()
    {
        return RedirectToAction("Dashboard");
    }
}