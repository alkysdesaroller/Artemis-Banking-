namespace ArtemisBanking.Core.Application.Dtos.Login;

public class ResetPasswordDto
{
    public string UserId { get; set; } = string.Empty; 
    public string Token { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; }  = string.Empty;
}