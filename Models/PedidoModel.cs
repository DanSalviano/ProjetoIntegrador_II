using Org.BouncyCastle.Cms;
using PizzaDelivery.Interfaces;
using PizzaDelivery.ViewModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Models
{
    [Table("Pedido")]
    public class PedidoModel : IAuditable, ISoftDeletable
    {
        protected PedidoModel() => Id = Guid.NewGuid().ToString();

        [Key]
        [MaxLength(36)]
        public string Id { get; set; }

        [Required]
        public PedidoStatus Status { get; set; }

        [Required]
        [MaxLength(200)]
        public string Logradouro { get; set; }

        [Required]
        [MaxLength(10)]
        public string Numero { get; set; }

        [MaxLength(200)]
        public string Complemento { get; set; }

        [Required]
        [MaxLength(50)]
        public string Bairro { get; set; }

        [Required]
        [MaxLength(50)]
        public string Cidade { get; set; }

        [Required]
        [Column(TypeName = "char(2)")]
        public string Estado { get; set; }

        private string _cep;
        [Required(ErrorMessage = "Digite o {0}")]
        [Column(TypeName = "char(9)")]
        public string CEP
        {
            get => _cep ?? string.Empty;
            set
            {
                _cep = value.Replace(".", "").Replace("-", "");
            }
        }

        [MaxLength(200)]
        public string Referencia { get; set; }

        [Required]
        [MaxLength(10)]
        public FormaPagamento FormaPagamento { get; set; }

        public double? Troco { get; set; }

        [MaxLength(200)]
        public string Observacao { get; set; }



        //*********[ Propriedades para Auditoria ]*********

        [DataType(DataType.Date)]
        public DateTime? DataInicioPreparo { get; set; }

        [MaxLength(36)]
        public string UsuarioIdInicioPreparo { get; set; }


        [DataType(DataType.Date)]
        public DateTime? DataFimPreparo { get; set; }
        [MaxLength(36)]
        public string UsuarioIdFimPreparo { get; set; }



        [DataType(DataType.Date)]
        public DateTime? DataInicioEntrega { get; set; }

        [MaxLength(36)]
        public string UsuarioIdInicioEntrega { get; set; }


        [DataType(DataType.Date)]
        public DateTime? DataFimEntrega { get; set; }

        [MaxLength(36)]
        public string UsuarioIdFimEntrega { get; set; }




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
