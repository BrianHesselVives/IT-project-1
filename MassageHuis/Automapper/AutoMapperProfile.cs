using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.Models;
using MassageHuis.ViewModels;
using NuGet.Protocol.Plugins;

namespace MassageHuis.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<KostPrijs, KostPrijsVM>();
            CreateMap<KostPrijsVM, KostPrijs>();
            CreateMap<RegulierTijdslot, RegulierTijdslotVM>();
            CreateMap<RegulierTijdslotVM, RegulierTijdslot>();
            CreateMap<Schema, SchemaVM>();
            CreateMap<SchemaVM, Schema>();
            CreateMap<Masseur,MasseurVM >();
            CreateMap<MasseurVM, Masseur>();
            CreateMap<ReservatieVM, Reservatie>();
            CreateMap<Reservatie, ReservatieVM>()
                .ForMember(dest => dest.DatumReservatie,
                   opts => opts.MapFrom(
                       src => src.DatumReservatie
                   ))
                .ForMember(dest => dest.DatumCreatie,
                    opts => opts.MapFrom(
                        src => src.DatumCreatie
                    ))
                .ForMember(dest => dest.Status,
                    opts => opts.MapFrom(
                        src => src.Status
                    ))
                .ForMember(dest => dest.MasseurId,
                    opts => opts.MapFrom(
                        src => src.IdMasseur
                    ))
                .ForMember(dest => dest.IdAspNetUsers,
                    opts => opts.MapFrom(
                        src => src.IdAspNetUsers
                    ))
                .ForMember(dest => dest.IdPrijs,
                    opts => opts.MapFrom(
                        src => src.IdPrijs
                    ))
                .ForMember(dest => dest.IdPromotieCode,
                    opts => opts.MapFrom(
                        src => src.IdPromotieCode
                    ))
                .ForMember(dest => dest.IdTijdSlot,
                    opts => opts.MapFrom(
                        src => src.IdRegulierTijdslot
                    ))
                .ForMember(dest => dest.MasseurNaam,
                    opts => opts.MapFrom(
                        src => src.IdMasseur
                    ))
                .ForMember(dest => dest.IdTypeMassage,
                    opts => opts.MapFrom(
                        src => src.IdTypeMassage
                    ))
                .ForMember(dest => dest.TeBetalenBedrag,
                    opts => opts.MapFrom(
                        src => src.TeBetalenBedrag
                    ))
                .ForMember(dest => dest.MasseurNaam,
                    opts => opts.MapFrom(
                        src => src.IdMasseurNavigation.IdAspNetUsersNavigation.Voornaam + " " +src.IdMasseurNavigation.IdAspNetUsersNavigation.Naam
                    )).ForMember(dest => dest.TypeMassage,
                        opts => opts.MapFrom(
                            src => src.IdTypeMassageNavigation.Type
                        ))
                    .ForMember(dest => dest.KlantNaam,
                    opts => opts.MapFrom(
                        src => src.IdAspNetUsersNavigation.Voornaam + " " + src.IdAspNetUsersNavigation.Naam
                    ));
                    }
    }
}
