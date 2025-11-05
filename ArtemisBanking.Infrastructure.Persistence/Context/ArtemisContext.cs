using System.Reflection;
using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Infrastructure.Persistence.Context;

public class ArtemisContext : DbContext
{
    
    public DbSet<Beneficiary>  Beneficiaries { get; set; }
    public DbSet<CardTransaction> CardTransactions { get; set; }
    public DbSet<Commerce> Commerces { get; set; }
    public DbSet<CreditCard> CreditCards { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanInstallment> LoanInstallments { get; set; }
    public DbSet<SavingAccount> SavingAccounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    
    public ArtemisContext(DbContextOptions<ArtemisContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasSequence<long>("SeqLoanAndAccountNumbersID")
            .StartsAt(1)
            .IncrementsBy(1)
            .HasMax(999999999)
            .IsCyclic(false);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
}