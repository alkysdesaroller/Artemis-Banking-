using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mappings.DtosAndEntities;

public class BeneficiaryMappingProfile : Profile 
{
    public BeneficiaryMappingProfile()
    {
        // La idea es que tengas una clase Entidad (y un ViewModel) y un Dto que sean simetricos. 
        // Por eso tienes que tener un Dto central, que literalmente sea la copia de la clase Entidad pero le cambias
        // el nombre a Dto.
        // Si dicho dicha clase entidad tiene propiedades de navegacion a otras, los Dto tambien tienen propiedades de
        // navegacion a objetos que son la contraparte Dto de esas entidades. 
        
        // De hecho, revisa las clases de Beneficiary y BeneficiaryDto para que veas lo que digo:
        // Tienes que hacer esto, pues el GenericService LITERALMENTE depende de IMapper y IMapper necesita que 
        // los objetos sean identicos para mapearlos.
        
        // Tambien es bueno mencionar que, aunque en las clases de Entidad no existan propiedades de navegacion hacia AppUser,
        // Deberias de poner propiedades de navegacion de tipo UserDto en los Dto que tengan una relacion con usuarios.
        // Esto te permite llenar esos objetos UserDto cuando sean necesario en los servicios de la APP
        CreateMap<Beneficiary, BeneficiaryDto>().ReverseMap();
    }
}