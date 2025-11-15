namespace ArtemisBanking.Core.Application.Enums;

public enum EmailType
{
    LoanApproved,
    LoanRateUpdated,
    LoanDisbursement,
    LoanPayment,
    
    CreditCardPayment,
    CashAdvance,
    
    ExpressTransferSender,
    ExpressTransferReceiver,
    
    BeneficiaryTransferSender,
    BeneficiaryTransferReceiver
}