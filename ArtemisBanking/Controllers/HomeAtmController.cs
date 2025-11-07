using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Controllers;

public class HomeAtmController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}