using AutoMapper;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Profiles
{

    public class CidadeProfile : Profile
    {
        public CidadeProfile()
        {
            CreateMap<CidadeModel, CidadeAddOrEditViewModel>();

            CreateMap<CidadeAddOrEditViewModel, CidadeModel>()
                .ForMember(dest => dest.Id, map => map.Ignore()).AfterMap((map, dest) => { dest.Id = map.Id ?? dest.Id; })
                .ForMember(dest => dest.UsuarioIdInclusao, map => map.Ignore());
        }
    }
}
