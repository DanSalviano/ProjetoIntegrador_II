using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.Models
{
    [Table("Endereco")]
    public class EnderecoModel
    {
        protected EnderecoModel() => Id = Guid.NewGuid().ToString();

        [Key]
        [MaxLength(36)]
        public string Id { get; set; }

        [MaxLength(36)]
        public string UsuarioId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Logradouro { get; set; }

        [MaxLength(8)]
        public string Numero { get; set; }

        [MaxLength(50)]
        public string Bairro { get; set; }

        [Required]
        [MaxLength(36)]
        public string CidadeId { get; set; }

        [Required, Column(TypeName = "char(2)")]
        public string UF { get; set; }

        [MaxLength(100)]
        public string Complemento { get; set; }

        [Required, Column(TypeName = "char(9)")]
        public string CEP { get; set; }

        public string Referencia { get; set; }

        public bool Selecionado { get; set; }

    }
}

