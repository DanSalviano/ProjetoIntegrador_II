using PizzaDelivery.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Models
{
    [Table("Categoria")]
    public class CategoriaModel
    {
        protected CategoriaModel() => Id = Guid.NewGuid().ToString();

        [Key]
        [MaxLength(36)]
        public string Id { get; set; }

        [MaxLength(80)]
        public string Nome { get; set; }

        public OrderGroup OrderGroup { get; set; } 
    }
}