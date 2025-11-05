namespace ArtemisBanking.Core.Domain.Entities;

public class SavingAccount
{
    public required string Id { get; set; }
    public required string ClientId { get; set; } // Es un UserId
    public required decimal Balance { get; set; } // Saldo de la cuenta
    public required DateTime CreatedAt { get; set; } = DateTime.Now;
    public required string AssignedByUserId { get; set; }
    public required bool IsPrincipalAccount { get; set; }
    public required bool IsActive { get; set; }
}