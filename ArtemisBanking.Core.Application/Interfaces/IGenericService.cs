using System.Linq.Expressions;

namespace ArtemisBanking.Core.Application.Interfaces;

public interface IGenericService<TEntity, in TSaveDto, TDto>
where  TEntity : class
{
    Task<TDto> Add(TSaveDto dto);
    Task Update(TSaveDto dto, int id);
    Task Delete(int id);
    Task<TDto> GetById(int id);
    Task<List<TDto>>  GetAll();
    Task<List<TDto>> GetAllWithInclude(List<string> properties);
    Task<bool> Exists(Expression<Func<TEntity, bool>> predicate);  
}