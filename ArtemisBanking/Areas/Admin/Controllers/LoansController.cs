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
    private readonly IRiskService _riskService;
    private readonly IMapper _mapper;
    // GET
    public LoansController(ILoanService loanService, IMapper mapper, IRiskService riskService)
    {
        _loanService = loanService;
        _mapper = mapper;
        _riskService = riskService;
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



    public async Task<IActionResult> Details(string loanId)
    {
        var amortizationTableResult = await _loanService.GetAmortizationTableAsync(loanId);
        var loanWithInstallments = amortizationTableResult.Value!;
        if (amortizationTableResult.IsFailure)
        {
            this.SendValidationErrorMessages(amortizationTableResult);
            return View("Index");
        }
        return View(_mapper.Map<LoanViewModel>(loanWithInstallments));
    }
    

    public async Task<IActionResult> SelectUser(string? identityCardNumber = null)
    {
        // Trae los clientes activos sin préstamo activo
        var usersWithDebtResult = await _loanService.GetClientsWithoutActiveLoan(identityCardNumber);
        var viewmodels = _mapper.Map<List<ClientsWithDebtViewModel>>(usersWithDebtResult.Value);
        return View(viewmodels);
        
    }
    
    [HttpPost]
    public async Task<IActionResult> SelectUserPost(string selectedClientId)
    {

        return View("CreateLoan", new CreateLoanViewModel
        {
            ClientId = selectedClientId,
            TermMonth = 6,
            AnualRate = 0,
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
            TermMonths = model.TermMonth,
            AnualRate = model.AnualRate,
            Completed = false, // un prestamo no completado esta activo
            IsDue = false,
            CreatedAt = DateTime.Now,
        };
        
        var averageClientDebtOfSystem = await _riskService.GetSystemAverageClientDebtAsync();
        var totalDebtOfUser = await _riskService.CalculateClientTotalDebt(model.ClientId);
        var capitalWithInterests = _riskService.CalculateTotalLoanDebt(model.Amount, model.AnualRate, model.TermMonth);

        if (totalDebtOfUser > averageClientDebtOfSystem)
        {
            ViewBag.Message = "Este cliente se considera de alto riesgo," +
                              " ya que su deuda actual supera el promedio del sistema" +
                              "¿Esta seguro de que quiere a singarle un préstamo?";
            return View("HighRiskClientWarning", model);
        }

        if ((totalDebtOfUser + capitalWithInterests) > averageClientDebtOfSystem)
        {
            ViewBag.Message = "Asignar este préstamo convertirá al cliente en un cliente de alto riesgo," +
                              " ya que su deuda superará el \numbral promedio del sistema." +
                              " ¿Esta seguro de que quiere asignarle un préstamo?";
            return View("HighRiskClientWarning", model);
        }
        
        var loanResult = await _loanService.AddAsync(loanDto);
        if (loanResult.IsFailure)
        {
            this.SendValidationErrorMessages(loanResult);    
            return View("CreateLoan", model);
        }
        
        return RedirectToRoute(new { controller = "loans", action = "Index" });
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateLoanFromWarning(CreateLoanViewModel model)
    {
        // No hay que validar aqui, fue redireccionado por el warning
        var loanDto = new LoanDto
        {
            ClientId = model.ClientId,
            ApprovedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "",
            Amount = model.Amount,
            TermMonths = model.TermMonth,
            AnualRate = model.AnualRate,
            Completed = false, // un prestamo no completado esta activo
            IsDue = false,
            CreatedAt = DateTime.Now,
        };
       
        var loanResult = await _loanService.AddAsync(loanDto);
        if (loanResult.IsFailure)
        {
            this.SendValidationErrorMessages(loanResult);    
            return View("Index");
        }

        return RedirectToRoute(new { controller = "loans", action = "Index" });
    }

    public async Task<IActionResult> Edit(string loanId)
    {
        var loan = await _loanService.GetByIdAsync(loanId);
        
        if (loan.IsFailure)
        {
            this.SendValidationErrorMessages(loan);
            return View("Index" );
        }

        return View("EditLoan", new EditLoanViewModel
        {
            LoanId = loan.Value!.Id,
            AnualRate = loan.Value!.AnualRate
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> Edit(EditLoanViewModel model)
    {
        var loan = await _loanService.UpdateInterestRateAsync(model.LoanId, model.AnualRate);
        if (loan.IsFailure)
        {
            this.SendValidationErrorMessages(loan);
            return View("Index");
        }
        return RedirectToRoute(new { controller = "loans", action = "Index" });
   }
    
}