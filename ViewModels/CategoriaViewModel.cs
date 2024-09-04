using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.ViewModels
{
    [Table("Categoria")]
    public class CategoriaViewModel
    {
        public string Id { get; set; }

        [StringLength(80, ErrorMessage = "O {0} excedeu {1} caracteres.")]
        public string Nome { get; set; }

        [DisplayName("Grupo de Ordenação")]
        public OrderGroup OrderGroup { get; set; }
    }
}