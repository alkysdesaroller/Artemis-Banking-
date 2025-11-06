namespace ArtemisBanking.Core.Application.Dtos;

public class SavingAccountDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string UserCedula { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsMainAccount { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
        
    // Propiedades calculadas
    public string AccountType => IsMainAccount ? "Principal" : "Secundaria";
}