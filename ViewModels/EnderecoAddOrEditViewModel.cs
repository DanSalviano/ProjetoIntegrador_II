using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PizzaDelivery.ViewModels
{
    public class EnderecoAddOrEditViewModel
    {
        public string Id { get; set; }

        public string UsuarioId { get; set; }

        public string Descricao {  get; set; }

        [Required]
        [StringLength(255)]
        public string Logradouro { get; set; }

        [StringLength(8)]
        public string Numero { get; set; }
        [StringLength(50)]
        public string Bairro { get; set; }

        [Required]
        public string CidadeId { get; set; }

        [StringLength(50)]
        public string Cidade { get; set; }

        [Required]
        [MaxLength(2)]
        public string UF { get; set; }

        [StringLength(100)]
        public string Complemento { get; set; }


        [Required]
        [StringLength(9)]
        public string CEP { get; set; }

        public string Referencia { get; set; }

        public bool Selecionado { get; set; }

        [NotMapped]
        public string EnderecoCompleto
        {
            get
            {
                return $"{Logradouro}, {Numero} {Complemento}, {Bairro}, {Cidade}, {UF}, CEP {CEP}";
            }
        }
    }
}

