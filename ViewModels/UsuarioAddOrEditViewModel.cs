using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.ViewModels
{
    public class UsuarioAddOrEditViewModel
    {
        public string Id { get; set; }


        [Display(Name = "Estado")]
        public string EstadoId { get; set; }
        public List<SelectListItem> Estados { get; set; }


        [Display(Name = "Cidade")]
        [Required(ErrorMessage = "Escolha a {0}.")]
        public string CidadeId { get; set; }


        [Display(Name = "Nome Completo")]
        [Required(ErrorMessage = "Informe o {0}.")]
        [StringLength(100, ErrorMessage = "O tamanho máximo do {0} é de {1} caracteres.")]
        public string NomeCompleto { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Data De Nascimento")]
        [Required(ErrorMessage = "Informe a {0}.")]
        public DateTime? DataNascimento { get; set; }

        private string _cpf;
        [Display(Name = "CPF")]
        [Required(ErrorMessage = "Informe o {0}.")]
        [StringLength(14, ErrorMessage = "O campo {0} deve ter {1} dígitos.")]

        public string CPF
        {
            get
            {
                if (string.IsNullOrEmpty(_cpf))
                    return string.Empty;

                return _cpf;
            }
            set
            {
                value = value.Replace(".", "").Replace("-", "");
                _cpf = $"{value.Substring(0, 3)}.{value.Substring(3, 3)}.{value.Substring(6, 3)}-{value.Substring(9, 2)}";
            }
        }


        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Informe o {0}.")]
        [StringLength(15, ErrorMessage = "O {0} deve ter {1} dígitos.")]
        public string Telefone { get; set; }

        [Display(Name = "E-mail")]
        [DataType(DataType.EmailAddress)] // Define o tipo de dados, mas não valida.
        [Required(ErrorMessage = "Informe o {0}.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")] // Valida o tipo de dados.
        [AllowedEmailDomain(ErrorMessage = "O domínio do email não é permitido.")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Informe a {0}.")]
        [StringLength(16, MinimumLength = 3, ErrorMessage = "A {0} deve ter de {2} à {1} caracteres.")]
        [DataType(DataType.Password, ErrorMessage = "A {0} pelo menos uma letra minúscula e maiúscula")]
        public string Senha { get; set; }


        [Display(Name = "Confirmação da Senha")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Informe a {0}.")]
        [MaxLength(16, ErrorMessage = "O tamanho máximo do campo {0} é de {1} caracteres.")]
        [MinLength(6, ErrorMessage = "O tamanho mínimo do campo {0} é de {1} caracteres.")]
        [Compare(nameof(Senha), ErrorMessage = "A {0} não confere com a senha.")]
        public string ConfSenha { get; set; }



        [DisplayName("Inclusão")]
        public string UsuarioIdInclusao { get; set; }

        //[DataType(DataType.Date)]
        [DisplayName("Data da Inclusão")]
        public DateTime? DataInclusao { get; set; }

        [DisplayName("Última Alteração")]
        public string UsuarioIdAlteracao { get; set; }

        //[DataType(DataType.Date)]
        [DisplayName("Data da Alteração")]
        public DateTime? DataAlteracao { get; set; }



        [DisplayName("Ativo")]
        public bool IsAtivo { get; set; } = true;

        [DisplayName("Excluído")]
        public bool IsExcluido { get; set; }

        [DisplayName("Exclusão")]
        public string UsuarioIdExclusao { get; set; }

        //[DataType(DataType.Date)]
        [DisplayName("Data da Exclusão")]
        public DateTime? DataExclusao { get; set; }



        [NotMapped]
        [Display(Name = "Idade")]
        public int Idade => (int)Math.Floor((DateTime.Now - (DateTime)DataNascimento).TotalDays / 365.25);
    }
}