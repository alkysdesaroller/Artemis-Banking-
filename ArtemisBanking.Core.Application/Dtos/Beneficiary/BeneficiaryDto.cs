namespace ArtemisBanking.Core.Application.Dtos.Beneficiary;

public class BeneficiaryDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BeneficiaryName { get; set; } = string.Empty;
    public string BeneficiaryLastName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}