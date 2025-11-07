
using ArtemisBanking.Core.Application.Dtos.Commerce;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface ICommerceService : IGenericService<int , CommerceDto> 
{
    //Task<Result> ChangeStatusAsync(int id, bool status);
    //Task<Result<bool>> RNCExistsAsync(string rnc, int? excludeId = null);
    //Task<Result<bool>> HasAssociatedUserAsync(int commerceId);
    
    /*
     * --Task<PagedResult<CommerceDto>> GetPagedAsync(int page = 1, int pageSize = 20);--
     * 
     * este es el metodo que se utlizara para desplegar los commercios de 20 en 20, no implemento,
     * ya que tengo que crear la clase PagedResult que sera un clase que extiende de Result, esto para la muestra de los reultados
     * y que por defecto cree una lista o devuelva una lista con la metadata de navegacion.
     *
     * Att: Alna
     */
}