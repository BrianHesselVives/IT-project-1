using AutoMapper;
using MassageHuis.Entities;
using MassageHuis.ViewModels;
using NuGet.Protocol.Plugins;

namespace MassageHuis.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MasseurVM, Masseur>();
        }
    }
}
