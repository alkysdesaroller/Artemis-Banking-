using ArtemisBanking.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Cashier.Controllers;

[Area("Cashier")]
[Authorize(Roles = $"{nameof(Roles.Atm)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}