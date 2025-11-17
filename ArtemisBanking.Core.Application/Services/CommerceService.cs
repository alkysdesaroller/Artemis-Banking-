using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Application.Dtos.CardTransaction;
using ArtemisBanking.Core.Application.Dtos.Commerce;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Core.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services;

public class CommerceService : GenericServices<int, Commerce, CommerceDto>, ICommerceService 
{
    private readonly ICommerceRepository _commerceRepository;
    private readonly IMapper _mapper;
    
    public CommerceService(ICommerceRepository repository, IMapper mapper) : base(repository, mapper)
    {
        _commerceRepository = repository;
        _mapper = mapper;
    }

    public async Task<Result<int>> GetCommerceIdByUserIdAsync(string userId)
    {
        // Nota: Si no hay una relación directa entre User y Commerce,
        // necesitarías agregar un campo UserId a Commerce o crear una tabla de relación.
        // Por ahora, asumimos que el primer Commerce activo es el del usuario.
        // Esto debería ajustarse según tu modelo de datos real.
        var commerce = await _commerceRepository.GetAllQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IsActive);
        
        if (commerce == null)
        {
            return Result<int>.Fail("No se encontró un comercio activo para este usuario.");
        }
        
        return Result<int>.Ok(commerce.Id);
    }
}