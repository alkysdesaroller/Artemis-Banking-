using ArtemisBanking.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Controllers;

[Authorize(Roles = $"{nameof(Roles.Atm)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class HomeAtmController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}