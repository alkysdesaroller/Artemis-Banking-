using ArtemisBanking.Core.Application.Dtos.Login;
using ArtemisBanking.Core.Application.Dtos.User;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface IBaseAccountService
{
    Task<Result<UserDto>> RegisterUser(UserSaveDto saveDto, string? origin, bool? isApi = false);
    Task<Result<UserDto>> EditUser(UserSaveDto saveDto, string? origin, bool? isCreated = false, bool? isApi = false);
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request, bool? isApi = false);
    Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<Result> DeleteAsync(string id);
    Task<Result<UserDto>> GetUserByEmail(string email);
    Task<Result<UserDto>> GetUserById(string id);
    Task<Result<List<UserDto>>> GetUsersByIds(List<string> ids);
    Task<Result<UserDto>> GetUserByUserName(string userName);
    Task<Result<List<UserDto>>> GetAllUser(bool? isActive = true);
    Task<Result> ConfirmAccountAsync(string userId, string token);
}