using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Loan;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Areas.Admin.ViewComponents;

public class ClientListViewComponent : ViewComponent
{
    private readonly ILoanService _loanService;
    private readonly IMapper _mapper;

    public ClientListViewComponent(ILoanService loanService, IMapper mapper)
    {
        _loanService = loanService;
        _mapper = mapper;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? cedula = null)
    {
        // Trae los clientes activos sin préstamo activo
        var usersWithDebtResult = await _loanService.GetClientsWithoutActiveLoan();
        var viewmodels = _mapper.Map<List<ClientsWithDebtViewModel>>(usersWithDebtResult.Value);
        return View(viewmodels);
    }
}
