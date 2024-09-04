using PizzaDelivery.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Models
{
    [Table("Estado")]
    public class EstadoModel : IAuditable, ISoftDeletable
    {

        [Key]
        [MaxLength(2)]
        [Column(TypeName = "char(2)")]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Estado { get; set; }

        [Required]
        public string Capital { get; set; }


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
