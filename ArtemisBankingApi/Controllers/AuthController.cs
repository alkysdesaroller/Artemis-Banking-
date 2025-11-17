using ArtemisBanking.Core.Application.Dtos.Login;
using ArtemisBanking.Core.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAccountServiceForWebApi accountService) : ControllerBase
{
    /// <summary>
    /// Autentica un usuario y devuelve un token JWT
    /// </summary>
    /// <param name="loginDto">Credenciales de login</param>
    /// <returns>Token JWT</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await accountService.AuthenticateAsync(loginDto);

        if (result.IsFailure)
        {
            return BadRequest(new { error = result.GeneralError ?? "Error de autenticación" });
        }

        return Ok(new { token = result.Value });
    }
}

