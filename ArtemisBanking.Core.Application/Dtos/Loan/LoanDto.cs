namespace ArtemisBanking.Core.Application.Dtos.Loan;

public class LoanDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string UserCedula { get; set; } = string.Empty;
    public string LoanNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal RemainingAmount { get; set; }
    public int DurationMonths { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int TotalInstallments { get; set; }
    public int PaidInstallments { get; set; }
    public bool IsOverdue { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
        
    // Propiedades calculadas
    public bool IsUpToDate => !IsOverdue;
    public int PendingInstallments => TotalInstallments - PaidInstallments;
}