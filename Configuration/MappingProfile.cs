using AutoMapper;
using LeagueOfItems.Models;
using LeagueOfItems.ViewModels;

namespace LeagueOfItems.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Item, ItemViewModel>();
        }
    }
}