using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{
    public class EstadoAddOrEditViewModel
    {
        [DisplayName("UF")]
        [Required(ErrorMessage = "Informe a {0}.")]
        [StringLength(2, ErrorMessage = "A {0} excedeu os {1} caracteres.")]
        public string Id { get; set; }

        public bool IsNovoCadastro { get; set; }

        [Required(ErrorMessage = "Informe o {0}.")]
        [StringLength(100, ErrorMessage = "O {0} excedeu os {1} caracteres.")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "Informe a {0}.")]
        public string Capital { get; set; }


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


    }
}
