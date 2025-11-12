using ArtemisBanking.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{nameof(Roles.Admin)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class LoansController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}