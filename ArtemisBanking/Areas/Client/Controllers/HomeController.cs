using ArtemisBanking.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = $"{nameof(Roles.Client)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class HomeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}