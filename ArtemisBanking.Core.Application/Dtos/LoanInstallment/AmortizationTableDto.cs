namespace ArtemisBanking.Core.Application.Dtos.LoanInstallment;

public class AmortizationTableDto
{
    public int LoanId { get; set; }
    public string LoanNumber { get; set; } = string.Empty;
    public decimal LoanAmount { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int TotalInstallments { get; set; }
    public decimal MonthlyPayment { get; set; }
    public List<LoanInstallmentDto>? Installments { get; set; } 
}