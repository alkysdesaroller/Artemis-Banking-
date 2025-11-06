namespace ArtemisBanking.Core.Application.Dtos.SavingAccount;

public class WithdrawalDto
{
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}