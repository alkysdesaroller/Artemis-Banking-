using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ArtemisBanking.Core.Application.Dtos.Login;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.ViewModels.Login;
using ArtemisBanking.Core.Domain.Common.Enums;
using ArtemisBanking.Extensions;
using ArtemisBanking.Infrastructure.Identity.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ArtemisBanking.Controllers;

public class LoginController : Controller
{
    private readonly IMapper _mapper;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly UserManager<AppUser> _userManager;

    public LoginController(IMapper mapper, IAccountServiceForWebApp accountServiceForWebApp, UserManager<AppUser> userManager)
    {
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
        _userManager = userManager;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var role =  await _userManager.GetRolesAsync(user);
            return await RedirectToHomeByRole(role[0]);
        }
        
        return View(new LoginViewModel() {UserName = "",  Password = ""});
    }
    
    [HttpPost]
    public async Task<IActionResult> LogIn(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Index",loginViewModel);
        }
        
        var loginDto = _mapper.Map<LoginDto>(loginViewModel);
        var result = await _accountServiceForWebApp.AuthenticateAsync(loginDto);
        
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Index",loginViewModel);
        }

        var user = result.Value!;
        return await RedirectToHomeByRole(user.Role);
    }

    public async Task<IActionResult> LogOut()
    {
        await _accountServiceForWebApp.SignOutAsync();
        return RedirectToRoute(new { controller = "Login", action = "Index" });
    }


    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordRequestViewModel() {UserName = ""} );
    }
    
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }
        string origin = Request?.Headers?.Origin.ToString() ?? string.Empty;
        ForgotPasswordRequestDto dto = new() { UserName = vm.UserName,Origin = origin};
        var result = await _accountServiceForWebApp.ForgotPasswordAsync(dto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View(vm);
        }          

        return RedirectToRoute(new { controller = "Login", action = "Index" });
    }

    public IActionResult ResetPassword(string userId, string token)
    {           
        return View(new ResetPasswordRequestViewModel()
        {
            Id = userId,
            Token = token,
            Password = "",
            ConfirmPassword = "",
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }  

        ResetPasswordRequestDto dto = new()
        {
            UserId = vm.Id,
            Password = vm.Password,
            Token = vm.Token
        };

        var result = await _accountServiceForWebApp.ResetPasswordAsync(dto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View(vm);
        }

        return RedirectToRoute(new { controller = "Login", action = "Index" });
    }



    private async Task<IActionResult> RedirectToHomeByRole(string role)
    {
        if (Roles.TryParse(role, out Roles userRole))
        {
            switch (userRole)
            {
                case Roles.Admin:
                    return RedirectToRoute(new { area="Admin" ,controller = "Home", action = "Index" });
                    break;

                case Roles.Atm:
                    return RedirectToRoute(new { area="Cashier" ,controller = "Home", action = "Index" });
                
                case Roles.Client:
                    return RedirectToRoute(new { area="Client" ,controller = "Home", action = "Index" });
           }
        }
        
        await _accountServiceForWebApp.SignOutAsync();
        ViewBag.Message = "Los usuarios de rol comercio solamente pueden acceder por la API";
        return View("Index", new LoginViewModel
        {
            UserName = "",
            Password = ""
        });
    }

}