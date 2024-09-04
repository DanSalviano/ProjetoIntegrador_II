using System.ComponentModel;
using PizzaDelivery.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Models
{
    [Table("Produto")]
    public class ProdutoModel : IAuditable, ISoftDeletable
    {
        protected ProdutoModel() => Id = Guid.NewGuid().ToString();

        [Key]
        [MaxLength(36)]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Produto { get; set; }

        [Required]
        [MaxLength(200)]
        public string Descricao { get; set; }

        [MaxLength(4)]
        public int Conteudo { get; set; }

        [Required]
        [MaxLength(8)]
        public string Medida { get; set; } // l, unid, m, cm, kg, g

        [Required]
        [MaxLength(400)]
        public string Ingredientes { get; set; }

        [Required]
        [MaxLength(36)]
        public string CategoriaId { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Preco { get; set; }


        [MaxLength(60)]
        public string NomeArquivoImagem { get; set; }



        //*********[ Propriedades para Auditoria ]*********

        [Required]
        [MaxLength(36)]
        public string UsuarioIdInclusao { get; set; }

        [ReadOnly(true)]
        [DataType(DataType.Date)]
        public DateTime? DataInclusao { get; set; }

        [MaxLength(36)]
        public string UsuarioIdAlteracao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataAlteracao { get; set; }



        [Required]
        public bool IsAtivo { get; set; }

        [Required]
        public bool IsExcluido { get; set; }

        [MaxLength(36)]
        public string UsuarioIdExclusao { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataExclusao { get; set; }
    }
}
