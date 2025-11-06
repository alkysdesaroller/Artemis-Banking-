namespace ArtemisBanking.Core.Application.Dtos.User;

public class SaveUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string UserType { get; set; } = string.Empty;
    public decimal InitialAmount { get; set; }

    // Para comercios
    public int? CommerceId { get; set; }
}