using System.Security.Claims;
using ArtemisBanking.Core.Application;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Users;
using ArtemisBanking.Core.Domain.Common.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBanking.Controllers;
[Authorize(Roles = $"{nameof(Roles.Admin)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class HomeAdminController : Controller
{
    private readonly IMapper _mapper;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;

    public HomeAdminController(IMapper mapper, IAccountServiceForWebApp accountServiceForWebApp)
    {
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Clients(int page = 1, int pageSize = 20)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var paginatedUsersDto = await _accountServiceForWebApp.GetAllTheUsersThatArentCommercesPaginated(userId, page, pageSize);
        var viewModels = _mapper.Map<List<UserViewModel>>(paginatedUsersDto.Items);
        var paginatedViewModel = new PaginatedData<UserViewModel>(viewModels, paginatedUsersDto.Pagination);
        return View(paginatedViewModel);
    }
}