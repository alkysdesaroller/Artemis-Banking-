using System.Security.Claims;
using ArtemisBanking.Core.Application;
using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Login;
using ArtemisBanking.Core.Application.ViewModels.Users;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Controllers;
[Authorize(Roles = $"{nameof(Roles.Admin)}")] // Recuerda leer el apartado de seguridad de los requerimientos
public class HomeAdminController : Controller
{
    private readonly IMapper _mapper;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly ISavingAccountService _savingAccountService;
    private readonly RoleManager<IdentityRole> _roleManager;

    public HomeAdminController(IMapper mapper, IAccountServiceForWebApp accountServiceForWebApp, RoleManager<IdentityRole> roleManager, ISavingAccountService savingAccountService)
    {
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
        _roleManager = roleManager;
        _savingAccountService = savingAccountService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> Users(int page = 1, int pageSize = 20, string? role = null)
    {
        
        await FillRolesViewBag(); 
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var paginatedUsersDto = await _accountServiceForWebApp.GetAllTheUsersThatArentCommercesPaginated(userId, page, pageSize, role);
        var viewModels = _mapper.Map<List<UserViewModel>>(paginatedUsersDto.Items);
        var paginatedViewModel = new PaginatedData<UserViewModel>(viewModels, paginatedUsersDto.Pagination);
        return View(paginatedViewModel);
    }

    public async Task<IActionResult> CreateUser()
    { 
        await FillRolesViewBag(); 
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model, decimal initialAmount = 0)
    {
        if (!ModelState.IsValid)
        {
            await FillRolesViewBag();
            return View(model);
        }
        var userInSessionId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var userToCreate = _mapper.Map<UserSaveDto>(model);
        var origin = HttpContext.Request.Headers["Origin"].FirstOrDefault() ?? "";
        var createUserResult = await _accountServiceForWebApp.RegisterUser(userToCreate, origin);
        
        if (createUserResult.IsFailure)
        {
            await FillRolesViewBag(); 
            this.SendValidationErrorMessages(createUserResult);
            return View(model);
        }

        var createdUser = createUserResult.Value!;

        if (model.Role == nameof(Roles.Client))
        {
            var account = new SavingAccountDto
            {
                ClientId = createdUser!.Id,
                Balance = initialAmount,
                CreatedAt = DateTime.Now,
                AssignedByUserId = userInSessionId,
                IsPrincipalAccount = true,
                IsActive = true
            };
            var accountResult = await _savingAccountService.AddAsync(account);
            if (accountResult.IsFailure)
            {
                await FillRolesViewBag(); 
                this.SendValidationErrorMessages(accountResult);
                return View(model);
            }           
        }
        return RedirectToRoute(new {controller="HomeAdmin", action="Users"});
    }

    private async Task FillRolesViewBag()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        roles.RemoveAll(idtRole => idtRole.Name == nameof(Roles.Commerce));
        ViewBag.Roles = new SelectList(roles,  "Name", "NormalizedName");
    }

    public async Task<IActionResult> EditUser(int userId)
    {
        return View();
    }
    

    public async Task<IActionResult> ChangeUserState(string userId, bool state)
    {
        var userResult = await _accountServiceForWebApp.GetUserById(userId);
        if (userResult.IsFailure)
        {
            this.SendValidationErrorMessages(userResult);
        }
        var user =  userResult.Value!;
        return View(new ChangeUserStateViewModel
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            State = state
        });
    }
    [HttpPost]
    public async Task<IActionResult> ChangeUserState(ChangeUserStateViewModel model)
    {
        var stateResult = await _accountServiceForWebApp.SetStateOnUser(model.UserId, model.State);
        if (stateResult.IsFailure)
        {
            this.SendValidationErrorMessages(stateResult);
            return View(model);
        }
        return RedirectToRoute(new {controller = "HomeAdmin", action = "Users"});
    }
}