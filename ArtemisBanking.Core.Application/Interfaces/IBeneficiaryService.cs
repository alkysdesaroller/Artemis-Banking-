using ArtemisBanking.Core.Application.Dtos.Beneficiary;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface IBeneficiaryService : IGenericService<int, BeneficiaryDto>
{
   // Te recomiendo que te centres en generar las interfaces faltantes de los otros servicios.
   // Puedes dejar la creación de métodos especificos para cuando estemos trabajando los modulos cada uno.
    
   // Recuerda que casi todos los metodos deberian de ser Result<> o Result. 
   // Usa Result<> cuando tengas que devolver un objeto junto al resultado, usa Result cuando solamente quieres expresar
   // si la tarea fallo o no.
   
   // Task<Result<List<BeneficiaryDto>>> GetByUserId(string userId);
   // Task<bool> AccountNumberExists(string accountNumber);
}