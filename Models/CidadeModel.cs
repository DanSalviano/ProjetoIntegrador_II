using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PizzaDelivery.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Models
{
    [Table("Cidade")]
    public class CidadeModel : IAuditable, ISoftDeletable
    {
        protected CidadeModel() => Id = Guid.NewGuid().ToString();

        [Key]
        [MaxLength(36)]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Cidade { get; set; }

        [Required]
        [MaxLength(2)]
        [Column(TypeName = "char(2)")]
        public string EstadoId { get; set; }



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
