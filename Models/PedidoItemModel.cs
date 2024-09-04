using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Models
{
    [Table("PedidoItem")]
    [PrimaryKey("PedidoId", "ProdutoId")]
    public class PedidoItemModel
    {
        [MaxLength(36)]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PedidoId { get; set; }

        [MaxLength(36)]
        [Column(Order = 1)]
        public string ProdutoId { get; set; }

        public int Quantidade { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Preco { get; set; }
    }
}