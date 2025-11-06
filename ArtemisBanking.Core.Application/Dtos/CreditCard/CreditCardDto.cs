namespace ArtemisBanking.Core.Application.Dtos.CreditCard;

public class CreditCardDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string UserCedula { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string LastFourDigits => CardNumber?.Length >= 4 ? CardNumber.Substring(CardNumber.Length - 4) : CardNumber!;
    public decimal CreditLimit { get; set; }
    public decimal CurrentDebt { get; set; }
    public decimal AvailableCredit => CreditLimit - CurrentDebt;
    public string ExpirationDate { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}