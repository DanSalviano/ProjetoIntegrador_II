using AutoMapper;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Profiles
{
    public class EstadoProfile : Profile
    {
        public EstadoProfile()
        {
            CreateMap<EstadoModel, EstadoAddOrEditViewModel>();
            //.ForMember(dest => dest.EnderecoCompleto, map => map.Ignore());

            CreateMap<EstadoAddOrEditViewModel, EstadoModel>()
                .ForMember(dest => dest.Id, map => map.Ignore()).AfterMap((map, dest) => { dest.Id = map.Id ?? dest.Id; })
                .ForMember(dest => dest.UsuarioIdInclusao, map => map.Ignore());
            //.ForMember(dest => dest.DataInclusao, map => map.Ignore())
            //.ForMember(dest => dest.UsuarioIdAlteracao, map => map.Ignore())
            //.ForMember(dest => dest.DataAlteracao, map => map.Ignore())
            //.ForMember(dest => dest.UsuarioIdExclusao, map => map.Ignore())
            //.ForMember(dest => dest.DataExclusao, map => map.Ignore());
        }
    }
}