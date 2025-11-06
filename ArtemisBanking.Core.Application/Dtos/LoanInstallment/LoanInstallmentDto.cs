namespace ArtemisBanking.Core.Application.Dtos.LoanInstallment;

public class LoanInstallmentDto
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public string LoanNumber { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal InstallmentAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount => InstallmentAmount - PaidAmount;
    public bool IsPaid { get; set; }
    public bool IsOverdue { get; set; }
    public DateTime? PaymentDate { get; set; }
    public int InstallmentNumber { get; set; }
        
    // Propiedades calculadas
    public string Status
    {
        get
        {
            if (IsPaid) return "Pagada";
            if (IsOverdue) return "Atrasada";
            return "Pendiente";
        }
    }
}