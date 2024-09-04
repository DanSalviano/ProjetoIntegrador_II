using AutoMapper;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Profiles
{

    public class PedidoItemProfile : Profile
    {
        public PedidoItemProfile()
        {
            CreateMap<ShoppingCartItemsViewModel, PedidoItemModel>()
                .ForMember(dest => dest.PedidoId, map => map.Ignore());
        }
    }
}
