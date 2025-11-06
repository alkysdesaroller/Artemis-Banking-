
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Services;

public class GenericServices<TKey, TEntity, TDtoModel>(IGenericRepository<TKey, TEntity> repository, IMapper mapper)
    : IGenericService<TKey, TDtoModel>
    where TEntity : class
    where TDtoModel : class
{
    public virtual async Task<Result<List<TDtoModel>>> GetAllAsync()
    {
        var dtos = mapper.Map<List<TDtoModel>>(await repository.GetAllAsync());
        return Result<List<TDtoModel>>.Ok(dtos);
    }

    public virtual async Task<Result<TDtoModel>> GetByIdAsync(TKey id)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id);
            if (entity == null)
            {
                return Result<TDtoModel>.Fail("The record was not found");
            }
            return Result<TDtoModel>.Ok(mapper.Map<TDtoModel>(entity));
        }
        catch (Exception e)
        {
            return Result<TDtoModel>.Fail(e.Message);
        }
    }

    public virtual async Task<Result<TDtoModel>> AddAsync(TDtoModel dtoModel)
    {
        try
        {
            TEntity entity = mapper.Map<TEntity>(dtoModel);
            TEntity? returnEntity = await repository.AddAsync(entity);

            TDtoModel dto = mapper.Map<TDtoModel>(returnEntity);
            return Result<TDtoModel>.Ok(dto);
        }
        catch (Exception e)
        {
            return Result<TDtoModel>.Fail(e.Message);
        }
    }

    public virtual async Task<Result<List<TDtoModel>>> AddRangeAsync(List<TDtoModel> dtomodels)
    {
        try
        {
            List<TEntity> entity = mapper.Map<List<TEntity>>(dtomodels);
            List<TEntity> returnEntities = await repository.AddRangeAsync(entity);
            
            List<TDtoModel> dtos = mapper.Map<List<TDtoModel>>(returnEntities);
            return Result<List<TDtoModel>>.Ok(dtos);
        }
        catch (Exception e)
        {
            return Result<List<TDtoModel>>.Fail(e.Message);
        }
    }

    public virtual async Task<Result<TDtoModel>> UpdateAsync(TKey id, TDtoModel dtoModel)
    {
        try
        {
            TEntity entity = mapper.Map<TEntity>(dtoModel);
            TEntity? returnEntity = await repository.UpdateAsync(id, entity);
            if (returnEntity == null)
            {
                return Result<TDtoModel>.Fail("The record could not be updated");
            }

            var dto = mapper.Map<TDtoModel>(returnEntity);
            return Result<TDtoModel>.Ok(dto);
        }
        catch (Exception e)
        {

            return Result<TDtoModel>.Fail(e.Message);
        }
    }

    public virtual async Task<Result> DeleteAsync(TKey id)
    {
        try
        {
            await repository.DeleteAsync(id);
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }

    public Task<Result<bool>> ExistsAsync(Func<IQueryable<TDtoModel>, bool> predicate)
    {
        try
        {
            //obtenemos las entidades
            var queryable = repository.GetAllQueryable();
        
            //mostramos los dtos
            var dtoQueryable = queryable.Select(e => mapper.Map<TDtoModel>(e));
        
            var exists = predicate(dtoQueryable);
        
            return Task.FromResult(Result<bool>.Ok(exists));
        }
        catch (Exception e)
        {
            return Task.FromResult(Result<bool>.Fail(e.Message));
        }
    }
}