using ArtemisBanking.Core.Application;
using ArtemisBanking.Core.Application.Dtos.SavingAccount;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArtemisBankingApi.Controllers;

[Authorize(Roles = $"{nameof(Roles.Admin)}")]
public class UsersController : BaseApiController
{
    private readonly IAccountServiceForWebApi _accountServiceForWebApi;
    private readonly ISavingAccountService _savingAccountService;
    private readonly IMapper _mapper;

    public UsersController(IAccountServiceForWebApi accountServiceForWebApi, IMapper mapper, ISavingAccountService savingAccountService)
    {
        _accountServiceForWebApi = accountServiceForWebApi;
        _mapper = mapper;
        _savingAccountService = savingAccountService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedData<UserDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers([FromQuery] UsersApiFiltersDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst("uid")?.Value ?? "";
            var users =
                await _accountServiceForWebApi.GetAllTheUsersThatArentCommercesPaginated(
                    userId,
                    dto.Page,
                    dto.PageSize,
                    dto.Role
                );
            return Ok(users);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("commerce")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedData<UserDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCommerces([FromQuery] CommerceFiltersDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst("uid")?.Value ?? "";

            var usersCommercesResult = await _accountServiceForWebApi.GetAllUserOfRole(Roles.Commerce, false);
            if (usersCommercesResult.IsFailure)
            {
                return BadRequest(usersCommercesResult.GeneralError);
            }

            var paginatedCommerces = PaginatedData<UserDto>.Create(usersCommercesResult.Value!, dto.Page, dto.PageSize);
            return Ok(paginatedCommerces);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser(CreateApiUserDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(ModelState);
            }

            if (dto.Role == Roles.Commerce)
            {
                return BadRequest("No se pueden crear comercios desde este endpoint");
            }
            
            var userNameExists = await _accountServiceForWebApi.ThisUsernameExists(dto.UserName);
            var userEmailExists = await _accountServiceForWebApi.ThisEmailExists(dto.Email);
            if (userEmailExists || userNameExists)
            {
                return Conflict("Este nombre de usuario o email ya existen");
            }

            var adminInSessionId = User.FindFirst("uid")?.Value ?? "";
            var userToCreate = new UserSaveDto
            {
                Id = "",
                IdentityCardNumber = dto.IdentityCardNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                Password = dto.Password,
                Role = dto.Role.ToString()
            };
            
            var createUserResult = await _accountServiceForWebApi.RegisterUser(userToCreate, "", true);

            if (createUserResult.IsFailure)
            {
                return BadRequest400WithErrorMessagesFromResult(createUserResult);
            }
            var createdUser = createUserResult.Value!;

            if (dto.Role == Roles.Client)
            {
                var accountResult =
                    await _savingAccountService.CreateNewSavingAccountCard(
                        adminInSessionId,
                        createdUser.Id,
                        dto.InitialAmount ?? 0,
                        true);
                
                if (accountResult.IsFailure)
                {
                    return BadRequest400WithErrorMessagesFromResult(createUserResult);
                }
            }

            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("commerce/{commerceId:int}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateCommerceUser([FromQuery] int commerceId ,[FromBody]CreateApiCommerceDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(ModelState);
            }

            
            var userNameExists = await _accountServiceForWebApi.ThisUsernameExists(dto.UserName);
            var userEmailExists = await _accountServiceForWebApi.ThisUsernameExists(dto.Email);
            if (userEmailExists || userNameExists)
            {
                return Conflict("Este nombre de usuario o email ya existen");
            }
            
            var commerceIdExists = await _accountServiceForWebApi.ThisCommerceIdExists(commerceId);
            if (commerceIdExists)
            {
                return Conflict("Este comercio ya esta registrado");
            }
            
            var adminInSessionId = User.FindFirst("uid")?.Value ?? "";
            var userToCreate = new UserSaveDto
            {
                Id = "",
                IdentityCardNumber = dto.IdentityCardNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.UserName,
                Password = dto.Password,
                Role = nameof(Roles.Commerce),
            };
            
            var createUserResult = await _accountServiceForWebApi.RegisterUser(userToCreate, "", true);
            if (createUserResult.IsFailure)
            {
                return BadRequest400WithErrorMessagesFromResult(createUserResult);
            }

            var createdUser = createUserResult.Value!;

            var accountResult = await _savingAccountService.CreateNewSavingAccountCard(
                adminInSessionId,
                createdUser.Id,
                dto.InitialAmount,
                true);

            if (accountResult.IsFailure)
            {
                return BadRequest400WithErrorMessagesFromResult(createUserResult);
            }

            return Created();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser([FromQuery] string id ,[FromBody] UpdateApiUserDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(ModelState);
            }
            
            var userNameExists = await _accountServiceForWebApi.ThisUsernameExists(dto.UserName, id);
            var userEmailExists = await _accountServiceForWebApi.ThisUsernameExists(dto.Email, id);
            if (userEmailExists || userNameExists)
            {
                return Conflict("Este nombre de usuario o email ya existen");
            }

            var user = await _accountServiceForWebApi.GetUserById(id);
            if (user is null)
            {
                return NotFound();
            }

            var update = _mapper.Map<UserSaveDto>(dto);
            update.Role = user.Role;
            await _accountServiceForWebApi.EditUser(update, "", true);

            if (update.Role == nameof(Roles.Client))
            {
                var mainAccountResult = await _savingAccountService.GetMainAccountByUserIdAsync(id);
                if (mainAccountResult.IsFailure)
                {
                    return BadRequest400WithErrorMessagesFromResult(mainAccountResult);
                }
                await _savingAccountService.DepositToAccountAsync(mainAccountResult.Value!.Id, dto.AdditionalAmount ?? 0);
            }
            
            return NoContent();

        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EditUser([FromQuery] string id, [FromBody] ChangeApiUserStatusDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(dto);
            }
        
            var adminInSessionId = User.FindFirst("uid")?.Value ?? "";

            if (id == adminInSessionId)
            {
                return Forbid();
            }
            
            var user = await _accountServiceForWebApi.GetUserById(id);
            if (user is null)
            {
                return NotFound();
            }

            await _accountServiceForWebApi.SetStateOnUser(id, dto.Status);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserApiWithDetailsDto))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserWithDetails([FromQuery] string id)
    {
     
        var user =  await _accountServiceForWebApi.GetUserById(id);
        if (user is null)
        {
            return NotFound();
        }
        var mainAccount = await _savingAccountService.GetMainAccountByUserIdAsync(id);
        var userDetails = new UserApiWithDetailsDto
        {
            Id = user.Id,
            IdentityCardNumber = user.IdentityCardNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserName = user.UserName,
            Role = user.Role,
            MainAccount = new SimpleSavingAccountApiDto
            {
                AccountNumber = mainAccount.Value!.Id, // Los Ids son los numeros de cuenta.
                Balance = mainAccount.Value!.Balance,
            }
        };
        
        return  Ok(userDetails);
    }
}