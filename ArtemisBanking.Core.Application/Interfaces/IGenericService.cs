using System.Linq.Expressions;
using ArtemisBanking.Core.Domain.Entities;

namespace ArtemisBanking.Core.Application.Interfaces;

// La interfaz realmente solamente usa 2 tipos, la implementacion de la intefaz si usa 3 tipos.
public interface IGenericService<in TKey, TDtoModel>
where TDtoModel: class
{
    Task<Result<TDtoModel>> GetByIdAsync(TKey id);
    Task<Result<List<TDtoModel>>> GetAllAsync();
    Task<Result<TDtoModel>> AddAsync(TDtoModel dto);
    Task<Result<List<TDtoModel>>> AddRangeAsync(List<TDtoModel> dto);
    Task<Result<TDtoModel>> UpdateAsync(TKey id, TDtoModel dto);
    Task<Result> DeleteAsync(TKey id);
    Task<Result<bool>> ExistsAsync(Func<IQueryable<TDtoModel>, bool> predicate);
    
    /* Debemos de borrar este metodo. Usa un TEntity, es decir una clase entidad de la DB.
     * No podemos acceder a la capa de persistencia desde aplicacion. Aun si pudieramos no deberiamos.
     * Opta por crear un metodo que revise si existe una entidad tal cual al llamar al GetAllQueryable del repo
     * 
     * Task<bool> Exists(Expression<Func<TEntity, bool>> predicate);
     */
    
    /*Agregado metodo Exists esta vez accediendo a la informacion a consultar desde 
     * el IQueryable, resolviendo asi el problema de separacion de capas
     * ATT: Alna.
     */
}