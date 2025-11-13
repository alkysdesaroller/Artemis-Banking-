using System.Transactions;
using ArtemisBanking.Core.Domain.Common.Enums;
using TransactionStatus = ArtemisBanking.Core.Domain.Common.Enums.TransactionStatus;

namespace ArtemisBanking.Core.Domain.Entities;

public class Transaction
{
    public required int Id { get; set; }
    public required decimal Amount { get; set; }
    public required string CreatedById { get; set; }
    public required TransactionType Type { get; set; }
    public required string Beneficiary  { get; set; } // En este contexto, es el destino de la transaccion
    public required string Origin  { get; set; }
    public required DateTime Date { get; set; }
    public required TransactionStatus  Status { get; set; }
    public required TransactionSubType SubType { get; set; }
}