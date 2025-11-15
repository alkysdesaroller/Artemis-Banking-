using System.Security.Claims;
using ArtemisBanking.Core.Application;
using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Loan;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Extensions;
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

    /*
    public async Task<IActionResult> CreateLoan()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoan()
    {
    }

    public async Task<IActionResult> Details(string loanId)
    {
    }*/
    

    public async Task<IActionResult> SelectUser(string selectedClientId)
    {
        return View("CreateLoan", new CreateLoanViewModel
        {
            ClientId = selectedClientId,
            TermMonth = 6,
            AnualRate = 0m,
            Amount = 0
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateLoan(CreateLoanViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("CreateLoan", model);
        }

        var loanDto = new LoanDto
        {
            ClientId = model.ClientId,
            ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            Amount = model.Amount,
            TermMonths = 0,
            AnualRate = 0,
            Completed = false,
            IsDue = false,
            CreatedAt = DateTime.Now,
        };
        
        var loanResult = await _loanService.AddAsync(loanDto);
        if (loanResult.IsFailure)
        {
            this.SendValidationErrorMessages(loanResult);    
            return View("CreateLoan", model);
        }
        
        return View(model);
    }
}