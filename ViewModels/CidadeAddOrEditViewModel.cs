using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PizzaDelivery.ViewModels
{
    public class CidadeAddOrEditViewModel
    {
        public string Id { get; set; }


        [DisplayName("UF")]
        [Required(ErrorMessage = "Escolha a {0}.")]
        public string EstadoId { get; set; }

        public List<SelectListItem> Estados { get; set; }


        [Required(ErrorMessage = "Informe o {0}.")]
        [StringLength(100, ErrorMessage = "O {0} excedeu os {1} caracteres.")]
        public string Cidade { get; set; }



        [DisplayName("Inclusão")]
        public string UsuarioIdInclusao { get; set; }

        //[DataType(DataType.Date)]
        [DisplayName("Data da Inclusão")]
        public DateTime? DataInclusao { get; set; }

        [DisplayName("Última Alteração")]
        public string UsuarioIdAlteracao { get; set; }

        //[DataType(DataType.Date)]  //Essa anotação mostra somente a data sem horário
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
