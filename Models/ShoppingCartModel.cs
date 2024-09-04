using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Models
{
    [Table("ShoppingCart")]
    [PrimaryKey("Id", "ProdutoId")] // [Id, ProdutoId]]
    public class ShoppingCartModel
    {
        [MaxLength(36)]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }

        [MaxLength(36)]
        [Column(Order = 1)]
        public string ProdutoId { get; set; }

        [MaxLength(36)]
        public string UsuarioId { get; set; }

        [MaxLength(3)]
        public int Quantidade { get; set; }

        [Required]
        public DateTime? Expiracao { get; set; }
    }
}
