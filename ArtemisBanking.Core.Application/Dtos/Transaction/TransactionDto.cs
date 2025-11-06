namespace ArtemisBanking.Core.Application.Dtos.Transaction;

public class TransactionDto
{
    public int Id { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Beneficiary { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
        
    // Propiedades calculadas para mejor presentación
    public string FormattedAmount => TransactionType == "DÉBITO" 
        ? $"-RD${Amount:N2}" 
        : $"+RD${Amount:N2}";
}