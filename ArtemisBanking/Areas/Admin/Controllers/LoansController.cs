using ArtemisBanking.Core.Application;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Loan;
using ArtemisBanking.Core.Domain.Common.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{nameof(Roles.Admin)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class LoansController : Controller
{
    private readonly ILoanService _loanService;
    private readonly IMapper _mapper;
    // GET
    public LoansController(ILoanService loanService, IMapper mapper)
    {
        _loanService = loanService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index(LoanFilterViewModel filters)
    {
        var paginatedDataDto = await _loanService.GetLoansPagedAsync(
            filters.Page,
            filters.PageSize,
            filters.IdentityCardNumber,
            filters.IsCompleted
            );
        var viewmodels = _mapper.Map<List<LoanSummaryViewModel>>(paginatedDataDto.Items);
        var indexViewModel = new LoanIndexViewModel
        {
            Data = new PaginatedData<LoanSummaryViewModel>(viewmodels, paginatedDataDto.Pagination),
            Filter = filters,
        };
        return View(indexViewModel);
    }


}