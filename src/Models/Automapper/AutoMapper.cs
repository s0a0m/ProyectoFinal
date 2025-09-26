namespace src.Models.automapper;

using AutoMapper;
using src.Models;
using src.Models.CodeFirst;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // EF → Dominio
        CreateMap<proveedor, Proveedor>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id_proveedor))
            .ForMember(dest => dest.razonSocial, opt => opt.MapFrom(src => src.razon_social))
            .ForMember(dest => dest.personaResponsable, opt => opt.MapFrom(src => src.persona_responsable))
            // .ForMember(dest => dest.condicion, opt => opt.MapFrom(src => src.id_condicion_pago_habitualNavigation))
            .ForMember(dest => dest.condicion, opt => opt.Ignore())
            // .ForMember(dest => dest.direccion, opt => opt.MapFrom(src => src.id_domicilioNavigation));
            .ForMember(dest => dest.direccion, opt => opt.Ignore());


        CreateMap<cuota, Cuota>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id_condicion_pago))
            .ForMember(dest => dest.dias_pago, opt => opt.MapFrom(src => src.dias_pago))
            .ForMember(dest => dest.numeroCuotas, opt => opt.MapFrom(src => src.cuotas))
            .ForMember(dest => dest.interes_porcentual, opt => opt.MapFrom(src => (float)src.interes_porcentual));

        CreateMap<contado, Contado>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id_condicion_pago))
            .ForMember(dest => dest.dias_pago, opt => opt.MapFrom(src => src.dias_pago));

        // Dominio → EF
        CreateMap<Proveedor, proveedor>()
            .ForMember(dest => dest.id_proveedor, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.razon_social, opt => opt.MapFrom(src => src.razonSocial))
            .ForMember(dest => dest.persona_responsable, opt => opt.MapFrom(src => src.personaResponsable))
            // .ForMember(dest => dest.id_condicion_pago_habitualNavigation, opt => opt.MapFrom(src => src.condicion))
            .ForMember(dest => dest.id_condicion_pago_habitualNavigation, opt => opt.Ignore())
            // .ForMember(dest => dest.id_domicilioNavigation, opt => opt.MapFrom(src => src.direccion));
             .ForMember(dest => dest.id_domicilioNavigation, opt => opt.Ignore());

        CreateMap<Cuota, cuota>()
            .ForMember(dest => dest.id_condicion_pago, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.dias_pago, opt => opt.MapFrom(src => src.dias_pago))
            .ForMember(dest => dest.cuotas, opt => opt.MapFrom(src => src.numeroCuotas))
            .ForMember(dest => dest.interes_porcentual, opt => opt.MapFrom(src => (decimal)src.interes_porcentual));

        CreateMap<Contado, contado>()
            .ForMember(dest => dest.id_condicion_pago, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.dias_pago, opt => opt.MapFrom(src => src.dias_pago));
            

        // Mapeos para Provincia
        CreateMap<provincia, Provincia>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id_provincia))
            .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.nombre));

        CreateMap<Provincia, provincia>()
            .ForMember(dest => dest.id_provincia, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.nombre, opt => opt.MapFrom(src => src.Nombre))
            .ForMember(dest => dest.domicilios, opt => opt.Ignore());

        // Mapeos para Dirección
        CreateMap<domicilio, Direccion>()
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id_domicilio))
            .ForMember(dest => dest.id_provincia, opt => opt.MapFrom(src => src.id_provincia))
            .ForMember(dest => dest.calle, opt => opt.MapFrom(src => src.calle))
            .ForMember(dest => dest.numero, opt => opt.MapFrom(src => src.numero))
            .ForMember(dest => dest.piso, opt => opt.MapFrom(src => src.piso))
            .ForMember(dest => dest.comentario, opt => opt.MapFrom(src => src.comentario));

        CreateMap<Direccion, domicilio>()
            .ForMember(dest => dest.id_domicilio, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.id_provincia, opt => opt.MapFrom(src => src.id_provincia))
            .ForMember(dest => dest.calle, opt => opt.MapFrom(src => src.calle))
            .ForMember(dest => dest.numero, opt => opt.MapFrom(src => src.numero))
            .ForMember(dest => dest.piso, opt => opt.MapFrom(src => src.piso))
            .ForMember(dest => dest.comentario, opt => opt.MapFrom(src => src.comentario))
            .ForMember(dest => dest.id_provinciaNavigation, opt => opt.Ignore())
            .ForMember(dest => dest.proveedores, opt => opt.Ignore());


    }
}
