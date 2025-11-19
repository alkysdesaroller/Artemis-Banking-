namespace ArtemisBanking.Core.Application.ViewModels.Teller;

public class ConfirmThirdPartyTransactionViewModel
{
    public ThirdPartyTransactionViewModel Transaction { get; set; }

    // Nombre del cliente destino (según documento del cajero)
    public string DestinationClientName { get; set; }
}
