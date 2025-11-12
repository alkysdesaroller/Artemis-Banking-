using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.ViewModels.Loan;

namespace ArtemisBanking.Core.Application.ViewModels.LoanInstallment;

public class LoanInstallmentViewModel
{
    public required int Id { get; set; }
    public required DateTime PaymentDay { get; set; }
    public required string LoanId { get; set; }
    public required decimal Amount { get; set; }
    public required bool IsPaid { get; set; }
    public required bool IsDue { get; set; } // si esta atrasada
    public LoanViewModel? Loan { get; set; }
}