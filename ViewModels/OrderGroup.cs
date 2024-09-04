using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{
    public enum OrderGroup
    {
        [Display(Name = "Grupo 1")]
        grupo1,
        [Display(Name = "Grupo 2")]
        grupo2,
        [Display(Name = "Grupo 3")]
        grupo3,
        [Display(Name = "Grupo 4")]
        grupo4,
        [Display(Name = "Grupo 5")]
        grupo5,
        [Display(Name = "Grupo 6")]
        grupo6
    }
}
