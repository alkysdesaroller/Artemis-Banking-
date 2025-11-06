using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Controllers;

public class HomeAdminController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}