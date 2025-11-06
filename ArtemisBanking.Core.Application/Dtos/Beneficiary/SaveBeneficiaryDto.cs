using System.ComponentModel.DataAnnotations;

namespace ArtemisBanking.Core.Application.Dtos.Saves;

public class SaveBeneficiaryDto
{
    public string AccountNumber { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;
}