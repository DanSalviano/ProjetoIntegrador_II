using AutoMapper;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Profiles
{

    public class ProdutoProfile : Profile
    {
        public ProdutoProfile()
        {
            CreateMap<ProdutoModel, ProdutoAddOrEditViewModel>();

            CreateMap<ProdutoAddOrEditViewModel, ProdutoModel>()
                .ForMember(dest => dest.Id, map => map.Ignore()).AfterMap((map, dest) => { dest.Id = map.Id ?? dest.Id; })
                .ForMember(dest => dest.UsuarioIdInclusao, map => map.Ignore());
        }
    }
}
