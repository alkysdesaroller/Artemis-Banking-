using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Teller.Controllers;

[Area("Cashier")]
[Authorize(Roles = $"{nameof(Roles.Atm)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class HomeController(ITransactionService transactionService, UserManager<AppUser> userManager)
    : Controller
{
    private readonly ITransactionService _transactionService = transactionService;
    private readonly UserManager<AppUser> _userManager = userManager;


    public async Task<IActionResult> Dashboard()
    {
        return View();
        /*var user = await _userManager.GetUserAsync(User);
        var tellerId = user!.Id;

        var summaryResult = await transactionService*/
    }

    public IActionResult Index()
    {
        return View();
    }
}