using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Controllers;

public class HomeClientController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}