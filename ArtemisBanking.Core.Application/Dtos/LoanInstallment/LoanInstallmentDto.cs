using ArtemisBanking.Core.Application.Dtos.Loan;

namespace ArtemisBanking.Core.Application.Dtos.LoanInstallment;

public class LoanInstallmentDto
{
    public required int Id { get; set; }
    public required DateTime PaymentDay { get; set; }
    public required string LoanId { get; set; }
    public required decimal Amount { get; set; }
    public required bool IsPaid { get; set; }
    public required bool IsDue { get; set; } // si esta atrasada
    public LoanDto? Loan { get; set; }
}