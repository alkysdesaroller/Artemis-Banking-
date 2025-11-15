using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Admin.ViewComponents;

[Area("Admin")]
public class AverageDebtPerUserViewComponent : ViewComponent 
{
    private readonly ILoanService _loanService;

    public AverageDebtPerUserViewComponent(ILoanService loanService)
    {
        _loanService = loanService;
    }

    // GET
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var averageDebtClientResult = await _loanService.GetAverageClientDebtAsync();
        if (averageDebtClientResult.IsFailure)
        {
            return View(0);
        }
        return View(averageDebtClientResult.Value);
    }
}