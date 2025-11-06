namespace ArtemisBanking.Core.Application.Dtos.User;

public class DashboardDto
{
    // Para Administrador
    public int TotalTransactions { get; set; }
    public int TodayTransactions { get; set; }
    public int TotalPayments { get; set; }
    public int TodayPayments { get; set; }
    public int ActiveClients { get; set; }
    public int InactiveClients { get; set; }
    public int TotalFinancialProducts { get; set; }
    public int ActiveLoans { get; set; }
    public int ActiveCreditCards { get; set; }
    public int TotalSavingAccounts { get; set; }
    public decimal AverageDebtPerClient { get; set; }

    // Para Cajero
    public int TellerTodayTransactions { get; set; }
    public int TellerTodayPayments { get; set; }
    public int TellerTodayDeposits { get; set; }
    public int TellerTodayWithdrawals { get; set; }
}