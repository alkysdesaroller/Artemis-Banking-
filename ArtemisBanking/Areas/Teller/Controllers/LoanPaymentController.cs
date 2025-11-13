using ArtemisBanking.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Teller.Controllers;

[Area ("Teller")]
[Authorize(Roles = $"{nameof(Roles.Atm)}")]
public class LoanPaymentController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}