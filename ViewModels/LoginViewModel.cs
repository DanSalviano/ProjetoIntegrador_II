using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "CPF ( somente números )")]
        [Required(ErrorMessage = "Digite o {0}.")]
        public string Usuario { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Digite a {0}.")]
        public string Senha { get; set; }

        [Required]
        [Display(Name = "Lembrar de mim")]
        public bool Lembrar { get; set; }

        public string ReturnUrl { get; set; }
    }
}