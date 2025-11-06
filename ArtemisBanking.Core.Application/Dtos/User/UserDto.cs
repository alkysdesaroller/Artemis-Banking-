namespace ArtemisBanking.Core.Application.Dtos.User;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}"; 
    public string Cedula { get; set; } = string.Empty;
    public string Email { get; set; }  = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
        
    // Para comercios
    public int? CommerceId { get; set; }
    public string CommerceName { get; set; } = string.Empty;
}