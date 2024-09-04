using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{

    public enum FormaPagamento
    {
        [Display(Name = "Pix")]
        Pix = 1,
        [Display(Name = "Cartão")]
        Card = 2,
        [Display(Name = "Dinheiro")]
        Cash = 3
    }
}
