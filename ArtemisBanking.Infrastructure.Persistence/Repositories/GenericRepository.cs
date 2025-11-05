using ArtemisBanking.Core.Domain.Interfaces;
using ArtemisBanking.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Infrastructure.Persistence.Repositories;


public class GenericRepository<TKey,TEntity> : IGenericRepository<TKey,TEntity>  where TEntity : class
{
    protected readonly ArtemisContext Context;

    public GenericRepository(ArtemisContext context)
    {
        this.Context = context;
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await Context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TKey id)
    {
        return await Context.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity);
        await Context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<TEntity>> AddRangeAsync(List<TEntity> entities)
    {
        await Context.Set<TEntity>().AddRangeAsync(entities);
        await Context.SaveChangesAsync();
        return entities;
    }

    public async Task<TEntity?> UpdateAsync(TKey id, TEntity entity)
    {
        var entityToUpdate = await Context.Set<TEntity>().FindAsync(id);
        if (entityToUpdate != null)
        {
            Context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
            await Context.SaveChangesAsync();
            return entityToUpdate;
        }

        return null;
    }

    public async Task DeleteAsync(TKey entity)
    {
        var entityToDelete = await Context.Set<TEntity>().FindAsync(entity);
        if (entityToDelete != null)
        {
            Context.Set<TEntity>().Remove(entityToDelete);
            await Context.SaveChangesAsync();
        }
    }

    public IQueryable<TEntity> GetAllQueryable()
    {
        return Context.Set<TEntity>().AsQueryable();
    }
}