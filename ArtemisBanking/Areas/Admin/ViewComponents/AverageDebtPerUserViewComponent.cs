using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Admin.ViewComponents;

[Area("Admin")]
public class AverageDebtPerUserViewComponent : ViewComponent 
{
    private readonly IRiskService  _riskService;

    public AverageDebtPerUserViewComponent(IRiskService riskService)
    {
        _riskService = riskService;
    }

    // GET
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var averageDebtClientResult = await _riskService.GetSystemAverageClientDebtAsync();
        return View(averageDebtClientResult);
    }
}