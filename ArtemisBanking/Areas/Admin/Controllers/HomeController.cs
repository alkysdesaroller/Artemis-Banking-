using ArtemisBanking.Core.Application.Dtos.Dashboard;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.AdminHome;
using ArtemisBanking.Core.Application.ViewModels.Teller;
using ArtemisBanking.Core.Domain.Common.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{nameof(Roles.Admin)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class HomeController : Controller
{
    
    private readonly IDashboardService _dashboardService;

    public HomeController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var dashboardDto = await _dashboardService.GetAdminDashboard();
        return View(new DashboardAdminViewModel
        {
            TotalTransactions = dashboardDto.TotalTransactions,
            TodayTransactions = dashboardDto.TodayTransactions,
            TotalPayments = dashboardDto.TotalPayments,
            TodayPayments = dashboardDto.TodayPayments,
            ActiveClients = dashboardDto.ActiveClients,
            InactiveClients = dashboardDto.InactiveClients,
            TotalProducts = dashboardDto.TotalProducts,
            ActiveLoans = dashboardDto.ActiveLoans,
            ActiveCreditCards = dashboardDto.ActiveCreditCards,
            ActiveSavingsAccounts = dashboardDto.ActiveSavingsAccounts,
            AverageClientDebt = dashboardDto.AverageClientDebt,
        });
    }
}