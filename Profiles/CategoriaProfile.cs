using AutoMapper;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Profiles
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile() => CreateMap<CategoriaModel, CategoriaViewModel>().ReverseMap();
    }
}