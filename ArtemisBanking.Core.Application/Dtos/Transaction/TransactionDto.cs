using ArtemisBanking.Core.Domain.Common.Enums;
using TransactionStatus = System.Transactions.TransactionStatus;

namespace ArtemisBanking.Core.Application.Dtos.Transaction;

// Modificado para que sea simetrico con la clase entidad
public class TransactionDto
{
    public required int Id { get; set; }
    public required decimal Amount { get; set; }
    public required TransactionType Type { get; set; }
    public required string Beneficiary  { get; set; } // En este contexto, es el destino de la transaccion
    public required string Origin  { get; set; }
    public required DateTime Date { get; set; }
    public required TransactionStatus Status { get; set; }
}