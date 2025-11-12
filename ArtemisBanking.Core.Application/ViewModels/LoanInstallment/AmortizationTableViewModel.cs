namespace ArtemisBanking.Core.Application.ViewModels.LoanInstallment;

public class AmortizationTableDta
{
    public int LoanId { get; set; }
    public string LoanNumber { get; set; } = string.Empty;
    public decimal LoanAmount { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int TotalInstallments { get; set; }
    public decimal MonthlyPayment { get; set; }
    public List<LoanInstallmentViewModel>? Installments { get; set; } 
}