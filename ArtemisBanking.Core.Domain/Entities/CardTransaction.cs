using ArtemisBanking.Core.Domain.Common.Enums;

namespace ArtemisBanking.Core.Domain.Entities;

public class CardTransaction
{
    public required int Id { get; set; }
    
    public required string CreditCardNumber { get; set; }
    public required decimal Amount { get; set; }
    public required int CommerceId { get; set; }
    public required DateTime Date { get; set; } = DateTime.Now;
    public required CreditCardTransactionStatus Status { get; set; }
    
    public Commerce? Commerce { get; set; }
    public CreditCard? CreditCard { get; set; }
}