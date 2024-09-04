using AutoMapper;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Profiles
{

    public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            CreateMap<PedidoModel, PedidoViewModel>();

            CreateMap<PedidoViewModel, PedidoModel>()
                .ForMember(dest => dest.Id, map => map.Ignore()).AfterMap((map, dest) => { dest.Id = map.Id ?? dest.Id; })
                .ForMember(dest => dest.UsuarioIdInclusao, map => map.Ignore());
        }
    }
}
