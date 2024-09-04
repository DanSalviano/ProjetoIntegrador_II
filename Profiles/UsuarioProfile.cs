using AutoMapper;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Profiles
{

    public class UsuarioProfile : Profile
    {
        public UsuarioProfile()
        {
            CreateMap<UsuarioModel, UsuarioAddOrEditViewModel>()
                .ForMember(dest => dest.Telefone, map => map.MapFrom(src => src.PhoneNumber));

            CreateMap<UsuarioAddOrEditViewModel, UsuarioModel>()
                .ForMember(dest => dest.Id, map => map.Ignore()).AfterMap((map, dest) => { dest.Id = map.Id ?? dest.Id; })
                .ForMember(dest => dest.UsuarioIdInclusao, map => map.Ignore())

                .ForMember(dest => dest.UserName, map => map.Ignore()).AfterMap((map, dest) => { dest.UserName = dest.CPF; })
                .ForMember(dest => dest.NormalizedUserName, map => map.Ignore()).AfterMap((map, dest) => { dest.NormalizedUserName = dest.CPF; })
                .ForMember(dest => dest.NormalizedEmail, map => map.MapFrom(src => src.Email.ToUpper().Trim()))
                .ForMember(dest => dest.PhoneNumber, map => map.MapFrom(src => src.Telefone));
        }
    }
}